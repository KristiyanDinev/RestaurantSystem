### Folder and file structure
Here is how I structures the folders and the files.

Models which reprecent database row like (Id, Username, Email, etc.). 
They go to `Models/` folder. They are named as `UserModel.cs` or `DishModel.cs`. You named them
like that, because when you search classes or variables by name then this class is more likely to appear.

Controllers which handle what happens when a request reaches to an endpoint. And endpoint is
`/login` or `/order/dishes/current` or `/watch?v=WWWWWWWWW`. As you already may know that the files
will end with `Controller.cs` as in `UserController.cs` or `DishController.cs` and as you already may know
they will be in `Controllers/` folder.

What about the database? Everything related to the database which is commands, connections, query preperations 
and etc. are in `Database/` folder. Now here it gets interesting. While we are in the database folder where
all things should be related, we have a cross-reference with `Utils` and `Models`. By `Utils`, because of the
`SqlBuilder.cs` which as you can guess builds the SQL query as string which means that it is used only for the database
and that is why it is under `Database/Utils/` and by `Models`, because we have
`UserModel.cs` or `DishModel.cs` which we know they reprecent a database row, but they are used for multiple
perposes and not only database. Just because they come from the database it don't mean that they only belong there.
Actully the real general perpose of the models is to use the data from the database in all different ways. In HTML
or in controllers or even in checks or generation. So the models are multiperpose, therefore we have a seperate folder
for them in `Models/`. Have you heard "Frist planning, then coding"? Well that is why. Now moving files between the 
namespaces/folders isn't hard. It is just renaming and maybe some properties that are `protected` or have default access modifiers.

What are the difference between `Handlers` and `Managers`. Like `DatabaseManager.cs` and `UserDatabaseHandler.cs`.
The manager is the one who manages the handlers. Like think of the manager as a real manager in a shop and the employees
are the handlers. In the manager are the core functions that the handlers sometimes need. The handlers use the manager and not manager the handlers.
In some other more complex project maybe the manager can use some handlers to be made whole with it's core functions, but that is for other time.
The name `Handlers` means to handle a specific area or problem, while the `Managers` provide code functionality that is required or
it is repeated for all handlers.

What about the `Helpers`. Like `WebHelper.cs`. I like to think of helpers as of 