﻿using ITStepFinalProject.Models;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using ITStepFinalProject.Database;
using System.Net;

namespace ITStepFinalProject.Utils {
    public class Utils {

        public static bool IsDateExpired(ISession session,
           string key) {
            try {
                return DateTime.Parse(session.GetString(key)) <= DateTime.Now;

            } catch (Exception) {
                return false;
            }
        }


        public static byte[] FromStringToUint8Array(string data) {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++) {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }

        public static void _handleRememberMe(ref ISession session,
            string remeberMe, int Id) {

            session.SetInt32("UserId", Id);
            if (remeberMe.Equals("off")) {

                session.SetString("UserId_ExpirationDate",
                    DateTime.Now.AddDays(1.0).ToString());
            }
        }



        public static async Task<string> GetFileContent(string path) {
            // path => /login => /dishes
            if (path.Contains('.')) {
                return await File.ReadAllTextAsync($"wwwroot{path}");

            } else {
                return await File.ReadAllTextAsync($"wwwroot{path}/Index.html");
            }

        }

        public static void ApplyUserBarElement(ref string data, UserModel user)
        {
            data = data.Replace("{{UserBar}}", @$"
        <div class=""user"">

               <button class=""navigation_button"" onclick=""goToCart()"">🛒</button>
    
           <button  class=""navigation_button"" onclick=""goToOrders()"">Orders</button>
        
             <button class=""navigation_button"" onclick=""goToDishes()"">Dishes</button>
            
            <div class=""profile"" onclick=""goToProfile()"">
                
                <img src=""{user.Image}"">
                <p>{user.Username}</p>
            </div>

           
             <div class=""logout"">
                <button onclick=""Logout()"" id=""logout"">Log Out <img src=""https://cdn-icons-png.flaticon.com/512/1286/1286853.png "">  </button>
             </div>

        </div>
");
        }

        public static void ApplyRestorantAddress(ref string data, UserModel user)
        {
            List<ResturantAddressModel> filtered = new List<ResturantAddressModel>();

            foreach (ResturantAddressModel restorantAddresses 
                in Program.resturantAddresses)
            {
                if (restorantAddresses.UserCity.Equals(user.City) &&
                    restorantAddresses.UserCountry.Equals(user.Country) &&
                    restorantAddresses.UserAddress.StartsWith(user.Address))
                {
                    filtered.Add(restorantAddresses);
                }
            }

            StringBuilder stringBuilder = new StringBuilder("<select id=\"resturant_address\">");

            foreach (ResturantAddressModel res in filtered)
            {
                stringBuilder.AppendLine($@"<option 
value='{res.RestorantAddress+';'+res.RestorantCity+';'+res.RestorantCountry}' selected='selected'>{
                    res.RestorantAddress +", "+res.RestorantCity+", "+res.RestorantCountry + " : "+res.AvrageTime
                    }</option>");
            }

            stringBuilder.Append("</select>");
            data = data.Replace("{{ResturantAddress}}", stringBuilder.ToString());
        }

        public static void _handleEntryInFile(ref string FileData, 
            object model, string prefix) {
            Type type = model.GetType();
            foreach (string property in
                    type.GetProperties().Select(f => f.Name).ToList()) {

                FileData = FileData.Replace("{{" +prefix +"."+ property + "}}",
                    Convert.ToString(type.GetProperty(property).GetValue(model)));
            }

        }


        public static int? IsLoggedIn(ISession session) {
            try {
                return session.GetInt32("UserId");

            } catch (Exception) {
                return null;
            }
        }

        public static void RemoveImage(string image)
        {
            if (File.Exists(image))
            {
                File.Delete(image);
            }
        }

       public static async Task<string?> UploadImage(string image)
        {
            string[] imageParts = image.Split(';');
            string imageName = imageParts[0];
            string imageData = imageParts[1];

            string byteData = Encoding.UTF8.GetString(
                Convert.FromBase64String(imageData));
            if (!(imageName.Contains('\\') ||
                imageName.Contains('/') ||
                imageName.Contains('\'') ||
                byteData.EndsWith(',') ||
                byteData.StartsWith(',')))
            {
                string imgPath = "wwwroot/images/user/" + imageName;
                RemoveImage(imgPath);
                using FileStream fs =
                    File.Create("wwwroot/images/user/" + imageName);
                await fs.WriteAsync(FromStringToUint8Array(byteData));

                return "/images/user/" + imageName;
            }
            return null;
        }

        
    }
}
