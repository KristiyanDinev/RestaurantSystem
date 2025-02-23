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



### DB
If you have a `float` or `double` property in your model in C# class that points to a `decimal` field in the DB.
Please change it, because it won't convert. Change th property to a `decimal`. Decimal can still have a floating point and
it is used like `1.23m`.

### Controllers
If you want to use simple body post form `[FromForm] class something` or more simpler `[FromForm] string username`
The same thing applies to queries which is `search?id=....&title=...` in the url `[FromQuery] class/string type`.
How about uploading files? Use `[FromFrom] string base64File` then decode it. 
**Be of warning to set upload limits or else you service may crash or slow down, because of large files.**
I usualy DisableAntiforgery(), because that key work like this: User sends request to your server. You server has a job to check if that request was 
intentional made from the original website or app or was it send by someone who is trying to break through. 
And basicly if the request has this key it is verified and can continue, but I say no, because first the guy may have the key, 
because it may got exposed or the guy can simply redirect the user's requests so the key can be autofilled. So I don't trust it
completely. I say a better way is to use `Encryption`. If you get an request that contains the data in an valid `encrypted` state,
then it is most likely the original user who is making that requst, because how can the guy who is trying to manipulate the server
into making his request accepted if he doesn't have the `encryption key`? Here we apply the same idea of an identifer for validation, but
in our case only the server knows that this data actually came from the user. I say that you use this method if you need to send
a protected request to your server and if you don't know much about the framework you use it still works fine as long as you hide that key.
See the guy may get the `encrypted` data, but he can't decrypt it, because he doesn't have the key, while in the `Antiforgery` situation
he can get the get and manipulate the server if he is skilled enough. What if he resends the same request he recored with the same encrypted data?
When then that request will be accepted, but here is more of a question on how do you handle your request. Does it just do the action it is
expected to do or does it check the data on the server and user and then do what it is expected to do.


### Tips
Please keep your code clean and small. By `small` I mean no more then 500 to 1000 lines per file. I myself am still trying to keep it this way.
If you file is more then these lines then consider making another file that handles the other part of it. Don't use too much abstraction. Here is how 
you would know if you use too much abstraction or not. `Abstraction` is like a library or framework or a class you made yourself that takes care of helping you in your project 
and you use that `abstraction` turn big parts of your code to smaller parts of your code. Too much abstraction is when you basicly have to edit the `abstraction` class more then the classes in 
your project. Like the `abstraction` classes become a large amout of your project as files and libraries and you basicly edit the `abstraction` rather then the code for the project itself.
The main idea of `abstraction` is to make your code smaller by handling often used statements. Too little abstraction is when you repeat yourself too much and after a month you can't understand 
a thing of what you wrote, because it is so much code or poor variable and class names.

