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

>More details on `RIDs` *(runtime identifier)* -> **https://learn.microsoft.com/en-us/dotnet/core/rid-catalog**

Build command for production (Windows 10/11)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"
```

Build command for production (Linux x64)
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"
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

## Steps for setting up SMTP (Simple Mail Transfer Protocol) for sending emails

### Gmail
- SMTP Settings

    Host: smtp.gmail.com

    Port: 587 (TLS) or 465 (SSL)

    Requires Auth: ✅

    SSL/TLS: ✅

- Usage Limits

    Free accounts: ~500 emails/day

    Google Workspace (paid): ~2,000 emails/day

    Attachment size: 25MB

- Pricing

    Free personal Gmail

    Workspace starts at $6/user/month

- Steps:
    1. Make sure you have 2-Steps Authentication on the account you want this app to use as the **Sender**. (At least 2 methods of authentication)

    2. Go to **https://myaccount.google.com/apppasswords** and create your app password. It should look like this: aaaa bbbb cccc dddd

    3. Set the `From_Email` option to the email address of that account.

    4. Set the `App_Password` option to the generated password, which Google gave you.


### iCloud Mail (Apple)
- SMTP Settings

    Host: smtp.mail.me.com

    Port: 587

    SSL/TLS: ✅

    Auth: ✅

- Usage Limits

    Daily cap: ~200–300 emails

    Attachment size: 20MB

- Pricing

    Free with iCloud email (5GB storage)

    Extra storage starts at $0.99/month


### Zoho Mail
- SMTP Settings

    Host: smtp.zoho.com

    Port: 587 (TLS) or 465 (SSL)

    SSL/TLS: ✅

    Auth: ✅

- Usage Limits

    Free: 500 emails/day

    Paid (Zoho Mail Standard): up to 30,000/day per domain

- Pricing

    Free (for up to 5 users with custom domain)

    Paid: from $1/user/month

### Outlook / Hotmail / Live (Microsoft)
- SMTP Settings

    Host: smtp.office365.com (Outlook/365)
    or smtp.live.com (Hotmail/Live)

    Port: 587

    SSL/TLS: ✅

    Auth: ✅

- Usage Limits

    Free accounts: 300 emails/day approx.

    Microsoft 365 Business: ~10,000/day (shared pool)

- Pricing

    Free (Outlook.com)

    Microsoft 365 Business: starts at ~$6/user/month

### Yahoo Mail
- SMTP Settings

    Host: smtp.mail.yahoo.com

    Port: 587 (TLS) or 465 (SSL)

    SSL/TLS: ✅

    Auth: ✅

- Usage Limits

    ~500 emails/day

    Attachments: 25MB

- Pricing

    Free with ads

    Yahoo Mail Plus: ~$5/month (no email limit increase)


### Comparison Table
| Provider | Free Limit     | App Password Req. | Paid Upgrade             | SMTP Host           |
| -------- | -------------- | ----------------- | ------------------------ | ------------------- |
| Gmail    | 500/day        |  (with 2FA)      | \$6/mo via Workspace     | smtp.gmail.com      |
| Yahoo    | 500/day        |  (with 2FA)      | \$5/mo Yahoo Mail Plus   | smtp.mail.yahoo.com |
| Outlook  | 300/day approx |  (with 2FA)      | \$6/mo Microsoft 365     | smtp.office365.com  |
| Zoho     | 500/day        |  (with 2FA)      | \$1/user/mo for business | smtp.zoho.com       |
| iCloud   | \~200–300/day  |  (with 2FA)      | \$0.99+/mo (for storage) | smtp.mail.me.com    |


## Database Notes

- Postgres -> C# DateTime only supports `DateTime.UtcNow`

