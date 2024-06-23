
using System.Linq;
using UnityEngine;

namespace BebiLibs.Analytics.GameEventLogger
{
    public class LoggerNameValidator
    {
        public static string ValidateName(string parameterName)
        {
            string oldParameter = parameterName;
            string errorText = string.Empty;

            RemoveSpaces(ref parameterName, ref errorText);
            RemoveExtraLength(ref parameterName, ref errorText);
            RemoveFirstNumericCharacter(ref parameterName, ref errorText);
            RemoveNonAlphanumericCharacters(ref parameterName, ref errorText);
            CheckForZeroLength(ref parameterName, ref errorText);

            if (!string.IsNullOrEmpty(errorText))
            {
                Debug.LogError($"InvalidParameterName, Parameter name: {oldParameter} should not: {errorText} Replaced with: {parameterName}");
            }

            return parameterName;
        }

        private static void RemoveSpaces(ref string parameterName, ref string errorText)
        {
            if (parameterName.Length == 0 || !parameterName.Contains(" "))
                return;

            parameterName = parameterName.Replace(" ", "");
            errorText += "have space characters, ";
        }

        private static void RemoveExtraLength(ref string parameterName, ref string errorText)
        {
            if (parameterName.Length <= 40)
                return;

            parameterName = parameterName.Substring(0, 40);
            errorText += "be longer than 40 characters, ";
        }


        private static void RemoveFirstNumericCharacter(ref string parameterName, ref string errorText)
        {
            if (parameterName.Length == 0 || !char.IsDigit(parameterName[0]))
                return;

            parameterName = RemoveFirstDigits(parameterName);
            errorText += "start with number, ";
        }

        private static void RemoveNonAlphanumericCharacters(ref string parameterName, ref string errorText)
        {
            if (parameterName.Length == 0 || !ContainsNonAlphaNumeric(parameterName))
                return;

            parameterName = RemoveNonAlphaNumericCharacters(parameterName);
            errorText += "contain non alphanumeric characters other than underscore (_), ";
        }

        private static void CheckForZeroLength(ref string parameterName, ref string errorText)
        {
            if (parameterName.Length != 0)
                return;

            parameterName = "Null";
            errorText += "Be Empty, ";
        }

        public static string RemoveNonAlphaNumericCharacters(string name)
        {
            return string.Concat(name.Where(x => IsAlphanumericOrUnderscore(x)));
        }

        public static bool ContainsNonAlphaNumeric(string s) => !s.All(c => IsAlphanumericOrUnderscore(c));
        public static bool IsAlphanumericOrUnderscore(char c) => (c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122) || c == 95;

        public static string RemoveFirstDigits(string parameterName)
        {
            try
            {
                return string.Concat(parameterName.SkipWhile(char.IsDigit));
            }
            catch
            {
                return parameterName;
            }
        }
    }
}
