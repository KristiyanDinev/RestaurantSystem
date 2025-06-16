using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Enums;
namespace RestaurantSystem.Utilities
{
    public class Utility
    {
        public readonly static int pageSize = 10;
        private readonly static string deliveryRoleName = "delivery";

        public static bool IsValidDishStatus(string status) { 
            return status.Equals(Status.Pending.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Preparing.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Ready.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidReservationStatus(string status)
        {
            return status.Equals(Status.Pending.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Accepted.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Cancelled.ToString(), StringComparison.OrdinalIgnoreCase);
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
