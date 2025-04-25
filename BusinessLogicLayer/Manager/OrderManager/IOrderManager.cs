using BusinessLogicLayer.DTOs.OrderDtos;
using BusinessLogicLayer.DTOs.ShoppingCartDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.OrderManager
{
    public interface IOrderManager
    {
        Task<int> CreateOrderAsync(string userId, ShoppingCartDto cart);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
    }
}
