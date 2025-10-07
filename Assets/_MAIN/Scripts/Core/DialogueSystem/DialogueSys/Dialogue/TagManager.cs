using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

// this script injects data into tags contained within the dialogue files
public class TagManager
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        { "<mainChar>", () => "Kode" },
        { "<time>", () => DateTime.Now.ToString("hh:mm tt") },
        { "<tempVal1>", () => "Hello World" }
    };
    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text, bool injectTags = true, bool injectVariables = true)
    {
        if (injectTags)
            text = InjectTags(text);

        if (injectVariables)
            text = InjectVariables(text);

        return text;
    }

    private static string InjectTags(string value)
    {

        if (tagRegex.IsMatch(value))
        {
            foreach (Match match in tagRegex.Matches(value))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    // we're using tagValueRequest like a function here is because we cast it as a Func<string>
                    value = value.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return value;

    }

    private static string InjectVariables(string value)
    {
        var matches = Regex.Matches(value, VariableStore.REGEX_VARIABLE_IDS);
        var matchesList = matches.Cast<Match>().ToList();

        for (int i = matches.Count - 1; i >= 0; i--)
        {
            var match = matchesList[i];
            string variableName = match.Value.TrimStart(VariableStore.VARIABLE_ID, '!');
            bool negate = match.Value.StartsWith('!');

            bool endsInIllegalChar = variableName.EndsWith(VariableStore.DATABASE_VARIABLE_ATTRIBUTE_ID);

            if (endsInIllegalChar)
                variableName = variableName.Substring(0, variableName.Length - 1);

            if (!VariableStore.TryGetValue(variableName, out object variableValue))
            {
                UnityEngine.Debug.Log($"Variable {variableName} not found in string assignment");
                continue;
            }

            if (negate && variableValue is bool)
                variableValue = !(bool)variableValue;

            int lengthToBeRemoved = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;
            if (endsInIllegalChar)
                lengthToBeRemoved -= 1;

            value = value.Remove(match.Index, lengthToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }

        return value;
    }
}
