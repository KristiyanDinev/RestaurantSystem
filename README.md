### Resturant System

### Dependencies
- Npgsql

### Database
- database: Restorant
- user: ResturantUser
- password: password123

### Git access token
- `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`

### Notes
- In text files the property names of the Model will be replaced 
Ex: `{{Id}}` -> `1`: If there is no data or error the `{{property}}` 
will be removed as a string from the file. There si prefix of that property Like `{{User.property}}`

- Global elements to replace in static files. User bar -> `{{UserBar}}`

- The Order's CurrentStatus will be `db` when it is being prossesed by the server,
 but if the CurrentStatus is `pending` thne the order is ready to be accepted and it can be cancelled


- `ResturantAddressAvrageDeliverTime` -> has format of `ul. User;Sofia;Bulgaria|5m - 10m|ul. Resturatn;Sofia;Bulgaria---ul. User2;Sofia;Bulgaria|1m - 2m|ul. Resturatn;Sofia;Bulgaria`
### TODO
- Maybe Cupon Controller. We will see.
- The user to be able to start orders and to see all his orders
- RestorantAddress builder for appsettings.json






### Model system
Think of this system as one class = one row
- Model will be a class which has no methods, but only properties with get; and set;

- Model's properties that won't be touched when updating in the database will start with `_property` . 
  These are like constant properties, except if you edit them by yourself.

- Model's property that is used to identify the model in the database when updating will start with `__property`. 
  That property will also be included in the `SET`, but it's value won't change, because it will be the same.
  Also when deleting this value will be used to identify the row.