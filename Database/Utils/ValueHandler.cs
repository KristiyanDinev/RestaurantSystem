using ITStepFinalProject.Utils.Utils;

namespace ITStepFinalProject.Database.Utils
{
    public class ValueHandler
    {
        private static readonly string _hashingSlat = "D6RTYFUYGIBUNOI";

        public static string HashString(string str)
        {
            return Convert.ToBase64String(EncryptionHandler.HashIt(str + _hashingSlat));
        }

        public static string Strings(object? obj, bool allowEmptyStrings = false)
        {
            string? str = obj?.ToString();
            if (str == null || (string.IsNullOrWhiteSpace(str) && !allowEmptyStrings))
            {
                return "null";
            }
            // Users   'User"s'  User"s"  // 'User"s"' 
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
