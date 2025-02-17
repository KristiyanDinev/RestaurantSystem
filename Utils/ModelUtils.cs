using System.Reflection;

namespace ITStepFinalProject.Utils
{
    public class ModelUtils
    {

        private static List<string> _Get_Model_Property_Names_With_Prefix(
            object model, string? prefix, bool no_prefix)
        {
            List<string> names = new List<string>();

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                string name = property.Name;
                if (no_prefix && !name[0].Equals('_')) {
                    names.Add(name);

                } else if (prefix == null || name.StartsWith(prefix))
                {
                    names.Add(name);
                }
            }

            return names;
        }

        public static List<string> Get_Exceptional_Model_Property_Names(object model)
        {
            return _Get_Model_Property_Names_With_Prefix(model, "_", false);
        }

        public static List<string> Get_Identity_Model_Property_Names(object model)
        {
            return _Get_Model_Property_Names_With_Prefix(model, "__", false);
        }

        public static List<string> Get_All_Model_Property_Names(object model)
        {
            return _Get_Model_Property_Names_With_Prefix(model, null, false);
        }

        public static List<string> Get_View_Model_Property_Names(object model)
        {
            return _Get_Model_Property_Names_With_Prefix(model, null, true);
        }

        public static object Get_Property_Value(object model, string property)
        {
            return model.GetType().GetProperty(property).GetValue(model);
        }

        public static void Set_Property_Value(object model, string property, object value)
        {
            model.GetType().GetProperty(property).SetValue(model, value, null);
        }
    }
}
