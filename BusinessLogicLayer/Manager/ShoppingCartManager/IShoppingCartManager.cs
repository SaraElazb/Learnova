using BusinessLogicLayer.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.ShoppingCartManager
{
    public interface IShoppingCartManager
    {
        void AddToCart(CourseDTO course);
        void RemoveFromCart(int courseId);
        decimal GetTotal();


    }
}
