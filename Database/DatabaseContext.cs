using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using System.Reflection.Emit;

namespace RestaurantSystem.Database {
    public class DatabaseContext : DbContext {

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CuponModel> Cupons { get; set; }
        public DbSet<DishModel> Dishies { get; set; }
        public DbSet<OrderedDishesModel> OrderedDishes { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<RolePermissionModel> RolePermissions { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
        public DbSet<RestaurantModel> Restaurants { get; set; }
        public DbSet<DeliveryModel> Delivery { get; set; }
        public DbSet<AddressModel> Addresses { get; set; }
        public DbSet<OrderServerMappingModel> OrderServerMappings { get; set; }


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresEnum<OrderStatusEnum>();
            builder.HasPostgresEnum<DishStatusEnum>();
            builder.HasPostgresEnum<ReservationStatusEnum>();
            builder.HasPostgresEnum<DishTypeEnum>();

            // UserModel
            BuildUserModel(ref builder);

            // ServiceModel
            BuildServicesModel(ref builder);

            // UserRoleModel
            BuildUserRoleModel(ref builder);

            // RestaurantModel
            BuildRestaurantModel(ref builder);

            // DishModel
            BuildDishModel(ref builder);

            // OrderModel
            BuildOrderModel(ref builder);

            // OrderedDishes
            BuildOrderedDishes(ref builder);

            // ReservationModel
            BuildReservationModel(ref builder);

            // CuponModel
            BuildCuponModel(ref builder);

            // RoleModel
            BuildRoleModel(ref builder);

            // RolePermissionModel
            BuildRolePermissionModel(ref builder);

            // DeliveryModel
            BuildDeliveryModel(ref builder);

            // AddressModel
            BuildAddressModel(ref builder);

            // OrderServerMappingModel
            BuildOrderServerMappingModel(ref builder);
        }


        private void BuildOrderServerMappingModel(ref ModelBuilder builder)
        {
            builder.Entity<OrderServerMappingModel>().ToTable("Order_Server_Mappings");
            builder.Entity<OrderServerMappingModel>()
                .HasKey(mapping => mapping.Id);

            builder.Entity<OrderServerMappingModel>()
                .Property(mapping => mapping.OrderId)
                .IsRequired();

            builder.Entity<OrderServerMappingModel>()
                .Property(mapping => mapping.ServerId)
                .IsRequired();
        }

        private void BuildUserModel(ref ModelBuilder builder)
        {
            builder.Entity<UserModel>().ToTable("Users");

            builder.Entity<UserModel>()
                .HasKey(user => user.Id);

            builder.Entity<UserModel>()
                .Property(user => user.Name)
                .IsUnicode()
                .IsRequired();

            builder.Entity<UserModel>()
                .HasMany(user => user.Addresses)
                .WithOne(address => address.User)
                .HasForeignKey(address => address.UserId)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.Email)
                .IsRequired();

            builder.Entity<UserModel>()
                .HasIndex(user => user.Email)
                .IsUnique();

            builder.Entity<UserModel>()
                .Property(user => user.Image)
                .HasDefaultValue(null);


            builder.Entity<UserModel>()
                .Property(user => user.RestaurantId)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Entity<UserModel>()
                .Property(user => user.LastTimeLogedIn)
                .IsRequired()
                .HasDefaultValue(DateOnly.FromDateTime(DateTime.Now));

            builder.Entity<UserModel>()
                .HasOne(user => user.Restaurant)
                .WithMany(restaurat => restaurat.Employees)
                .HasForeignKey(user => user.RestaurantId);
        }

        private void BuildServicesModel(ref ModelBuilder builder) {
            builder.Entity<ServiceModel>().ToTable("Services");

            builder.Entity<ServiceModel>()
                .HasKey(service => service.Path);

            builder.Entity<ServiceModel>()
                .Property(service => service.Description)
                .HasDefaultValue(null);
        }

        private void BuildRolePermissionModel(ref ModelBuilder builder)
        {
            builder.Entity<RolePermissionModel>().ToTable("Role_Permissions");

            builder.Entity<RolePermissionModel>()
                .HasKey(role => role.Id);

            builder.Entity<RolePermissionModel>()
                .HasOne(role => role.Role)
                .WithMany(role => role.RolePermissions)
                .HasForeignKey(role => role.RoleName);

            builder.Entity<RolePermissionModel>()
                .HasOne(role => role.Service)
                .WithMany(role => role.RolePermissions)
                .HasForeignKey(role => role.ServicePath);
        }

