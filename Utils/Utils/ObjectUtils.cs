using System.Reflection;

namespace ITStepFinalProject.Utils.Utils
{
    public class ObjectUtils
    {
        private static List<string> _Get_Model_Property_Names(object model)
        {
            List<string> names = new List<string>();
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                names.Add(property.Name);
            }
            return names;
        }

        public static List<string> Get_Model_Property_Names(object model)
        {
            return _Get_Model_Property_Names(model);
        }

        public static object? Get_Property_Value(object model, string property)
        {
            return Get_PropertyInfo(model, property)?.GetValue(model) ?? null;
        }

        public static void Set_Property_Value(object model, string property, object? value)
        {
            PropertyInfo? info = Get_PropertyInfo(model, property);
            if (info == null)
            {
                return;
            }

            if (info.PropertyType == typeof(int) && value is string)
            {
                value = int.Parse(Convert.ToString(value));

            } else if (info.PropertyType == typeof(DateTime) && value is string)
            {
                value = DateTime.Parse(Convert.ToString(value));

            }
            else if (info.PropertyType == typeof(DateOnly) && value is string)
            {
                value = DateOnly.Parse(Convert.ToString(value));

            }
            else if (info.PropertyType == typeof(decimal) && (value is double || value is float))
            {
                value = decimal.Parse(Convert.ToString(value));
            }
            info.SetValue(model, value, null);
        }

        public static PropertyInfo? Get_PropertyInfo(object model, string property)
        {
            return model.GetType().GetProperty(property);
        }
    }
}
