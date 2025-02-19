### WebHelper

Here is how it works.
Every model has a name. For example the class UserModel.cs 
has a name of `User` in the front-end in HTML.
In the html there can be placeholders for that model which reprecent components.
For example a placeholder for the user bar is `{{UserBar}}`.
The backend gets the components for the common placeholders that are in the html 
in order to replace them with the html. How does that work? 
The backend will have a cicle which goes through the common placeholders and check
whether or not they are in the main html page. If they are not then the 
placeholders are skiped, because they will not have any effect. If the placeholder is in
the main html page. It loads the html from the `/WebComponent/Component.html` for that model 
and the model is gotten from the database if it has it not currently. Let me break it down.

Model -> the C# class which is used in the backed and in returning results in `POST` endpoints.
Component -> the html which reprecents the C# model. That HTML will be used in the 
 front-end when working with placeholders.

Ok. We have the html component and the model. What is next? Replacing the placeholder with the html component.
Also remember that the component itself can have placeholders for the different properties of the model.
So we go through another replacement of placeholder, but this time is for the properties of the model.
You can have a list of C# models and one html component. The code will just generate multiple html components that 
reprecent the list of C# models. Like that you can use these html components to display list of data. 
You can use a placeholder which will be replaced with these multiple html components. And like that you have a dynamic Website with server rendering.

Good to note here. The text: `UserBar` in placeholder `{{UserBar}}` will 
be the component name and it will be searched in `/WebComponent/UserBar.html`. 
This is the same with all placeholders.

Here is how it works in simple terms:

C# Model -> class User { public int __Id;  public string Name; }
HTML Component (per model) -> <div> <p>{{User.__Id}}</p> <p>{{User.Name}}</p> </div>


server:
User user1 = new User();
user1.__Id = 1;
user1.Name = "John";

User user2 = new User();
user2.__Id = 2;
user2.Name = "Ivan";

List<User> users = new List<User>();
users.Add(user1);
users.Add(user2);

 // this "User" acts like prefix of the model property in the html
mainPageHTML = WebHelper.GetModelsHTML("User", users, mainPageHTML, "ComponentName", "{{UserBar}}");
mainPageHTML = WebHelper.GetModelsHTML("User", users, mainPageHTML, "ComponentName", "{{UserOrders}}");
return mainPageHTML

// as you can see the server generates multiple 
// html pages for every model and brings them together.
// You can also use it for a single model.


### ASP.NET Session
Each session has a unique key. Each user has this key stored in the browser in 
header in the cookie. So when a user makes a request. The server gets that key 
and gets the session data. Then the server can make any changes to the session 
data and save the changes. These sessions can be in-memory or in database or in 
files or anywhere where you can store data.



### Model system
Think of this system as one class = one row
