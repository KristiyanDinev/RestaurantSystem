using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Database {
    public class DatabaseManager : DbContext {

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CuponModel> Cupons { get; set; }
        public DbSet<DishModel> Dishies { get; set; }
        public DbSet<OrderedDishesModel> OrderedDishes { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
        public DbSet<RestaurantModel> Restaurants { get; set; }
        public DbSet<TimeTableModel> TimeTables { get; set; }

        public DatabaseManager(DbContextOptions<DatabaseManager> options) : base(options) {}

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
        }

        private void BuildUserModel(ref ModelBuilder builder)
        {
            builder.Entity<UserModel>()
                .HasKey(user => user.Id);

            builder.Entity<UserModel>()
                .Property(user => user.Name)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.Email)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.Image)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.Address)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.City)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.State)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.Country)
                .IsRequired();

            builder.Entity<UserModel>()
                .Property(user => user.PhoneNumber)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.Notes)
                .HasDefaultValue(null);

            builder.Entity<UserModel>()
                .Property(user => user.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Entity<UserModel>()
                .HasMany(user => user.Roles)
                .WithOne(userRole => userRole.UserModel)
                .HasForeignKey(userRole => userRole.UserModelId)
                .IsRequired();
        }

        private void BuildServicesModel(ref ModelBuilder builder) {
            // URL path = Service
            // UserRoleModel = User's role whether they can access it or not.
            builder.Entity<ServiceModel>()
                .HasKey(service => service.Service);
        }

        private void BuildUserRoleModel(ref ModelBuilder builder)
        {
            // URL path = Service
            // UserRoleModel = User's role whether they can access it or not.
            builder.Entity<UserRoleModel>()
                .HasKey(role => new { role.UserModelId, role.Role });

            builder.Entity<UserRoleModel>()
                .Property(user => user.Role)
                .IsRequired();

            builder.Entity<UserRoleModel>()
                .Property(user => user.UserModelId)
                .IsRequired();

            builder.Entity<UserRoleModel>()
                .HasMany(userRole => userRole.Services)
                .WithMany(service => service.AllowedRoles)
                .UsingEntity(j => j.ToTable("RoleServiceAccess"));
        }
        
        private void BuildRestaurantModel(ref ModelBuilder builder)
        {
            builder.Entity<RestaurantModel>()
                .HasKey(restaurant => restaurant.Id);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.Address)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.City)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.State)
                .HasDefaultValue(null);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.Country)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.DoDelivery)
                .HasDefaultValue(true);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ServeCustomersInPlace)
                .HasDefaultValue(true);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMaxAdults)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMinAdults)
                .HasDefaultValue(1);

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMaxChildren)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMinChildren)
                .IsRequired();

            builder.Entity<RestaurantModel>()
                .Property(restaurant => restaurant.ReservationMinChildren)
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
        }

        private void BuildTimeTableModel(ref ModelBuilder bulder)
        {
            bulder.Entity<TimeTableModel>()
                .HasKey(time => time.RestuarantModelId);

            bulder.Entity<TimeTableModel>()
                .Property(time => time.UserAddress)
                .IsRequired();

            bulder.Entity<TimeTableModel>()
                .Property(time => time.UserCity)
                .IsRequired();

            bulder.Entity<TimeTableModel>()
                .Property(time => time.UserState)
                .HasDefaultValue(null);

            bulder.Entity<TimeTableModel>()
                .Property(time => time.UserCountry)
                .IsRequired();

            bulder.Entity<TimeTableModel>()
                .Property(time => time.AvrageDeliverTime)
                .IsRequired();
        }

        private void BuildDishModel(ref ModelBuilder builder) {
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
                .Property(dish => dish.IsAvailable)
                .HasDefaultValue(true);

            builder.Entity<DishModel>()
                .Property(dish => dish.RestaurantModelId)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.AvrageTimeToCook)
                .IsRequired();

            builder.Entity<DishModel>()
                .Property(dish => dish.Image)
                .HasDefaultValue(null);
        }

        private void BuildOrderModel(ref ModelBuilder builder) {
            builder.Entity<OrderModel>()
                .HasKey(order => order.Id);

            builder.Entity<OrderModel>()
                .Property(order => order.CurrentStatus)
                .HasDefaultValue("pending")
                .IsRequired();

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
                .Property(order => order.UserModelId)
                .IsRequired();

            builder.Entity<OrderModel>()
                .Property(order => order.RestaurantModelId)
                .IsRequired();
        }

        private void BuildOrderedDishes(ref ModelBuilder builder) {
            builder.Entity<OrderedDishesModel>()
                .Property(order => order.Notes)
                .HasDefaultValue(null);

            builder.Entity<OrderedDishesModel>()
                .Property(order => order.CurrentStatus)
                .HasDefaultValue("preparing")
                .IsRequired();
        }

        private void BuildReservationModel(ref ModelBuilder builder)
        {
            builder.Entity<ReservationModel>()
                .HasKey(res => res.Id);

            builder.Entity<ReservationModel>()
                .Property(order => order.Notes)
                .HasDefaultValue(null);

            builder.Entity<ReservationModel>()
                .Property(order => order.CurrentStatus)
                .HasDefaultValue("pending")
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.UserModelId)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.RestaurantModelId)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Amount_Of_Adults)
                .HasDefaultValue(1)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Amount_Of_Children)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.At_Date)
                .IsRequired();

            builder.Entity<ReservationModel>()
                .Property(order => order.Created_At)
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }

        private void BuildCuponModel(ref ModelBuilder builder) {
            builder.Entity<CuponModel>()
                .HasKey(cupon => cupon.CuponCode);

            builder.Entity<CuponModel>()
                .Property(cupon => cupon.Name)
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
