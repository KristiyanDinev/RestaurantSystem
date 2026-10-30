using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace RestaurantSystem.Tests;

public class BusinessLogicTests
{
    [Fact]
    public void CouponService_CalculatesDiscountPrecisely()
    {
        using ServiceProvider provider = CreateProvider();
        CouponService service = provider.GetRequiredService<CouponService>();

        Assert.Equal(75m, service.HandleCouponDiscount(25, 100m));
        Assert.Equal(0m, service.HandleCouponDiscount(100, 100m));
    }

    [Fact]
    public async Task CouponService_CreatesEditsAndDeletesCoupons()
    {
        await using ServiceProvider provider = CreateProvider();
        CouponService service = provider.GetRequiredService<CouponService>();
        CouponCreateFormModel model = new()
        {
            Name = "Summer",
            CouponCode = "SUMMER",
            DiscountPercent = 10,
            ExpDate = new DateOnly(2030, 7, 1)
        };

        Assert.True(await service.CreateCouponAsync(model));
        Assert.NotNull(await service.GetCouponByCodeAsync("SUMMER"));

        model.DiscountPercent = 20;
        Assert.True(await service.EditCouponAsync(model));
        Assert.Equal(20, (await service.GetCouponByCodeAsync("SUMMER"))!.DiscountPercent);

        Assert.True(await service.DeleteCouponAsync("SUMMER"));
        Assert.Null(await service.GetCouponByCodeAsync("SUMMER"));
        Assert.False(await service.DeleteCouponAsync("MISSING"));
    }

    [Fact]
    public async Task OrderService_OnlyAllowsValidStatusTransitions()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        OrderService service = provider.GetRequiredService<OrderService>();
        OrderModel order = new()
        {
            UserId = 1,
            RestaurantId = 1,
            CurrentStatus = OrderStatusEnum.Pending,
            TotalPrice = 20
        };
        database.Orders.Add(order);
        await database.SaveChangesAsync();

