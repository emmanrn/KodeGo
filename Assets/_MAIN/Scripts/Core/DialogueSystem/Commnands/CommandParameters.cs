using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace COMMANDS
{
    public class CommandParameters
    {
        private const char PARAMETER_IDENTIFIER = '-';
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        private List<string> unlabeledParams = new List<string>();

        public CommandParameters(string[] parameters, int startingIndex = 0)
        {
            for (int i = startingIndex; i < parameters.Length; i++)
            {
                // as long as this value starts with '-' and is also not interpreted as a float value then it MUST be an identifier
                // this is so we can actually use negative numbers as params
                if (parameters[i].StartsWith(PARAMETER_IDENTIFIER) && !float.TryParse(parameters[i], out _))
                {
                    string pName = parameters[i];
                    string pValue = "";

                    if (i + 1 < parameters.Length && !parameters[i + 1].StartsWith(PARAMETER_IDENTIFIER))
                    {
                        pValue = parameters[i + 1];
                        i++;
                    }

                    this.parameters.Add(pName, pValue);
                }
                else
                    unlabeledParams.Add(parameters[i]);
            }
        }

        // this will allow to either have one parameter value or multiple to find a certain param
        public bool TryGetValue<T>(string parameterName, out T value, T defaultVal = default(T)) => TryGetValue(new string[] { parameterName }, out value, defaultVal);
        public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultVal = default(T))
        {
            foreach (string parameterName in parameterNames)
            {
                if (parameters.TryGetValue(parameterName, out string paramVal))
                {
                    if (TryCastParam(paramVal, out value))
                    {
                        return true;
                    }
                }

            }
            // if we reach here, no match was found for the identified params
            // so search the unlabeled params instead
            foreach (string parameterName in unlabeledParams)
            {
                if (TryCastParam(parameterName, out value))
                {
                    unlabeledParams.Remove(parameterName);
                    return true;
                }

            }

            value = defaultVal;
            return false;
        }

        private bool TryCastParam<T>(string paramVal, out T value)
        {
            if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(paramVal, out bool boolVal))
                {
                    value = (T)(object)boolVal;
                    return true;
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(paramVal, out int intVal))
                {
                    value = (T)(object)intVal;
                    return true;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(paramVal, out float floatVal))
                {
                    value = (T)(object)floatVal;
                    return true;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                value = (T)(object)paramVal;
                return true;
            }

            value = default(T);
            return false;
        }




    }

}