using System.Text.RegularExpressions;

namespace UsersMS.Core.Utils.RegExps
{
    public class PhoneNumberRegex
    {
        public static readonly Regex PhoneNumberRegexs = new Regex(@"^\+58 \d{3}-\d{7}$", RegexOptions.Compiled);
        internal static bool IsMatch(string phone)
        {
            return PhoneNumberRegexs.IsMatch(phone);
        }
    }
}
