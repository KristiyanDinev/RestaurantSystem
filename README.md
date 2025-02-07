### Resturant System

### Dependencies
- Npgsql

### Database
- database: Restorant
- user: ResturantUser
- password: password123

### Notes
- In text files the property names of the Model will be replaced 
Ex: `{{Id}}` -> `1`: If there is no data or error the `{{property}}` 
will be removed as a string from the file.

- The Order's CurrentStatus will be `db` when it is being prossesed by the server,
 but if the CurrentStatus is `pending` thne the order is ready to be accepted and it can be cancelled


### TODO
- Maybe Cupon Controller. We will see.
- The user to edit his profile.
