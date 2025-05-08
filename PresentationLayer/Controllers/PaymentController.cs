using BusinessLogicLayer.DTOs.PaymentDtos;
using BusinessLogicLayer.Manager.OrderManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PaymentController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(
            IOrderManager orderManager, 
            IConfiguration configuration,
            UserManager<User> userManager,
            ILogger<PaymentController> logger,
            IUnitOfWork unitOfWork)
        {
            _orderManager = orderManager;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int orderId)
        {
            var order = await _orderManager.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string stripeToken, CheckoutViewModel model)
        {
            _logger.LogInformation($"Processing payment for order {orderId}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid checkout model state");
                var validationErrors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning($"Validation errors: {validationErrors}");
                return RedirectToAction("Checkout", new { orderId });
            }
            
            var order = await _orderManager.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found during payment processing");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation($"Creating Stripe customer with email {model.Email}");
                
                // Create a customer with the provided email
                var customerOptions = new CustomerCreateOptions
                {
                    Email = model.Email,
                    Name = model.Name,
                    Source = stripeToken,
                    Address = new AddressOptions
                    {
                        Line1 = model.Address,
                        City = model.City,
                        PostalCode = model.PostalCode,
                        Country = model.Country
                    }
                };
                
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);

                _logger.LogInformation($"Stripe customer created: {customer.Id}");

                // Create a charge
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Convert to cents
                    Currency = "usd",
                    Description = $"Payment for Order #{order.OrderId}",
                    Customer = customer.Id,
                    ReceiptEmail = model.Email,
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", order.OrderId.ToString() },
                        { "UserId", order.UserId }
                    }
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);

                _logger.LogInformation($"Charge created with status: {charge.Status}");

                if (charge.Status == "succeeded")
                {
                    // Create an invoice
                    var invoiceOptions = new InvoiceCreateOptions
                    {
                        Customer = customer.Id,
                        CollectionMethod = "send_invoice",
                        DaysUntilDue = 30,
                        Metadata = new Dictionary<string, string>
                        {
                            { "OrderId", order.OrderId.ToString() }
                        }
                    };

                    var invoiceService = new InvoiceService();
                    var invoice = await invoiceService.CreateAsync(invoiceOptions);

                    _logger.LogInformation($"Invoice created: {invoice.Id}");

                    // Add invoice items
                    foreach (var item in order.Items)
                    {
                        var invoiceItemOptions = new InvoiceItemCreateOptions
                        {
                            Customer = customer.Id,
                            Amount = (long)(item.Price * 100),
                            Currency = "usd",
                            Description = item.CourseTitle
                        };
                        await new InvoiceItemService().CreateAsync(invoiceItemOptions);
                    }

                    // Send the invoice
                    await invoiceService.SendInvoiceAsync(invoice.Id);
                    _logger.LogInformation($"Invoice sent to {model.Email}");

                    // Update order status
                    await _orderManager.UpdateOrderStatusAsync(orderId, "Completed");
                    _logger.LogInformation($"Order {orderId} marked as completed");
                    
                    // Create enrollments for each course in the order
                    foreach (var item in order.Items)
                    {
                        var enrollment = new Enrollment
                        {
                            Course_ID = item.CourseId,
                            User_ID = userId,
                            Enrollment_date = DateTime.UtcNow
                        };
                        
                        // Check if the enrollment already exists
                        var existingEnrollment = await _unitOfWork.Enrollments.FindAsync(
                            e => e.Course_ID == item.CourseId && e.User_ID == userId);
                            
                        if (existingEnrollment == null)
                        {
                            await _unitOfWork.Enrollments.AddAsync(enrollment);
                            _logger.LogInformation($"Created enrollment for user {userId} in course {item.CourseId}");
                        }
                    }
                    
                    // Save changes to database
                    await _unitOfWork.CompleteAsync();

                    TempData["Success"] = "Payment successful! Your courses are now available in your dashboard. An invoice has been sent to your email.";
                    return RedirectToAction("Success", new { orderId });
                }

                _logger.LogWarning($"Payment failed with charge status: {charge.Status}");
                ViewBag.Error = "Payment failed.";
                return View("Error");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error during payment processing: {ex.Message}");
                ViewBag.Error = ex.StripeError.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during payment processing: {ex.Message}");
                ViewBag.Error = "An unexpected error occurred. Please try again later.";
                return View("Error");
            }
        }

        public IActionResult Success(int orderId)
        {
            return View(orderId);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
