### Resturant System

### Dependencies
- Npgsql
- System.IdentityModel.Tokens.Jwt
- Microsoft.IdentityModel.Tokens

### Database
- database: Restorant
- user: ResturantUser
- password: password123

The `Orders` table reprecents all the orders that were done. When the `CurrentStatus` of an order is
`db` then either it is in the prosses of being added to the database or it failed to be added in the database,
so you can delete it if it failed to do so.
The `CurrectStatus` will be `pending` when the order was added successfully to the database and it is waiting to
 be accepted by staff.
The `CurrectStatus` will be `completed` when the order has been completed and delivered to the user and if there is any
 payment to be made. It is made, but there maybe will be exceptions based on the restorant's politics.

In case the restorant has a physical place to go eat. Then there also the staff has to create an online order, from his staff
 account. This is done, because the total sales will be calculated based on the amount of completed orders and also the manager of 
 the restorant to see how many orders have a staff done in his worktime.

The `CurrectStatus` will be `stopped` if it got cancelled by the user only when the status is `db` or `pending` which should be when the
 order has not yet been accepted.

- `ReservationMaxAdults` and `ReservationMaxChildren` are set to `-1` by default which means no limit. if the number is `-1` or bellow `-1` then no limit should be used.
- If you want to disable reservations then set `ReservationMaxAdults` and `ReservationMaxChildren` to `0` and `ReservationMinAdults` and `ReservationMinChildren` to `-1`
 This can be done from the UI as planned or from the database.

### Git access token
- `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`

### Notes
- In text files the property names of the Model will be replaced 
Ex: `{{User.__Id}}` -> `1`: If there is no data or error the `{{property}}` 
will be removed as a string from the file. There is prefix of that property Like `{{User.property}}`.
The `User` is the model name.

- Global elements to replace in static files. User bar -> `{{UserBar}}`. 
 This can be replaced by a html component which the server generates.

- See the `Orders`'s `CurrentStatus` in the DB notes.

- `Auth` header will contain a encrypted JWT token representing the user's data.

- If statement supported for comparing numbers and lenght of strings. Example:
```
{% if "{{User.Image}}".length > 0 %}
    <img src="{{User.Image}}">

{% elseif "{{User.Image}}".length == 0 %}
   <img>
{% else %}
    <p>idk</p>
{% endif %}
```
You open it with `{% if ... %}` and close it with `{% endif %}`. The `{% elseif ... %}` and `{% else %}` is optional

- Added transactions to the database

### TODO
- Staff page for seeing orders, reservations and updating / cancelling them.
- Admin page for adding staff.
- Dish Controller needs some work. Handle dish avaliable in what restorants.