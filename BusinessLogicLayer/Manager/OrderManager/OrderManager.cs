using AutoMapper;
using BusinessLogicLayer.DTOs.OrderDtos;
using BusinessLogicLayer.DTOs.ShoppingCartDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.OrderManager
{
    public class OrderManager : IOrderManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateOrderAsync(string userId, ShoppingCartDto cart)
        {
            var order = new DataAccessLayer.Entities.Order
            {
                UserId = userId,
                TotalAmount = cart.Items.Sum(i => i.Price),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Items = cart.Items.Select(i => new DataAccessLayer.Entities.OrderItem
                {
                    CourseId = i.Course_ID,
                    CourseTitle = i.Title,
                    Price = i.Price
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return order.OrderId;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.FindAsync(
                o => o.OrderId == orderId,
                q => q.Include(o => o.Items));
            
            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _unitOfWork.Orders.FindAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.FindAllAsync(
                o => o.UserId == userId,
                q => q.Include(o => o.Items));
            
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