        private void BuildUserRoleModel(ref ModelBuilder builder)
        {
            builder.Entity<UserRoleModel>().ToTable("User_Roles");

            builder.Entity<UserRoleModel>()
                .HasKey(role => role.Id);

            builder.Entity<UserRoleModel>()
                .Property(user => user.UserId)
                .IsRequired();

            builder.Entity<UserRoleModel>()
                .Property(user => user.RoleName)
                .IsRequired();

            builder.Entity<UserRoleModel>()
                .HasOne(user => user.User)
                .WithMany(user => user.Roles)
                .HasForeignKey(user => user.UserId);

            builder.Entity<UserRoleModel>()
                .HasOne(role => role.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(role => role.RoleName);
        }

        private void BuildRoleModel(ref ModelBuilder builder)
        {
            builder.Entity<RoleModel>().ToTable("Roles");

            builder.Entity<RoleModel>()
                .HasKey(role => role.Name);

            builder.Entity<RoleModel>()
                .Property(role => role.Description)
                .HasDefaultValue(null);
        }
        
        private void BuildRestaurantModel(ref ModelBuilder builder)
        {
            builder.Entity<RestaurantModel>().ToTable("Restaurants");

            builder.Entity<RestaurantModel>()
                .HasKey(restaurant => restaurant.Id);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.Address)
                .IsRequired()
                .IsUnicode();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.PostalCode)
                .IsRequired()
                .IsUnicode();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.Country)
                .IsRequired()
                .IsUnicode();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.State)
                .HasDefaultValue(null)
                .IsUnicode();

            builder.Entity<RestaurantModel>()
               .Property(restaurant => restaurant.City)
               .HasDefaultValue(null)
               .IsUnicode();

            builder.Entity<RestaurantModel>()
               .Property(restaurant => restaurant.PhoneNumber)
               .HasDefaultValue(null)
               .IsUnicode();

            builder.Entity<RestaurantModel>()
               .Property(restaurant => restaurant.Email)
               .HasDefaultValue(null);


            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMaxAdults)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMinAdults)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMaxChildren)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMinChildren)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.DoDelivery)
                .HasDefaultValue(true);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ServeCustomersInPlace)
                .HasDefaultValue(true);
        }

        private void BuildDishModel(ref ModelBuilder builder) {
            builder.Entity<DishModel>().ToTable("Dishes");

            builder.Entity<DishModel>()
                .HasKey(dish => dish.Id);

            builder.Entity<DishModel>()
                .Property(dish => dish.Name)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.Price)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.Ingredients)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.Grams)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.Type_Of_Dish)
                .HasConversion<string>()
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.RestaurantId)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.AverageTimeToCook)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.IsAvailable)
                .HasDefaultValue(true);

            builder.Entity<DishModel>()
                .Property(dish => dish.Image)
                .HasDefaultValue(null);
        }

        private void BuildOrderModel(ref ModelBuilder builder) {
            builder.Entity<OrderModel>().ToTable("Orders");

            builder.Entity<OrderModel>()
                .HasKey(order => order.Id);

            builder.Entity<OrderModel>()
                .Property(order => order.CurrentStatus)
                .HasConversion<string>()
                .HasDefaultValue(OrderStatusEnum.Pending)
                .IsRequired();

            builder.Entity<OrderModel>()
                .Property(order => order.TableNumber)
                .HasDefaultValue(null);

            builder.Entity<OrderModel>()
                .Property(order => order.Notes)
                .HasDefaultValue(null);

            builder.Entity<OrderModel>()
                .Property(order => order.OrderedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Entity<OrderModel>()
                .Property(order => order.TotalPrice)
                .IsRequired();

            builder.Entity<OrderModel>()
                .Property(order => order.UserId)
                .IsRequired();

            builder.Entity<OrderModel>()
                .Property(order => order.RestaurantId)
                .IsRequired();

            builder.Entity<OrderModel>()
                .Property(order => order.UserAddressId)
                .HasDefaultValue(null);

            builder.Entity<OrderModel>()
                .HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId);

            builder.Entity<OrderModel>()
                .HasOne(order => order.Restaurant)
                .WithMany(restauratn => restauratn.Orders)
                .HasForeignKey(order => order.RestaurantId);

            builder.Entity<OrderModel>()
                .HasOne(order => order.UserAddress)
                .WithMany(address => address.Orders)
                .HasForeignKey(order => order.UserAddressId);
        }

        private void BuildOrderedDishes(ref ModelBuilder builder) {
            builder.Entity<OrderedDishesModel>().ToTable("Ordered_Dishes");

            builder.Entity<OrderedDishesModel>()
                .HasKey(order => order.Id);

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.CurrentStatus)
                .HasConversion<string>()
                .HasDefaultValue(DishStatusEnum.Pending)
                .IsRequired();

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.OrderId)
                .IsRequired();

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.DishId)
                .IsRequired();

            builder.Entity<OrderedDishesModel>()
                .HasOne(order => order.Order)
                .WithMany(order => order.OrderedDishes)
                .HasForeignKey(order => order.OrderId);

            builder.Entity<OrderedDishesModel>()
                .HasOne(order => order.Dish)
                .WithMany(dish => dish.OrderedDishes)
                .HasForeignKey(order => order.DishId);
        }

        private void BuildReservationModel(ref ModelBuilder builder)
        {
            builder.Entity<ReservationModel>().ToTable("Reservations");

            builder.Entity<ReservationModel>()
                .HasKey(res => res.Id);

            builder.Entity<ReservationModel>()
                .Property(order => order.Notes)
                .HasDefaultValue(null);

            builder.Entity<ReservationModel>()
                .Property(order => order.CurrentStatus)
                .HasConversion<string>()
                .HasDefaultValue(ReservationStatusEnum.Pending)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.UserId)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.PhoneNumber)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.TotalPrice)
                .HasDefaultValue(0);

            builder.Entity<ReservationModel>()
                .Property(order => order.RestaurantId)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Amount_Of_Children)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.At_Date)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Amount_Of_Adults)
                .HasDefaultValue(1)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Created_At)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Entity<ReservationModel>()
                .HasOne(reservation => reservation.User)
                .WithMany(user => user.Reservations)
                .HasForeignKey(reservation => reservation.UserId);

            builder.Entity<ReservationModel>()
                .HasOne(reservation => reservation.Restaurant)
                .WithMany(restaurant => restaurant.Reservations)
                .HasForeignKey(reservation => reservation.RestaurantId);
        }

        private void BuildCuponModel(ref ModelBuilder builder) {
            builder.Entity<CuponModel>().ToTable("Cupons");

            builder.Entity<CuponModel>()
                .HasKey(cupon => cupon.CuponCode);

            builder.Entity<CuponModel>()
                .Property(cupon => cupon.Name)
                .IsRequired();

            builder.Entity<CuponModel>()
                .Property(cupon => cupon.CuponCode)
                .IsRequired();

            builder.Entity<CuponModel>()
                .Property(cupon => cupon.DiscountPercent)
                .IsRequired();

            builder.Entity<CuponModel>()
                .Property(cupon => cupon.ExpirationDate)
                .IsRequired();
        }

        private void BuildDeliveryModel(ref ModelBuilder builder) {
            builder.Entity<DeliveryModel>().ToTable("Delivery");

            builder.Entity<DeliveryModel>()
                .HasKey(delivery => delivery.Id);

            builder.Entity<DeliveryModel>()
                .Property(delivery => delivery.UserId)
                .IsRequired();

            builder.Entity<DeliveryModel>()
                .Property(delivery => delivery.OrderId)
                .IsRequired();

            builder.Entity<DeliveryModel>()
                .HasIndex(delivery => delivery.UserId)
                .IsUnique();

            builder.Entity<DeliveryModel>()
                .HasIndex(delivery => delivery.OrderId)
                .IsUnique();

            builder.Entity<DeliveryModel>()
                .HasOne(delivery => delivery.User)
                .WithOne(user => user.Delivery)
                .HasForeignKey<DeliveryModel>(
                    delivery => delivery.UserId);

            builder.Entity<DeliveryModel>()
                .HasOne(delivery => delivery.Order)
                .WithOne(order => order.Delivery)
                .HasForeignKey<DeliveryModel>(
                    delivery => delivery.OrderId);
        }

        private void BuildAddressModel(ref ModelBuilder builder) {
            builder.Entity<AddressModel>().ToTable("Addresses");

            builder.Entity<AddressModel>()
                  .HasKey(address => address.Id);

            builder.Entity<AddressModel>()
                .Property(address => address.Country)
                .IsRequired()
                .IsUnicode();

            builder.Entity<AddressModel>()
                .Property(address => address.State)
                .HasDefaultValue(null)
                .IsUnicode();

            builder.Entity<AddressModel>()
                .Property(address => address.City)
                .HasDefaultValue(null)
                .IsUnicode();

            builder.Entity<AddressModel>()
                .Property(address => address.Address)
                .IsRequired()
                .IsUnicode();

            builder.Entity<AddressModel>()
                .Property(address => address.PhoneNumber)
                .IsRequired();

            builder.Entity<AddressModel>()
                .Property(address => address.PostalCode)
                .IsUnicode()
                .IsRequired();

            builder.Entity<AddressModel>()
                .Property(address => address.Notes)
                .HasDefaultValue(null);
        }
    }
}
