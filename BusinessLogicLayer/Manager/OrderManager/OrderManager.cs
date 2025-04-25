//using AutoMapper;
//using BusinessLogicLayer.DTOs.OrderDtos;
//using BusinessLogicLayer.DTOs.ShoppingCartDtos;
//using DataAccessLayer.Entities;
//using DataAccessLayer.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLogicLayer.Manager.OrderManager
//{
//    public class OrderManager : IOrderManager
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;

//        public OrderManager(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }

//        public async Task<int> CreateOrderAsync(string userId, ShoppingCartDto cart)
//        {
//            var order = new Order
//            {
//                UserId = userId,
//                TotalAmount = cart.Items.Sum(i => i.Price),
//                Status = "Pending",
//                CreatedAt = DateTime.Now,
//                Items = cart.Items.Select(i => new OrderItem
//                {
//                    CourseId = i.Course_ID,
//                    CourseTitle = i.Title,
//                    Price = i.Price
//                }).ToList()
//            };

//            await _unitOfWork.Orders.AddAsync(order);
//            await _unitOfWork.SaveChangesAsync();

//            return order.OrderId;
//        }

//        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
//        {
//            var order = await _unitOfWork.Orders.FindAsync(o => o.OrderId == orderId,
//                q => q.Include(o => o.Items));
//            return _mapper.Map<OrderDto>(order);
//        }
//    }
//}
