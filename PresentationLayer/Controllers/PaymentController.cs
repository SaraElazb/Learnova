using BusinessLogicLayer.DTOs.PaymentDtos;
using BusinessLogicLayer.Manager.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace PresentationLayer.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IConfiguration _configuration;

        public PaymentController(IOrderManager orderManager, IConfiguration configuration)
        {
            _orderManager = orderManager;
            _configuration = configuration;
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
        public async Task<IActionResult> ProcessPayment(int orderId, string stripeToken)
        {
            var order = await _orderManager.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            try
            {
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), 
                    Currency = "usd",
                    Description = $"Payment for Order {order.OrderId}",
                    Source = stripeToken
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);

                if (charge.Status == "succeeded")
                {
                    order.Status = "Completed";
                    return RedirectToAction("Success", new { orderId });
                }

                ViewBag.Error = "Payment failed.";
                return View("Error");
            }
            catch (StripeException ex)
            {
                ViewBag.Error = ex.StripeError.Message;
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
