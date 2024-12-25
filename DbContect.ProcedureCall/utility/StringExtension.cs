using System.Linq;

namespace EF.StoreProcedureHelper.utility
{
    internal static class StringExtension
    {
        internal static string FirstCharToLower(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.First().ToString().ToLower() + string.Join("", input.Skip(1));
        }
    }
}
