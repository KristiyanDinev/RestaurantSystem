using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using System.Net.WebSockets;
using Xunit;

namespace RestaurantSystem.Tests;

public class IdentityTests
{
    [Theory]
    [InlineData("Bulgaria", null, true)]
    [InlineData(" ", null, false)]
    [InlineData("Bulgaria", " ", false)]
    public void LocationSelection_ValidatesRequiredValues(
        string? country, string? state, bool expected)
    {
        Assert.Equal(expected, LocationService.IsValidSelection(country, state));
    }

    [Fact]
    public async Task UserService_UsesIdentityPasswordHasherForRegistrationAndLogin()
    {
        await using ServiceProvider provider = CreateProvider();
        UserService service = provider.GetRequiredService<UserService>();

        (UserModel? registered, IdentityResult registrationResult) =
            await service.RegisterUserAsync(new RegisterFormModel
        {
            Name = "Test User",
            Email = "user@example.com",
            Password = "CorrectPassword1!"
        });

        Assert.True(registrationResult.Succeeded);
        Assert.NotNull(registered);
        Assert.NotEqual("CorrectPassword1!", registered!.PasswordHash);
        Assert.NotNull(await service.LoginUserAsync("user@example.com", "CorrectPassword1!"));
        Assert.Null(await service.LoginUserAsync("user@example.com", "wrong-password"));
    }

    [Fact]
    public async Task UserService_ResetsPasswordThroughIdentity()
    {
        await using ServiceProvider provider = CreateProvider();
        UserService service = provider.GetRequiredService<UserService>();
        UserModel user = (await service.RegisterUserAsync(new RegisterFormModel
        {
            Name = "Test User",
            Email = "reset@example.com",
            Password = "OldPassword1!"
        })).User!;

        Assert.True(await service.UpdateUserPasswordAsync(user.Email!, "NewPassword1!"));
        Assert.Null(await service.LoginUserAsync(user.Email!, "OldPassword1!"));
        Assert.NotNull(await service.LoginUserAsync(user.Email!, "NewPassword1!"));
    }

    [Fact]
    public async Task RoleService_UsesIdentityRolesForServicePermissions()
    {
        await using ServiceProvider provider = CreateProvider();
        UserService userService = provider.GetRequiredService<UserService>();
        RoleService roleService = provider.GetRequiredService<RoleService>();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        UserModel user = (await userService.RegisterUserAsync(new RegisterFormModel
        {
            Name = "Staff User",
            Email = "staff@example.com",
            Password = "Password1!"
        })).User!;

        database.Services.Add(new ServiceModel { Path = "/staff/cook" });
        database.RolePermissions.Add(new RolePermissionModel
        {
            RoleName = RoleEnum.cook.ToString(),
            ServicePath = "/staff/cook"
        });
        await database.SaveChangesAsync();

        Assert.False(await roleService.CanUserAccessServiceAsync(user.Id, "/staff/cook"));
        Assert.True(await roleService.AssignRoleToUserAsync(user.Id, RoleEnum.cook));
        Assert.True(await roleService.CanUserAccessServiceAsync(user.Id, "/staff/cook/orders"));
    }

    [Fact]
    public async Task OrderService_DoesNotAllowDeletingAnotherUsersOrder()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        OrderService service = provider.GetRequiredService<OrderService>();
        database.Orders.Add(new OrderModel
        {
            Id = 42,
            UserId = 2,
            RestaurantId = 1,
            CurrentStatus = OrderStatusEnum.Pending,
            TotalPrice = 10
        });
        await database.SaveChangesAsync();

        Assert.False(await service.DeleteOrderAsync(42, userId: 1));
        Assert.True(await service.DeleteOrderAsync(42, userId: 2));
    }

    [Fact]
    public async Task AddressService_DoesNotAllowUpdatingAnotherUsersAddress()
    {
        await using ServiceProvider provider = CreateProvider();
        DatabaseContext database = provider.GetRequiredService<DatabaseContext>();
        AddressService service = provider.GetRequiredService<AddressService>();
        database.Addresses.Add(new AddressModel
        {
            Id = 42,
            UserId = 2,
            Country = "Bulgaria",
            Address = "Main Street",
            PhoneNumber = "123",
            PostalCode = "1000"
        });
        await database.SaveChangesAsync();

        Assert.Null(await service.GetAddressByIdAsync(42, 1));
        Assert.NotNull(await service.GetAddressByIdAsync(42, 2));
    }

    [Fact]
    public void WebSocketUtility_ReplacesSubscriptionsAndRemovesDisconnectedSockets()
    {
        WebSocketUtility utility = new();
        using ClientWebSocket socket = new();

        utility.AddOrdersToListenTo([1, 2, 2], socket);
        Assert.Single(utility.GetListenersForOrderId(1));
        Assert.Equal([1, 2], utility.GetOrderIdsForSocket(socket));

        utility.AddOrdersToListenTo([3], socket);
        Assert.Empty(utility.GetListenersForOrderId(1));
        Assert.Equal([3], utility.GetOrderIdsForSocket(socket));

        utility.RemoveOrderIdsFromSocket(socket);
        Assert.Empty(utility.GetOrderWebSocketModels());
    }

    private static ServiceProvider CreateProvider()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContext<DatabaseContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentityCore<UserModel>()
            .AddRoles<RoleModel>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<UserService>();
        services.AddScoped<RoleService>();
        services.AddScoped<OrderedDishesService>();
        services.AddScoped<OrderService>();
        services.AddScoped<AddressService>();
        return services.BuildServiceProvider();
    }
}
