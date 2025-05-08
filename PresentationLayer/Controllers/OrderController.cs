using BusinessLogicLayer.Manager.OrderManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PresentationLayer.Extensions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderManager orderManager, ILogger<OrderController> logger)
        {
            _orderManager = orderManager;
            _logger = logger;
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                _logger.LogInformation("Checkout process initiated");
                
                // Get cart from session
                var cart = HttpContext.Session.GetObject<BusinessLogicLayer.DTOs.ShoppingCartDtos.ShoppingCartDto>("Cart");
                
                if (cart == null || !cart.Items.Any())
                {
                    _logger.LogWarning("Checkout attempted with empty cart");
                    TempData["Error"] = "Your cart is empty. Please add courses before checkout.";
                    return RedirectToAction("Index", "ShoppingCart");
                }

                // Get current user ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found during checkout");
                    TempData["Error"] = "You must be logged in to checkout.";
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "ShoppingCart") });
                }
                
                _logger.LogInformation($"Creating order for user {userId} with {cart.Items.Count} items");
                
                // Create the order
                var orderId = await _orderManager.CreateOrderAsync(userId, cart);
                
                _logger.LogInformation($"Order created successfully. Order ID: {orderId}");

                // Clear the cart after order creation
                HttpContext.Session.Remove("Cart");

                // Redirect to payment
                return RedirectToAction("Checkout", "Payment", new { orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout process");
                TempData["Error"] = "An error occurred during checkout. Please try again.";
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var orders = await _orderManager.GetUserOrdersAsync(userId);
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user orders");
                TempData["Error"] = "An error occurred while retrieving your orders.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderManager.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning($"Order with ID {id} not found");
                    return NotFound();
                }

                // Verify the order belongs to the current user
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (order.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to access order {id} belonging to {order.UserId}");
                    return Forbid();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order details for order {id}");
                TempData["Error"] = "An error occurred while retrieving order details.";
                return RedirectToAction("MyOrders");
            }
        }
    }
} 