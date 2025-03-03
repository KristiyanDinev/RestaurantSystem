using ITStepFinalProject.Utils.Utils;
using System.Text;

namespace ITStepFinalProject.Utils.Web
{
    public class WebUtils
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

        private Dictionary<string, List<string>> commonPlaceholders;
        private TemplateRenderer _templateRenderer;

        public WebUtils(Dictionary<string, List<string>>  _commonPlaceholders)
        {
            commonPlaceholders = _commonPlaceholders;
            _templateRenderer = new TemplateRenderer();
        }

        public string FillInPlaceholdersForModel(string modelName,
            object model, string html)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            foreach (string property in
                ObjectUtils.Get_Model_Property_Names(model))
            {
                variables.Add("{{" + modelName + "." + property + "}}", 
                    Convert.ToString(ObjectUtils.Get_Property_Value(model, property) ?? ""));
            }

            html = _templateRenderer.Render(html, variables);

            foreach (string key in variables.Keys)
            {
                html = html.Replace(key, Convert.ToString(variables[key]));
            }
            return html;
        }

        public string GetHTMLForModel(string componet)
        {
            try
            {
                return File.ReadAllText("WebComponent/" + componet + ".html");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public string GetModelsHTML(string modelName, List<object> models, string html,
            string componentName)
        {  // componentName will be the placeholder {{UserBar}}
            if (!html.Contains(componentName))
            {
                return html;
            }

            string fullPlaceholder = componentName;
            componentName = componentName.Remove(0, 2);
            componentName = componentName.Remove(componentName.Length - 2, 2);

            string modelHtml = GetHTMLForModel(componentName);
            if (modelHtml.Length == 0)
            {
                return html.Replace(fullPlaceholder, "");
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (object model in models)
            {
               stringBuilder.AppendLine(FillInPlaceholdersForModel(modelName, model, modelHtml));
            }

            return html.Replace(fullPlaceholder, stringBuilder.ToString());
        }


        public string HandleCommonPlaceholders(string html, string modelName, 
            List<object> models) { 
        // will the component name be the placeholder the one that is for a whole model? yes
            foreach (string placeholder in commonPlaceholders[modelName])
            {
                html = GetModelsHTML(modelName, models, html, placeholder);
            }
            return html;
        }
    }
}
