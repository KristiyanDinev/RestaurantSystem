using ITStepFinalProject.Utils;
using System.Text;

namespace ITStepFinalProject.Database.Utils
{
    public class ValueHandler
    {
        private static readonly string _hashingSlat = "D6RTYFUYGIBUNOI";

        public static string HashString(string str)
        {
            return Convert.ToBase64String(
                Program.hashing?.ComputeHash(
                    Encoding.UTF8.GetBytes(str + _hashingSlat)) ?? []);
        }

        public static string Strings(object? str)
        {
            return str == null || ((string)str).Replace(" ", "").Length == 0 ?
                "null" : "'" + ((string)str).Replace("'", "''") + "'";
        }

        public static object GetModelPropertyValue(object model, string property)
        {
            object value = ModelUtils.Get_Property_Value(model, property);
            if (value is string || value == null)
            {
                value = Strings(value);
            }
            return value;
        }
    }
}
