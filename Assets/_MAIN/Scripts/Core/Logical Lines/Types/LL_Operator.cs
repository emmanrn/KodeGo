using System;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;


using static DIALOGUE.LogicalLines.LogicalLineUtils.Expressions;
using Cinemachine;

// this script will provide the logical line to declare and modify variables within a dialogue file
namespace DIALOGUE.LogicalLines
{
    public class LL_Operator : ILogicalLine
    {
        // dont really need a keyword for this so we just leave it like this
        public string keyword => throw new System.NotImplementedException();

        public IEnumerator Execute(DIALOGUE_LINES line)
        {
            string trimmedLine = line.rawData.Trim();
            string[] parts = Regex.Split(trimmedLine, REGEX_ARITHMETIC);

            if (parts.Length < 3)
            {
                Debug.LogError($"Invalid command: {trimmedLine}");
                yield break;
            }

            string variable = parts[0].Trim().TrimStart(VariableStore.VARIABLE_ID);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];
            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            object value = CalculateValue(remainingParts);

            if (value == null)
                yield break;

            ProcessOperator(variable, op, value);
        }

        private void ProcessOperator(string variable, string op, object value)
        {
            if (VariableStore.TryGetValue(variable, out object currentValue))
            {
                ProcessOperatorOnVariable(variable, op, value, currentValue);
            }
            // this will only make it so that we will only create a variable if it doesnt exist if it has a direct assignment
            // so that means if we arew adding a variable that doesnt exist then we shouldnt be able to perform that operation
            else if (op == "=")
            {
                VariableStore.CreateVariable(variable, value);
            }
        }

        private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue)
        {
            switch (op)
            {
                case "=":
                    VariableStore.TrySetValue(variable, value);
                    break;
                case "+=":
                    VariableStore.TrySetValue(variable, ConcatenateOrAdd(currentValue, value));
                    break;
                case "-=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                    break;
                default:
                    Debug.LogError($"Invalid operator: {op}");
                    break;
            }
        }

        private object ConcatenateOrAdd(object currentValue, object value)
        {
            if (value is string)
                return currentValue.ToString() + value;

            return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
        }

        public bool Matches(DIALOGUE_LINES line)
        {
            Match match = Regex.Match(line.rawData.Trim(), REGEX_OPERATOR_LINE);

            return match.Success;
        }
    }

}