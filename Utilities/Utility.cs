
namespace RestaurantSystem.Utilities
{
    public class Utility
    {
        public readonly static int pageSize = 10;
        public readonly static string restaurantId = "restaurant_id";
        public readonly static string delivery_address_header = "delivery_address_id";
        public readonly static string delivery_restaurant_header = "delivery_restaurant_id";

        private static async Task<string?> UploadImageAsync(IFormFile? Image, string assetPath)
        {
            if (Image == null || Image.Length >= 10_000_000)
            {
                return null;
            }
            string extension = Path.GetExtension(Image.FileName).ToLowerInvariant();
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
            if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            string imageName = $"{Guid.NewGuid():N}{extension}";
            try
            {
                string path = "wwwroot" + assetPath;
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                using FileStream fileStream = new FileStream(
                    Path.Combine(path, imageName), FileMode.CreateNew);
                await Image.CopyToAsync(fileStream);
                return assetPath + imageName;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Image upload failed: {exception.Message}");
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

            string? newImage = isDish ? await UploadDishImageAsync(Image) :
                await UploadUserImageAsync(Image);
            if (newImage != null && OldImage != null)
            {
                DeleteImage(OldImage);
            }
            return newImage;
        }

        public static IQueryable<T> GetPageAsync<T>(IQueryable<T> query, int pageNumber)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
