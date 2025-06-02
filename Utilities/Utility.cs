using RestaurantSystem.Enums;
using System.Text;

namespace RestaurantSystem.Utilities
{
    public class Utility
    {

        public bool IsValidDishStatus(string status) { 
            return status.Equals(Status.Pending.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Preparing.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Ready.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public bool IsValidReservationStatus(string status)
        {
            return status.Equals(Status.Pending.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Accepted.ToString(), StringComparison.OrdinalIgnoreCase) ||
                   status.Equals(Status.Cancelled.ToString(), StringComparison.OrdinalIgnoreCase);
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

        private void RemoveImage(string image)
        {
            if (File.Exists(image))
            {
                File.Delete(image);
            }
        }

        public async Task<string?> UploadImage(string image)
        {
            // image.png;safawfedscwad==
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
