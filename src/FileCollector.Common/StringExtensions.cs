using System.Text.RegularExpressions;

namespace FileCollector.Common
{
    public static class StringExtensions
    {
        public static bool MatchPattern(this string text, string pattern, string wildcard = "*")
        {
            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(pattern))
            {
                string escapedWildcard = Regex.Escape(wildcard);

                string pat = Regex.Escape(pattern);
                pat = $"^{pat.Replace(escapedWildcard, ".*")}$";

                Regex reg = new Regex(pat, RegexOptions.IgnoreCase);

                return reg.IsMatch(text);
            }
            return false;
        }
    }
}
