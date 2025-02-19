using System.Text;

namespace ITStepFinalProject.Utils
{
    public class WebHelper
    {

        /* whole page html
         * <head>
         * </head>
         * <body>
         * ...
         * <div>
         * {{UserModels}}
         * </div>
         * ...
         * </body>
         * 
         * 
         * model html - per model
         * 
         * <div>
         * <p>{{User.Name}}</p>
         * <p>{{User.Email}}</p>
         * <p>{{User.Address}}</p>
         * </div>
         * 
         */

        public static List<string> commonPlaceholders =
        [
            "{{UserBar}}", "{{Profile}}"
        ];

        public static string FillInPlaceholdersForModel(string modelName,
            object model, string html)
        {
            foreach (string property in 
                ModelUtils.Get_Model_Property_Names(model))
            {
                html = html.Replace("{{" + modelName + "." + property + "}}", 
                    Convert.ToString(ModelUtils.Get_Property_Value(model, property)));
            }
            return html;
        }

        public static string GetHTMLForModel(string componet)
        {
            try
            {
                return File.ReadAllText("/WebComponent/" + componet + ".html");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetModelsHTML(string modelName, List<object> models, string html,
            string componentName)
        {  // componentName will be the placeholder {{UserBar}}
            if (!html.Contains(componentName))
            {
                return html;
            }

            componentName = componentName.Remove(0, 2);
            componentName = componentName.Remove(componentName.Length - 2, 2);

            string modelHtml = GetHTMLForModel(componentName);
            if (modelHtml.Length == 0)
            {
                return html.Replace(componentName, "");
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (object model in models)
            {
                stringBuilder.AppendLine(FillInPlaceholdersForModel(modelName, model, modelHtml));
            }

            return html.Replace(componentName, stringBuilder.ToString());
        }


        public static string HandleCommonPlaceholders(string html, string modelName, 
            List<object> models) { 
        // will the component name be the placeholder the one that is for a whole model? yes
            foreach (string placeholder in commonPlaceholders)
            {
                html = GetModelsHTML(modelName, models, html, placeholder);
            }
            return html;
        }
    }
}
