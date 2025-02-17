﻿### Resturant System

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
Ex: `{{User.__Id}}` -> `1`: If there is no data or error the `{{property}}` 
will be removed as a string from the file. There is prefix of that property Like `{{User.property}}`.
The `User` is the model name.

- Global elements to replace in static files. User bar -> `{{UserBar}}`. 
 This can be replaced by a html component which the server generates.

- The Order's CurrentStatus will be `db` when it is being prossesed by the server,
 but if the CurrentStatus is `pending` thne the order is ready to be accepted and it can be cancelled


- `ResturantAddressAvrageDeliverTime` -> has format of `ul. User;Sofia;Bulgaria|5m - 10m|ul. Resturatn;Sofia;Bulgaria---ul. User2;Sofia;Bulgaria|1m - 2m|ul. Resturatn;Sofia;Bulgaria`

### TODO
- Maybe Cupon Controller. We will see.
- The user to be able to start orders and to see all his orders
- RestorantAddress builder for appsettings.json
- Update all things with the new Model System and WebHelper.

- Rework Utils.cs
- Rework all controllers
- add checks for "where" and limiting and ordering in DatabaseManager