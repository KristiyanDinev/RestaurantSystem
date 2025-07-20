# Resturant System


## Dishes and Restaurant

First of all that user must be logged in to see the restaurants.
So the system can actually pick the restaurants in his city.

When a user visits the website, he needs to select a restaurant (`/restaurants`), then that 
restaurat is saved in the cookie by `restaurant_id` key (it is an **int**).
That string `restaurant_id` is located in `Controllers/RestaurantController.cs` 
and `Services/RestaurantService.cs`.

You need to select a restaurant, before you got to see the dishes.

## Orders

You can't cancel your order, when the current status of the order is other then `pending`.
*Note: the pending status check is located in `Services/OrderService.cs function DeleteOrder(int orderId)`,
but the string itself is in `Database/DatabaseContext.cs string DefaultOrder_CurrentStatus`*

Users can order home delivery from `POST /Order/Start` endpoint.
Waitress staff can place orders in the restaurant they work in from `... work in progress...` endpoint.

When client starts a websocket and starts to track his own orders.
He sends a JSON like: `{"orders": [1, 2, 3]}`
- The orders, which will be tracked by the websocket. No matter what kind of order it is. Still, it will be tracked.

Orders have a `TableNumber`, which is a string and it can be null if it is a online order.

The only websocket for now is `/ws/orders` which is to set listeners for all of the order ids.
That client should only receive JSON data from teh server regarding updates on only these orders, no matter if they
are the user's or not.

### Order Status

- `pending` - the order is not yet accepted by the staff.

- `preparing` - the order is accepted and the staff is preparing it.

- `ready` - the order is ready to be picked up by the delivery guy or the user.
- 
- `delivering` - the order is being delivered by the delivery guy.

- `delivered` - the order is delivered to the user.

## Cart

You need an account and selected restaurant, so you can ses your cart.

## Reservations

You are required to log in, so you can use the reservations feature.

To get the reservation form you need to have selected a restaurant.
The rest of the endpoints don't requre a selected restaurant.

**Waitress Staff**

You need first to cancel the reservation, before you delete it.

Users can't cancel their reservation, if it is already canceled or it is 2 hours before the reservation time.

### Reservation Status
- `pending` - the reservation is not yet accepted by the staff.

- `accepted` - the reservation is accepted by the staff.

- `canceled` - the reservation is canceled by the user or the staff.

## Login

When the user is logged in, he has a cookie `Authentication: Bearer ...`, which represent an 
encrypted jwt token for authenticating the user.

The server assigns this cookie to the user after a successful login. This JWT may contain 
a expiration date, which means that the user has not selected the **remember me** option.
If there is no such date, then the user has selected that option and the token will continue to be 
valid until the actul encryption key or jwt verification key is changed by the server.

## Restaurant

When starting an order you need to be logged in and have a selected restaurant.
When browsing through your orders you only need to be logged in.

## Roles and Services

services:
- `/staff/dishes`
- `/staff/manager`
- `/staff/reservations`
- `/staff/delivery`

roles:
- `delivery`
- `staff`
- `waitress`
- `manager`
- `cook`

## Staff

Staff or employees are users who have roles, which give them access to specific services. A service is a url path.

**Delivery**
The delivery guy will get the orders from all the restaurants in the same city as his address in his profile.
He can get different restaurants based on his own profile address.

*Note: This can make the job of a delivery guy voluntary, but he needs an admin, manager or an owner to
update his profile if he is changing cities. 
He can freely update his **address** (meaning the streat and building number), because this information is not
used, when the system picks up restaurants for him to do delivery.
The restaurants are based on **city**, **country** and **state**.
Default role name for delivery guy is `delivery`, 
found in `Controllers/UserController.cs function ProfileUpdate(ProfileUpdateFormModel)`.*

Staff endpoints:

- `/admin/dishes` - service is for the cooks, to see which dish to cook and to give feedback on the user.


### Dependencies
- Npgsql
- System.IdentityModel.Tokens.Jwt
- Microsoft.IdentityModel.Tokens

### Database
- database: Restorant
- user: ResturantUser
- password: password123

### Add test data
*Note: Make sure that this will be the first data inserted in the tables in the database.*

Add Role
```sql
INSERT INTO "User_Roles" 
("UserId", "RoleName")
VALUES
(2, 'manager');
```

Remove Role
```sql
DELETE FROM "User_Roles" WHERE "UserId" = 2 AND "RoleName" = 'manager';
```

Add Restaurant To User
```sql
UPDATE "Users" SET "RestaurantId" = 1 WHERE "Id" = 2;
```

Remove Restaurant From User
```sql
UPDATE "Users" SET "RestaurantId" = NULL WHERE "Id" = 2;
```

Add migration
```bash
dotnet ef migrations add NameOfMigration
```

Apply migration
```bash
dotnet ef database update
```

Build command for production (Windows 10/11)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"
```

Add new package
```bash
dotnet add package <PackageName> --version <Version>
```

Remove package
```bash
dotnet remove package <PackageName>
```

ASP.NET with Hot Reload (VSC)
```bash
dotnet watch run
```

## TODO

- Add cupons for the manager to manage.

- Add Restaurant manager to manage the reservations (on/off/limits)

- If there is time. Add cupon checks in the cart and waitress add order page.

- Waitresses update total price of reservations if needed.

