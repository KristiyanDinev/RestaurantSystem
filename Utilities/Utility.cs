
namespace RestaurantSystem.Utilities
{
    public class Utility
    {
        public readonly static int pageSize = 10;
        public readonly static string restaurantId = "restaurant_id";
        public readonly static string delivery_address_header = "delivery_address_id";
        public readonly static string delivery_restaurant_header = "delivery_restaurant_id";

        public static string MakeCapital(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        private static async Task<string?> UploadImageAsync(IFormFile? Image, string assetPath)
        {
            if (Image == null)
            {
                return null;
            }
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            try
            {
                using FileStream fileStream = new FileStream("wwwroot/"+assetPath + imageName, FileMode.Create);
                await Image.CopyToAsync(fileStream);
                return assetPath + imageName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string?> UploadUserImageAsync(IFormFile? Image)
        {
            return await UploadImageAsync(Image, "/assets/images/user/");
        }

        public static async Task<string?> UploadDishImageAsync(IFormFile? Image)
        {
            return await UploadImageAsync(Image, "/assets/images/dishes/");
        }

        public static void DeleteImage(string? img)
        {
            if (img == null)
            {
                return;
            }
            string oldImagePath = "wwwroot" + img;
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }
        }

        public static async Task<string?> UpdateImageAsync(string? OldImage, 
            IFormFile? Image, bool isDish = false) {

            if (OldImage != null)
            {
                DeleteImage(OldImage);
            }

            return isDish ? await UploadDishImageAsync(Image) :
                await UploadUserImageAsync(Image);
        }

        public static IQueryable<T> GetPageAsync<T>(IQueryable<T> query, int pageNumber)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
