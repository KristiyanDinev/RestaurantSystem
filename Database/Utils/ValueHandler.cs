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

        public static string Strings(object? obj)
        {
            string? str = obj?.ToString();
            if (string.IsNullOrWhiteSpace(str))
            {
                return "null";
            }

            return "'"+str.Replace("'", "''")+"'";
        }

        public static object GetModelPropertyValue(object model, string property)
        {
            object? value = ObjectUtils.Get_Property_Value(model, property);
            if ((value is string || value == null) || 
                (value is DateOnly || value is DateTime || value is DateTimeOffset || value is DateTimeKind))
            {
                value = Strings(value);
            }
            return value;
        }
    }
}
