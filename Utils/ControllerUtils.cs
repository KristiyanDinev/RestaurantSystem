﻿using ITStepFinalProject.Models;
using System.Text;

namespace ITStepFinalProject.Utils {
    public class ControllerUtils
    {

        private EncryptionHandler _encryptionHandler;
        private JWTHandler _jwtHandler;

        public readonly string UserModelName = "User";
        public readonly string OrderModelName = "Order";
        public readonly string DishModelName = "Dish";
        public readonly string RestorantModelName = "Restorant";

        public ControllerUtils(EncryptionHandler encryptionHandler, JWTHandler jwtHandler)
        {
            _encryptionHandler = encryptionHandler;
            _jwtHandler = jwtHandler;
        }


        public byte[] FromStringToUint8Array(string data)
        {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++)
            {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }


        public async Task<string> GetHTMLFromWWWROOT(string path)
        {
            // path => /login => /dishes
            if (path.Contains('.'))
            {
                return await File.ReadAllTextAsync($"wwwroot{path}");

            }
            else
            {
                return await File.ReadAllTextAsync($"wwwroot{path}/Index.html");
            }

        }

        public List<int>? GetCartItems(HttpContext context)
        {
            try
            {
                List<string> dishesIdsStr = context.Request.Cookies["cart"].Split('-').ToList();
                List<int> dishesIds = new List<int>();

                foreach (string dishIdStr in dishesIdsStr)
                {
                    if (int.TryParse(dishIdStr, out int dishId))
                    {
                        dishesIds.Add(dishId);
                    }
                }
                return dishesIds;

            } catch (Exception)
            {
                return new List<int>();
            }
        }
        public void RemoveImage(string image)
        {
            if (File.Exists(image))
            {
                File.Delete(image);
            }
        }

        public async Task<string?> UploadImage(string image)
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


        public async Task<IResult> HandleDefaultPage_WithUserModel(string path,
            HttpContext context, UserUtils userUtils, WebUtils webUtils)
        {
            try
            {
                UserModel? user = await userUtils.GetUserModelFromAuth(context);
                if (user == null)
                {
                    return Results.Redirect("/login");
                }

                string FileData = await GetHTMLFromWWWROOT(path);
                FileData = webUtils.HandleCommonPlaceholders(FileData, UserModelName, [user]);

                return Results.Content(FileData, "text/html");

            }
            catch (Exception)
            {
                return Results.Redirect("/error");
            }
        }


        /*
         * public JsonElement GetModelFromSession(ISession session, string modelName)
        {
            return JsonSerializer.Deserialize<JsonElement>(session.GetString(modelName));
            /*
            string key = modelName+":" + modelID + ":";
            foreach (string property in ModelUtils.Get_Model_Property_Names(model))
            {
                if (ModelUtils.Get_PropertyInfo(model, property).PropertyType is int)
                {
                    ModelUtils.Set_Property_Value(model, property,
                        session.Get(key + property));
                } else
                {
                    ModelUtils.Set_Property_Value(model, property,
                        session.GetString(key + property));
                }
                    
            }
            return model;*/
    }
}