        Assert.True(await service.UpdateOrderCurrentStatusByIdAsync(order.Id, OrderStatusEnum.Preparing));
        Assert.False(await service.UpdateOrderCurrentStatusByIdAsync(order.Id, OrderStatusEnum.Delivered));
        Assert.Equal(OrderStatusEnum.Preparing,
            (await provider.GetRequiredService<DatabaseContext>().Orders
                .SingleAsync(item => item.Id == order.Id)).CurrentStatus);
    }

    [Fact]
    public async Task OrderService_AddOrderPersistsItemsAndNormalizesNotes()
    {
        await using ServiceProvider provider = CreateProvider();
        OrderService service = provider.GetRequiredService<OrderService>();

        OrderModel order = (await service.AddOrderAsync(
            userId: 5,
            restaurantId: 2,
            dishesId: [10, 10, 11],
            notes: "   ",
            totalPrice: 12.345m,
            tableNumber: null,
            coupon: "SAVE",
            address_id: 8))!;

        Assert.Equal(12.35m, order.TotalPrice);
        Assert.Null(order.Notes);
        Assert.Equal(OrderStatusEnum.Pending, order.CurrentStatus);
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        Assert.Equal(3, await database.OrderedDishes.CountAsync(item => item.OrderId == order.Id));
        Assert.Equal("SAVE", order.CouponCode);
        Assert.Equal(8, order.UserAddressId);
    }

    [Fact]
    public async Task OrderedDishesService_UpdatesMatchingRowsAndReportsStatuses()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        OrderedDishesService service = provider.GetRequiredService<OrderedDishesService>();
        database.OrderedDishes.AddRange(
            new OrderedDishesModel
            {
                OrderId = 4, DishId = 9, CurrentStatus = DishStatusEnum.Pending
            },
            new OrderedDishesModel
            {
                OrderId = 4, DishId = 9, CurrentStatus = DishStatusEnum.Pending
            });
        await database.SaveChangesAsync();

        Assert.True(await service.UpdateOrderedDishStatusByIdAsync(
            9, 4, DishStatusEnum.Ready));
        Assert.Equal([DishStatusEnum.Ready, DishStatusEnum.Ready],
            await service.GetDishCurrectStatusAsync(4));
        Assert.False(await service.UpdateOrderedDishStatusByIdAsync(
            100, 4, DishStatusEnum.Ready));
    }

    [Fact]
    public void DishService_ParsesCartIdsAndIgnoresMalformedValues()
    {
        using ServiceProvider provider = CreateProvider();
        DishService service = provider.GetRequiredService<DishService>();
        DefaultHttpContext context = new();
        context.Request.Headers.Cookie = "cart_items=1-x--3";

        Assert.Equal([1, 3], service.GetDishIDsFromCartAsync(context));
    }

    [Fact]
    public async Task DeliveryService_OnlyAssignsReadyOnlineOrders()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        DeliveryService service = provider.GetRequiredService<DeliveryService>();
        database.Orders.AddRange(
            new OrderModel
            {
                Id = 1, UserId = 1, RestaurantId = 1,
                CurrentStatus = OrderStatusEnum.Ready, TotalPrice = 10
            },
            new OrderModel
            {
                Id = 2, UserId = 1, RestaurantId = 1,
                CurrentStatus = OrderStatusEnum.Pending, TotalPrice = 10
            },
            new OrderModel
            {
                Id = 3, UserId = 1, RestaurantId = 1,
                CurrentStatus = OrderStatusEnum.Ready, TableNumber = "A1", TotalPrice = 10
            });
        await database.SaveChangesAsync();

        Assert.True(await service.AddDeliveryAsync(9, 1));
        Assert.False(await service.AddDeliveryAsync(9, 2));
        Assert.False(await service.AddDeliveryAsync(9, 3));
        Assert.True(await service.RemoveDeliveryAsync(9, 1));
        Assert.False(await service.RemoveDeliveryAsync(9, 1));
    }

    [Fact]
    public async Task RestaurantService_ReservationValidationHonorsCapacityAndServiceMode()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        RestaurantService service = provider.GetRequiredService<RestaurantService>();
        database.Restaurants.Add(new RestaurantModel
        {
            Id = 1,
            Address = "Main Street",
            Country = "Bulgaria",
            PostalCode = "1000",
            ServeCustomersInPlace = true,
            ReservationMinAdults = 1,
            ReservationMaxAdults = 4,
            ReservationMinChildren = 0,
            ReservationMaxChildren = 2
        });
        await database.SaveChangesAsync();

        Assert.True(await service.CheckForReservationAsync(new ReservationModel
        {
            RestaurantId = 1, UserId = 1, PhoneNumber = "123",
            Amount_Of_Adults = 2, Amount_Of_Children = 1,
            CurrentStatus = ReservationStatusEnum.Pending
        }));
        Assert.False(await service.CheckForReservationAsync(new ReservationModel
        {
            RestaurantId = 1, UserId = 1, PhoneNumber = "123",
            Amount_Of_Adults = 5, Amount_Of_Children = 1,
            CurrentStatus = ReservationStatusEnum.Pending
        }));
    }

    [Fact]
    public async Task ReservationService_CreateAndUpdateEnforcesUserScope()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        ReservationService service = provider.GetRequiredService<ReservationService>();
        database.Restaurants.Add(new RestaurantModel
        {
            Id = 1, Address = "Main Street", Country = "Bulgaria",
            PostalCode = "1000", ServeCustomersInPlace = true,
            ReservationMinAdults = 1, ReservationMaxAdults = 4,
            ReservationMinChildren = 0, ReservationMaxChildren = 2
        });
        await database.SaveChangesAsync();

        ReservationModel reservation = (await service.CreateReservationAsync(3, 1,
            new ReservationFormModel
            {
                Amount_Of_Adults = 2,
                Amount_Of_Children = 0,
                PhoneNumber = "123",
                At_Date = DateTime.UtcNow.AddDays(1)
            }))!;

        Assert.NotNull(reservation);
        Assert.False(await service.UpdateReservationAsync(reservation.Id,
            ReservationStatusEnum.Cancelled, userId: 4));
        Assert.True(await service.UpdateReservationAsync(reservation.Id,
            ReservationStatusEnum.Cancelled, userId: 3));
    }

    private static ServiceProvider CreateProvider()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddDbContext<DatabaseContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<CouponService>();
        services.AddScoped<RestaurantService>();
        services.AddScoped<ReservationService>();
        services.AddScoped<AddressService>();
        services.AddScoped<DeliveryService>();
        services.AddScoped<OrderedDishesService>();
        services.AddScoped<OrderService>();
        services.AddScoped<DishService>();
        return services.BuildServiceProvider();
    }
}
