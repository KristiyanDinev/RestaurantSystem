using ITStepFinalProject.Models;
using System.Security.Claims;

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


        public static void _handleEmptyEntryInFile(ref string FileData, object model) {
            foreach (string property in
                    model.GetType().GetProperties().Select(f => f.Name).ToList()) {
                FileData = FileData.Replace("{{" + property + "}}", "");
            }
        }


        public static void _handleEntryInFile(ref string FileData, object model) {
            Type type = model.GetType();
            foreach (string property in
                    type.GetProperties().Select(f => f.Name).ToList()) {

                FileData = FileData.Replace("{{" + property + "}}",
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
    }
}
