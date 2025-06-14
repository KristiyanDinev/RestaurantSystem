﻿using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class OrderService
    {

        private DatabaseContext _databaseContext;
        private OrderedDishesService _orderedDishesDatabaseHandler;

        public OrderService(DatabaseContext databaseContext, 
            OrderedDishesService orderedDishesDatabaseHandler,
            WebSocketUtility webSocketUtility)
        {
            _databaseContext = databaseContext;
            _orderedDishesDatabaseHandler = orderedDishesDatabaseHandler;
        }

        public async Task<OrderModel?> AddOrderAsync(int userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice,
            string? tableNumber, string? cupon_code)
        {
            // total price here is with applied discount if the code is correct.
            OrderModel order = new OrderModel {
                Notes = notes != null && notes.Replace(" ", "").Length == 0 ? null : notes,
                RestaurantId = restaurantId,
                CurrentStatus = _databaseContext.DefaultOrder_CurrentStatus,
                TotalPrice = decimal.Parse($"{totalPrice:F2}"),
                UserId = userId,
                TableNumber = tableNumber
            };

            await _databaseContext.Orders.AddAsync(order);

            if (await _databaseContext.SaveChangesAsync() <= 0) {
                return null;
            }

            foreach (int id in dishesId)
            {
                await _orderedDishesDatabaseHandler.CreateOrderedDishAsync(id, order.Id, null);
            }

            if (await _databaseContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            return order;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId);

            if (order == null || !order.CurrentStatus.Equals(
                _databaseContext.DefaultOrder_CurrentStatus))
            {
                return false;
            }

            await _orderedDishesDatabaseHandler.DeleteOrderedDishesAsync(orderId);

            _databaseContext.Orders.Remove(order);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderCurrentStatusByIdAsync(int orderId, string status)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId) ?? throw new Exception();

            order.CurrentStatus = status;

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<OrderModel>> GetOrdersByUserAsync(int userId)
        {
            return await _databaseContext.Orders
                .Include(order => order.Restaurant)
                .Include(order => order.OrderedDishes)
                .Where(
                order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetOrdersByRestaurantIdAsync(int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => order.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> Get_HomeDelivery_OrdersBy_RestaurantIdAsync(
            int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => 
                order.RestaurantId == restaurantId &&
                order.TableNumber == null)
                .ToListAsync();
        }

        public async Task<OrderModel?> GetOrderByIdAsync(int orderId)
        {
            return await _databaseContext.Orders
                .Include(order => order.OrderedDishes)
                .Include(order => order.Restaurant)
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

    }
}
