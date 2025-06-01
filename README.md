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

- `delivered` - the order is delivered to the user.

## Cart

You need an account and selected restaurant, so you can ses your cart.

## Reservations

You are required to log in, so you can use the reservations feature.

To get the reservation form you need to have selected a restaurant.
The rest of the endpoints don't requre a selected restaurant.


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
- `/staff`
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

### TODO

- Add profile access even on the restaurant part.

### Add test data
*Note: Make sure that this will be the first data inserted in the tables in the database.*

```sql
INSERT INTO "Restaurants" 
("Address", "City", "State", "Country", 
"PostalCode", "DoDelivery", "ServeCustomersInPlace", 
"ReservationMaxChildren", "ReservationMinChildren", 
"ReservationMaxAdults", "ReservationMinAdults") 
VALUES 
('ul. Test', 'Sofia', NULL, 'Bulgaria', '1234', TRUE, TRUE, 10, 0, 4, 1),
('ul. Test2', 'Sofia', NULL, 'Bulgaria', '1235', TRUE, FALSE, 10, 0, 4, 1),
('ul. Test3', 'Sofia', NULL, 'Bulgaria', '1236', FALSE, TRUE, 10, 0, 4, 1);

INSERT INTO "Dishes" 
("Name", "Price", "Grams", "Image", "Ingredients", 
"Type_Of_Dish", "IsAvailable", "AvrageTimeToCook", "Notes", "RestaurantId")
VALUES 
('Some salad', 22.30, 220, '/assets/images/salad/1.png', 'Ingredients...', 'salad', TRUE, '2 - 3 minutes', 'Some notes on the dish', 1),
('Some salad2', 2.30, 210, NULL, 'Ingredients...', 'salad', TRUE, '2 - 3 minutes', 'Some notes on the dish', 1),
('Some salad3', 10, 100, NULL, 'Ingredients...', 'salad', FALSE, '2 - 3 minutes', 'Some notes on the dish', 1),

('Some drink', 31, 210, NULL, 'Ingredients...', 'drink', TRUE, '2 - 3 minutes', 'Some notes on the dish', 1),
('Some drink2', 12, 100, NULL, 'Ingredients...', 'drink', FALSE, '2 - 3 minutes', 'Some notes on the dish', 1),

('Premium Some salad', 22.30, 220, '/assets/images/salad/1.png', 'Ingredients...', 'salad', TRUE, '2 - 3 minutes', 'Some notes on the dish', 2),
('Premium Some salad2', 2.30, 210, NULL, 'Ingredients...', 'salad', TRUE, '2 - 3 minutes', 'Some notes on the dish', 2),
('Premium Some salad3', 10, 100, NULL, 'Ingredients...', 'salad', FALSE, '2 - 3 minutes', 'Some notes on the dish', 2),

('Premium Some drink', 31, 210, NULL, 'Ingredients...', 'drink', TRUE, '2 - 3 minutes', 'Some notes on the dish', 2),
('Premium Some drink2', 12, 100, NULL, 'Ingredients...', 'drink', FALSE, '2 - 3 minutes', 'Some notes on the dish', 2);


INSERT INTO "Roles"
("Name", "Description")
VALUES
('staff', 'Access to staff.'),
('delivery', 'Access to online delivery.'),
('waitress', 'Access to serving the people in place and handling reservations.'),
('manager', 'Access to handle all staff for a specific restaurant.'),
('cook', 'Access to see the orders and their dishes to cook them for that restaurant. Includes online orders.');

INSERT INTO "Cupons" 
("Name", "CuponCode", "DiscountPercent", "ExpirationDate")
VALUES
('Summer', 'Summer123', 15, '01-01-2027'),
('Winter', 'Winter123', 20, '01-01-2027');

INSERT INTO "Services" 
("Path", "Description")
VALUES
('/staff', 'Staff home service.'),
('/staff/dishes', 'The cook service.'),
('/staff/manager', 'The manager service.'),
('/staff/reservations', 'Reservations for waitresses.'),
('/staff/delivery', 'Delivery page for the delivery staff.');

INSERT INTO "Role_Permissions" 
("RoleName", "ServicePath")
VALUES
('staff', '/staff'),
('delivery', '/staff/delivery'),
('delivery', '/staff'),
('waitress', '/staff/reservations'),
('waitress', '/staff'),
('manager', '/staff/manager'),
('manager', '/staff'),
('manager', '/staff/dishes'),
('manager', '/staff/reservations'),
('manager', '/staff/delivery'),
('cook', '/staff/dishes'),
('cook', '/staff');
```

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

### Local testing notes:

Username: Bob
Email: bob@example.com
Password: 123
Address: ul. Home
City: Sofia
State: *(empty)*
Country: Bulgaria
Postal Code: 1234
PhoneNumber: +3592523523


Staff manager
Username: John
Email: john@example.com
Password: 1234
Address: ul. Home1
City: Sofia
State: *(empty)*
Country: Bulgaria
Postal Code: 12345
PhoneNumber: +3592523511
Roles: manager