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

```sql
INSERT INTO "Restaurants" 
("Address", "City", "State", "Country", 
"PostalCode", "DoDelivery", "ServeCustomersInPlace", 
"ReservationMaxChildren", "ReservationMinChildren", 
"ReservationMaxAdults", "ReservationMinAdults") 
VALUES 
('ul. Test', 'Sofia', 'Sofia City', 'Bulgaria', '1234', TRUE, TRUE, 10, 0, 4, 1),
('ul. Test2', 'Sofia', 'Sofia City', 'Bulgaria', '1235', TRUE, FALSE, 10, 0, 4, 1),
('ul. Test3', 'Sofia', 'Sofia City', 'Bulgaria', '1236', FALSE, TRUE, 10, 0, 4, 1);

-- SALADS
INSERT INTO "Dishes" ("Name", "Price", "Grams", "Image", "Ingredients", "Type_Of_Dish", "IsAvailable", "AvrageTimeToCook", "RestaurantId") VALUES
('Caesar Salad', 8.50, 250, '/assets/images/salad/1.png', 'Romaine lettuce, croutons, parmesan, Caesar dressing', 'salads', TRUE, '5 - 7 minutes', 1),
('Greek Salad', 7.20, 200, NULL, 'Tomatoes, cucumbers, onions, olives, feta cheese', 'salads', TRUE, '4 - 6 minutes', 1),
('Caprese Salad', 6.80, 180, NULL, 'Tomatoes, mozzarella, basil, olive oil', 'salads', TRUE, '3 - 5 minutes', 1),
('Waldorf Salad', 7.90, 220, NULL, 'Apples, celery, walnuts, mayonnaise', 'salads', FALSE, '4 - 6 minutes', 1),
('Quinoa Salad', 9.00, 260, NULL, 'Quinoa, cucumber, bell pepper, lemon juice', 'salads', FALSE, '6 - 8 minutes', 1),

-- SOUPS
('Tomato Basil Soup', 5.50, 300, NULL, 'Tomatoes, basil, cream, garlic', 'soups', TRUE, '10 - 12 minutes', 1),
('Chicken Noodle Soup', 6.20, 350, NULL, 'Chicken, noodles, carrots, celery', 'soups', TRUE, '12 - 15 minutes', 1),
('Minestrone', 6.00, 320, NULL, 'Vegetables, beans, pasta, tomato broth', 'soups', TRUE, '11 - 13 minutes', 1),
('French Onion Soup', 6.90, 330, NULL, 'Onions, beef broth, bread, cheese', 'soups', FALSE, '13 - 15 minutes', 1),
('Lentil Soup', 5.80, 300, NULL, 'Lentils, carrots, onion, cumin', 'soups', FALSE, '10 - 12 minutes', 1),

-- APPETIZERS
('Bruschetta', 4.50, 150, NULL, 'Grilled bread, tomatoes, garlic, basil', 'appetizers', TRUE, '5 - 7 minutes', 1),
('Mozzarella Sticks', 5.00, 200, NULL, 'Mozzarella, breadcrumbs, marinara', 'appetizers', TRUE, '6 - 8 minutes', 1),
('Stuffed Mushrooms', 6.30, 180, NULL, 'Mushrooms, cheese, herbs', 'appetizers', TRUE, '7 - 9 minutes', 1),
('Deviled Eggs', 4.20, 160, NULL, 'Eggs, mayonnaise, mustard, paprika', 'appetizers', FALSE, '4 - 6 minutes', 1),
('Spring Rolls', 5.50, 190, NULL, 'Vegetables, rice paper, dipping sauce', 'appetizers', FALSE, '6 - 8 minutes', 1),

-- MAIN DISHES
('Grilled Salmon', 14.50, 400, NULL, 'Salmon, lemon, herbs, olive oil', 'dishes', TRUE, '15 - 20 minutes', 1),
('Spaghetti Carbonara', 12.00, 350, NULL, 'Pasta, eggs, pancetta, cheese', 'dishes', TRUE, '10 - 12 minutes', 1),
('Beef Stroganoff', 13.50, 360, NULL, 'Beef, mushrooms, sour cream, noodles', 'dishes', TRUE, '14 - 18 minutes', 1),
('Chicken Alfredo', 11.80, 340, NULL, 'Chicken, pasta, Alfredo sauce', 'dishes', FALSE, '12 - 15 minutes', 1),
('Vegetable Stir Fry', 10.20, 300, NULL, 'Mixed vegetables, soy sauce, rice', 'dishes', FALSE, '10 - 12 minutes', 1),

-- DESSERTS
('Chocolate Lava Cake', 6.00, 180, NULL, 'Chocolate, flour, sugar, eggs', 'desserts', TRUE, '8 - 10 minutes', 1),
('Tiramisu', 5.80, 150, NULL, 'Mascarpone, espresso, ladyfingers, cocoa', 'desserts', TRUE, '6 - 8 minutes', 1),
('Apple Pie', 5.50, 200, NULL, 'Apples, cinnamon, crust', 'desserts', TRUE, '12 - 15 minutes', 1),
('Panna Cotta', 5.20, 160, NULL, 'Cream, gelatin, vanilla, sugar', 'desserts', FALSE, '5 - 7 minutes', 1),
('Baklava', 5.60, 170, NULL, 'Phyllo dough, nuts, honey syrup', 'desserts', FALSE, '10 - 12 minutes', 1),

-- DRINKS
('Lemonade', 2.50, 250, NULL, 'Lemon, sugar, water', 'drinks', TRUE, '2 - 3 minutes', 1),
('Iced Tea', 2.80, 250, NULL, 'Tea, lemon, sugar, ice', 'drinks', TRUE, '3 - 4 minutes', 1),
('Espresso', 3.00, 80, NULL, 'Coffee beans, water', 'drinks', TRUE, '2 - 3 minutes', 1),
('Hot Chocolate', 3.20, 200, NULL, 'Milk, cocoa powder, sugar', 'drinks', FALSE, '3 - 5 minutes', 1),
('Milkshake', 4.00, 300, NULL, 'Milk, ice cream, flavor syrup', 'drinks', FALSE, '4 - 6 minutes', 1),


('Cobb Salad', 9.00, 280, NULL, 'Chicken, bacon, avocado, blue cheese, eggs', 'salads', TRUE, '6 - 8 minutes', 2),
('Borscht', 6.50, 300, NULL, 'Beets, cabbage, potatoes, sour cream', 'soups', TRUE, '15 - 18 minutes', 2),
('Shrimp Cocktail', 7.80, 200, NULL, 'Shrimp, cocktail sauce, lemon', 'appetizers', TRUE, '4 - 6 minutes', 2),
('Lamb Tagine', 16.00, 400, NULL, 'Lamb, apricots, spices, couscous', 'dishes', TRUE, '20 - 25 minutes', 2),
('Mango Sticky Rice', 5.80, 220, NULL, 'Sticky rice, coconut milk, mango', 'desserts', TRUE, '8 - 10 minutes', 2),

('Fattoush Salad', 8.00, 250, NULL, 'Mixed greens, pita chips, sumac, radish', 'salads', TRUE, '5 - 7 minutes', 3),
('Pho', 9.50, 400, NULL, 'Beef broth, rice noodles, herbs, beef slices', 'soups', TRUE, '12 - 15 minutes', 3),
('Gyoza', 6.50, 200, NULL, 'Dumplings, pork, cabbage, soy sauce', 'appetizers', TRUE, '6 - 8 minutes', 3),
('Ratatouille', 11.00, 350, NULL, 'Zucchini, eggplant, bell peppers, tomato', 'dishes', TRUE, '14 - 18 minutes', 3),
('Tapioca Pudding', 5.00, 180, NULL, 'Tapioca pearls, milk, vanilla', 'desserts', TRUE, '8 - 10 minutes', 3);

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
('/staff/dishes', 'The cook service.'),
('/staff/manager', 'The manager service.'),
('/staff/reservations', 'Reservations for waitresses.'),
('/staff/orders', 'Orders for waitresses.'),
('/staff/delivery', 'Delivery page for the delivery staff.');

INSERT INTO "Role_Permissions" 
("RoleName", "ServicePath")
VALUES
('cook', '/staff/dishes'),
('delivery', '/staff/delivery'),
('waitress', '/staff/reservations'),
('waitress', '/staff/orders'),
('manager', '/staff/manager'),
('manager', '/staff/dishes'),
('manager', '/staff/reservations'),
('manager', '/staff/orders'),
('manager', '/staff/delivery');
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


### TODO
- Waitress staff to mark the order as served and delete the orders onces the customers leave the restaurant.
 (if customers will have an temporary account to track the order)
