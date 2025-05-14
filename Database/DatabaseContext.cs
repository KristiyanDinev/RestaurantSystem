using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models.DatabaseModels;

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
        public DbSet<TimeTableModel> TimeTables { get; set; }

        public readonly string DefaultOrder_CurrentStatus = "pending";
        public readonly string DefaultOrderedDish_CurrentStatus = "pending";
        public readonly string DefaultReservation_CurrentStatus = "pending";

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // UserModel
            BuildUserModel(ref builder);

            // ServiceModel
            BuildServicesModel(ref builder);

            // UserRoleModel
            BuildUserRoleModel(ref builder);

            // RestaurantModel
            BuildRestaurantModel(ref builder);

            // TimeTableModel
            BuildTimeTableModel(ref builder);

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

            base.OnModelCreating(builder);
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
                .Property(user => user.PostalCode)
                .IsUnicode()
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.PostalCode)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.Address)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.City)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.Country)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.PhoneNumber)
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
                .Property(user => user.State)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.Notes)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.RestaurantId)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

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
                .HasKey(rp => rp.RoleName);

            builder.Entity<RolePermissionModel>()
                .HasKey(rp =>  rp.ServicePath);

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
                .HasKey(role => role.UserId);

            builder.Entity<UserRoleModel>()
                .HasKey(role => role.RoleName);

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
                .Property(restaurant => restaurant.City)
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
                .Property(restaurant => restaurant.Price_Per_Children)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.Price_Per_Adult)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.State)
                .HasDefaultValue(null);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.DoDelivery)
                .HasDefaultValue(true);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ServeCustomersInPlace)
                .HasDefaultValue(true);
        }

        private void BuildTimeTableModel(ref ModelBuilder builder)
        {
            builder.Entity<TimeTableModel>().ToTable("TimeTable");

            builder.Entity<TimeTableModel>()
                .HasKey(time => time.RestuarantId);

            builder.Entity<TimeTableModel>()
                .Property(time => time.UserAddress)
                .IsRequired();

            builder.Entity<TimeTableModel>()
                .Property(time => time.UserCity)
                .IsRequired();

            builder.Entity<TimeTableModel>()
                .Property(time => time.UserCountry)
                .IsRequired();

            builder.Entity<TimeTableModel>()
                .Property(time => time.AvrageDeliverTime)
                .IsRequired();

            builder.Entity<TimeTableModel>()
                .Property(time => time.RestuarantId)
                .IsRequired();

            builder.Entity<TimeTableModel>()
                .Property(time => time.UserState)
                .HasDefaultValue(null);

            builder.Entity<TimeTableModel>()
                .HasOne(time => time.Restuarant)
                .WithMany(restaurant => restaurant.TimeTables)
                .HasForeignKey(time => time.RestuarantId);
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
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.RestaurantId)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.AvrageTimeToCook)
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
                .HasDefaultValue(DefaultOrder_CurrentStatus)
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
                .HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId);

            builder.Entity<OrderModel>()
                .HasOne(order => order.Restaurant)
                .WithMany(restauratn => restauratn.Orders)
                .HasForeignKey(order => order.RestaurantId);
        }

        private void BuildOrderedDishes(ref ModelBuilder builder) {
            builder.Entity<OrderedDishesModel>().ToTable("Ordered_Dishes");

            builder.Entity<OrderedDishesModel>()
                .HasKey(order => order.Id);

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.Notes)
                .HasDefaultValue(null);

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.CurrentStatus)
                .HasDefaultValue(DefaultOrderedDish_CurrentStatus)
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
                .HasDefaultValue(DefaultReservation_CurrentStatus)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.UserId)
                .IsRequired();

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
    }
}
