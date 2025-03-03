using ITStepFinalProject.Utils.Utils;
using System.Security.Cryptography;
using System.Text;

namespace ITStepFinalProject.Database.Utils
{
    public class ValueHandler
    {
        private static readonly string _hashingSlat = "D6RTYFUYGIBUNOI";

        public static string HashString(string str)
        {
            using var hashing = SHA256.Create();
            return Convert.ToBase64String(EncryptionHandler.HashIt(str + _hashingSlat));
        }

        public static string Strings(object? str)
        {
            return str == null || ((string)str).Replace(" ", "").Length == 0 ?
                "null" : "'" + ((string)str).Replace("'", "''") + "'";
        }

        public static object GetModelPropertyValue(object model, string property)
        {
            object? value = ObjectUtils.Get_Property_Value(model, property);
            if (value is string || value == null)
            {
                value = Strings(value);
            }
            return value;
        }
    }
}
