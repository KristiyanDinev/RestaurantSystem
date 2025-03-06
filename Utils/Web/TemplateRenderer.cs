using System.Text.RegularExpressions;

namespace ITStepFinalProject.Utils.Web
{
    public class TemplateRenderer
    {
        private static readonly Regex IfRegex = new Regex(@"{%\s*if\s+(.*?)\s+%}(.*?)(?:{%\s*elseif\s+(.*?)\s+%}(.*?))*(?:{%\s*else\s*%}(.*?))?{%\s*endif\s*%}",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public string Render(string template, Dictionary<string, object> variables = null)
        {
            variables = variables ?? new Dictionary<string, object>();

            // Process all if statements
            return ProcessIfStatements(template, variables);
        }

        private string ProcessIfStatements(string template, Dictionary<string, object> variables)
        {
            return IfRegex.Replace(template, match =>
            {
                string condition = match.Groups[1].Value;
                string ifContent = match.Groups[2].Value;

                // Check if the condition is true
                if (EvaluateCondition(condition, variables))
                {
                    return ifContent;
                }

                // Check elseif conditions
                int groupIndex = 3;
                while (groupIndex < match.Groups.Count && match.Groups[groupIndex].Success)
                {
                    string elseifCondition = match.Groups[groupIndex].Value;
                    string elseifContent = match.Groups[groupIndex + 1].Value;

                    if (EvaluateCondition(elseifCondition, variables))
                    {
                        return elseifContent;
                    }

                    groupIndex += 2;
                }

                // If we got here, check if there's an else block
                if (match.Groups[match.Groups.Count - 1].Success)
                {
                    return match.Groups[match.Groups.Count - 1].Value;
                }

                // If no conditions matched and no else block, return empty string
                return string.Empty;
            });
        }

        private string SetVariables(string part, Dictionary<string, object> variables)
        {
            foreach (var variable in variables)
            {
                part = part.Replace(variable.Key, variable.Value?.ToString() ?? "null");
            }
            return part;
        }

        private string[] SplitByOperator(string condition, string _operator, 
            Dictionary<string, object> variables)
        {
            string[] parts = condition.Split(_operator);
            string leftPart = parts[0].Trim();
            string rightPart = parts[1].Trim();

            leftPart = SetVariables(leftPart, variables);
            rightPart = SetVariables(rightPart, variables);
            string[] strings = { leftPart, rightPart };
            return strings;
        } 

        private bool EvaluateCondition(string condition, Dictionary<string, object> variables)
        {
            try
            {
                condition = condition.Trim();
                // Handle boolean literals  | DataTables Do not support strings.
                string tempCondition = SetVariables(condition, variables).ToLower();
                if (tempCondition.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (tempCondition.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }



                // Parse operators
                if (condition.Contains("=="))
                {
                    string[] parts = SplitByOperator(condition, "==", variables);

                    object left = EvaluateValue(parts[0]);
                    object right = EvaluateValue(parts[1]);
                    return Equals(left, right);
                }
                else if (condition.Contains("!="))
                {
                    string[] parts = SplitByOperator(condition, "!=", variables);

                    object left = EvaluateValue(parts[0]);
                    object right = EvaluateValue(parts[1]);
                    return !Equals(left, right);
                }
                else if (condition.Contains(">="))
                {
                    string[] parts = SplitByOperator(condition, ">=", variables);

                    double left = Convert.ToDouble(EvaluateValue(parts[0]));
                    double right = Convert.ToDouble(EvaluateValue(parts[1]));
                    return left >= right;
                }
                else if (condition.Contains("<="))
                {
                    string[] parts = SplitByOperator(condition, "<=", variables);
                    double left = Convert.ToDouble(EvaluateValue(parts[0]));
                    double right = Convert.ToDouble(EvaluateValue(parts[1]));
                    return left <= right;
                }
                else if (condition.Contains('>'))
                {
                    string[] parts = SplitByOperator(condition, ">", variables);
                    double left = Convert.ToDouble(EvaluateValue(parts[0]));
                    double right = Convert.ToDouble(EvaluateValue(parts[1]));
                    return left > right;
                }
                else if (condition.Contains('<'))
                {
                    string[] parts = SplitByOperator(condition, "<", variables);
                    double left = Convert.ToDouble(EvaluateValue(parts[0]));
                    double right = Convert.ToDouble(EvaluateValue(parts[1]));
                    return left < right;
                }
                else if (condition.Contains("&&"))
                {
                    string[] parts = SplitByOperator(condition, "&&", variables);

                    bool left = Convert.ToBoolean(EvaluateCondition(parts[0], variables));
                    bool right = Convert.ToBoolean(EvaluateCondition(parts[1], variables));
                    return left && right;
                }
                else if (condition.Contains("||"))
                {
                    string[] parts = SplitByOperator(condition, "||", variables);

                    bool left = Convert.ToBoolean(EvaluateCondition(parts[0], variables));
                    bool right = Convert.ToBoolean(EvaluateCondition(parts[1], variables));
                    return left || right;
                }

                // If no operators, just evaluate the condition as a value
                object result = EvaluateValue(condition.Trim());
                if (result is bool boolResult)
                {
                    return boolResult;
                }

                if (result is string strResult)
                {
                    return !string.IsNullOrEmpty(strResult);
                }

                if (result is int intResult)
                {
                    return intResult != 0;
                }

                if (result is double doubleResult)
                {
                    return doubleResult != 0;
                }

                // Default to false if we couldn't evaluate
                return false;
            }
            catch
            {
                // If there's any error in evaluation, return false
                return false;
            }
        }

        

        private object EvaluateValue(string value)
        {

            // Try to parse as number
            if (int.TryParse(value, out int intResult))
            {
                return intResult;
            }

            if (double.TryParse(value, out double doubleResult))
            {
                return doubleResult;
            }

            // Check for boolean values
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            } 

            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // If it's a quoted string, remove the quotes
            if (value.StartsWith('\"'))
            {
                if (value.EndsWith(".length"))
                {
                    return value.Substring(1, value.Length - 9).Length;
                }
                if (value.EndsWith('\"'))
                {
                    return value.Substring(1, value.Length - 2);
                }
            }
                

            // Handle special values
            if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            } 

            return value;
        }
    }
}
