using System.Text.RegularExpressions;

namespace TaleofMonsters.Tools
{
    internal class NameChecker
    {
        public enum NameCheckResult
        {
            Ok,
            NameEmpty,
            NameLengthError,
            PunctuationOnly,
            EngOnly,
        }

        private static readonly Regex NORMAL_NAME_REGEX = new Regex("^[a-zA-Z0-9]+$"); //英文数字
        private static readonly Regex SYMBOL_REGEX = new Regex("[~!@#$%^&\\*\\(\\)\\+`\\-=\\{\\};\\'\\\",\\.\\/\\?:<>\\！￥……？，。：；‘“]+$");

        public static NameCheckResult CheckName(string name, uint minLen, uint maxLen)
        {
            if (string.IsNullOrEmpty(name))
                return NameCheckResult.NameEmpty;

            var nameLen = CountLength(name);
            if (nameLen < minLen || nameLen > maxLen)
                return NameCheckResult.NameLengthError;

            string noSymbolStr = SYMBOL_REGEX.Replace(name, string.Empty);
            if (noSymbolStr.Length == 0)
                return NameCheckResult.PunctuationOnly;

            return NameCheckResult.Ok;
        }

        public static NameCheckResult CheckNameEng(string name, uint minLen, uint maxLen)
        {
            if (string.IsNullOrEmpty(name))
                return NameCheckResult.NameEmpty;

            var nameLen = CountLength(name);
            if (nameLen < minLen || nameLen > maxLen)
                return NameCheckResult.NameLengthError;

            if (!NORMAL_NAME_REGEX.IsMatch(name))
                return NameCheckResult.EngOnly;

            return NameCheckResult.Ok;
        }

        private static int CountLength(string str)
        {
            return str.Length;
        }
    }
}