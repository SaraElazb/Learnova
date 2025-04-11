using BusinessLogicLayer.DTOs.ShoppingCartDtos;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.ShoppingCartManager;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.DTOs.CourseDtos;

namespace PresentationLayer.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICourseManager _courseManager;

        public ShoppingCartController(ICourseManager courseManager)
        {
            _courseManager = courseManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<ShoppingCartDto>("Cart") ?? new ShoppingCartDto();
            var cartManager = new ShoppingCartManager(cart);
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var course = await _courseManager.GetByIdAsync(id);
            if (course != null)
            {
                var cart = HttpContext.Session.GetObject<ShoppingCartDto>("Cart") ?? new ShoppingCartDto();
                var cartManager = new ShoppingCartManager(cart);

                var courseDto = new CourseDTO
                {
                    Course_ID = course.Course_ID,
                    Title = course.Title,
                    Description = course.Description,
                    Price = course.Price,
                    Rating = course.Rating,
                    ImagePath = course.ImagePath,
                    CreatedDate = course.CreatedDate,
                    IsActive = course.IsActive,
                    CategoryName = course.Category?.Category_Name ?? "Uncategorized"
                };

                cartManager.AddToCart(courseDto); 
                HttpContext.Session.SetObject("Cart", cart);
            }
            return RedirectToAction("Index");
        }


        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObject<ShoppingCartDto>("Cart") ?? new ShoppingCartDto();
            var cartManager = new ShoppingCartManager(cart);
            cartManager.RemoveFromCart(id);
            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }
    }
}
