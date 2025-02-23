using System.Reflection;

namespace ITStepFinalProject.Utils
{
    public class ModelUtils
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

        public static object Get_Property_Value(object model, string property)
        {
            return Get_PropertyInfo(model, property).GetValue(model);
        }

        public static void Set_Property_Value(object model, string property, object value)
        {
            Get_PropertyInfo(model, property).SetValue(model, value, null);
        }

        public static PropertyInfo? Get_PropertyInfo(object model, string property)
        {
            return model.GetType().GetProperty(property);
        }
    }
}
