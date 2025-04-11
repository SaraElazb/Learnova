using AutoMapper;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BusinessLogicLayer.DTOs.ShoppingCartDtos;
using BusinessLogicLayer.DTOs.CourseDtos;

namespace BusinessLogicLayer.Manager.ShoppingCartManager
{
    public class ShoppingCartManager : IShoppingCartManager
    {
        private ShoppingCartDto _cart;

        public ShoppingCartManager(ShoppingCartDto cart)
        {
            _cart = cart;
        }

        public void AddToCart(CourseDTO course)
        {
            if (!_cart.Items.Any(i => i.Course_ID == course.Course_ID))
            {
                _cart.Items.Add(course);
            }
        }

        public void RemoveFromCart(int courseId)
        {
            var item = _cart.Items.FirstOrDefault(i => i.Course_ID == courseId);
            if (item != null)
            {
                _cart.Items.Remove(item);
            }
        }

        public decimal GetTotal()
        {
            return _cart.Items.Sum(i => i.Price);
        }
    }
}
