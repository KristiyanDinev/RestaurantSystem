### Resturant System

Here is how it works on client side:

- User first visits `/restaurants` to chose which restaurant he wants to use.
- Then the user can chose whether he can order dishes (`/dishes`) or made an reservation (`/reservation`).
- After he choses what he wants, then his order is taken to the staff team and he can track it.

Staff or employees are users who have roles, which give them access to specific services. A service is a url path.

**Delivery**
The delivery guy will get the orders from all the restaurants in the same city he has in his profile.

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

