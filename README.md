### Resturant System

### Dependencies
- Npgsql
- System.IdentityModel.Tokens.Jwt
- Microsoft.IdentityModel.Tokens

### Database
- database: Restorant
- user: ResturantUser
- password: password123

### Git access token
- `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`

### Notes
- In text files the property names of the Model will be replaced 
Ex: `{{User.__Id}}` -> `1`: If there is no data or error the `{{property}}` 
will be removed as a string from the file. There is prefix of that property Like `{{User.property}}`.
The `User` is the model name.

- Global elements to replace in static files. User bar -> `{{UserBar}}`. 
 This can be replaced by a html component which the server generates.

- The Order's CurrentStatus will be `db` when it is being prossesed by the server,
 but if the CurrentStatus is `pending` thne the order is ready to be accepted and it can be cancelled


- `ResturantAddressAvrageDeliverTime` -> has format of `ul. User;Sofia;;Bulgaria|5m - 10m|ul. Resturatn;Sofia;;Bulgaria---ul. User2;Sofia;;Bulgaria|1m - 2m|ul. Resturatn;Sofia;;Bulgaria`
  `local user address;user City;user State (can be empty if none);user Country|Avrage Time to Deliver|local restorant address;restorant city;restorant state;resturant country`

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

### TODO
- Maybe Cupon Controller. We will see.
- InsertRestorantModel.