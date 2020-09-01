using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenTicket.Domain.Utility
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Convert enum member to human readable words.
        /// It takes from <see cref="DescriptionAttribute"/> when available or using camel-case as a word.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string Humanize<TEnum>(this TEnum enumeration) where TEnum : System.Enum
        {
            var enumType = typeof(TEnum);
            if (!System.Enum.IsDefined(enumType, enumeration)) return null;
            string memberName = enumeration.ToString();
            var fieldInfo = enumType.GetField(memberName);
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttributes.Length > 0) return ((DescriptionAttribute)descriptionAttributes[0]).Description;

            var allCapsPattern = new Regex(@"^[A-Z]+$");
            var words = (from word in Regex.Split(memberName, @"([A-Z][a-z0-9]+)")
                         where !string.IsNullOrWhiteSpace(word)
                         select allCapsPattern.IsMatch(word) ? word : word.ToLower()).ToArray();
            return string.Join(" ", words);
        }
    }
}
