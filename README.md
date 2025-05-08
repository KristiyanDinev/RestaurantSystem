# Resturant System


## Dishes and Restaurant

When a user visits the website, he needs to select a restaurant (`/restaurants`), then that 
restaurat is saved in the cookie by `restaurant_id` key (it is an **int**).

You need to select a restaurant, before you got to see the dishes.
You don't need a login or registression to see the retaurants or dishes.

## Orders

You can't cancel your order, when the current status of the order is other then `pending`.
*Note: the pending status check is located in `Services/OrderService.cs function DeleteOrder(int orderId)`*

Users can order home delivery from `POST /Order/Start` endpoint.

Waitress staff can place orders in the restaurant they work in from `... work in progress..` endpoint.

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

Staff or employees are users who have roles, which give them access to specific services. A service is a url path.

**Delivery**
The delivery guy will get the orders from all the restaurants in the same city as his address in his profile.
He can get different restaurants based on his own profile address.
*Note: This can make the job of a delivery guy voluntary, but he needs an admin, manager or an owner to
update his profile if he is changing cities. 
He can freely update his **address** (meaning the streat and building number), because this information is not
used, when the system picks up restaurants for him to do delivery.
The restaurants are based on **city**, **country** and **state**.*

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

