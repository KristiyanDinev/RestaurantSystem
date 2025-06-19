using Microsoft.EntityFrameworkCore;
namespace RestaurantSystem.Utilities
{
    public class Utility
    {
        public readonly static int pageSize = 10;

        public static string MakeCapital(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static async Task<string?> UploadImageAsync(IFormFile? Image)
        {
            if (Image == null)
            {
                return null;
            }
            
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            try
            {
                using FileStream fileStream = new FileStream("wwwroot/assets/images/user/" + imageName, FileMode.Create);
                await Image.CopyToAsync(fileStream);
                return "/assets/images/user/" + imageName;
            }
            catch (Exception)
            {
                return null;
            }
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

        public static async Task<string?> UpdateImage(string? OldImage, IFormFile? Image) {

            if (OldImage != null)
            {
                DeleteImage(OldImage);
            }

            return await UploadImageAsync(Image);
        }

        public static async Task<List<T>> GetPageAsync<T>(IQueryable<T> query, int pageNumber)
        {
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
