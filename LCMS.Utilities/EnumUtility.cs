using System.ComponentModel;
using System.Reflection;

namespace LCMS.Utilities
{
    public static class EnumUtility
    {
        /// <summary>
        /// Get the description of an enum value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Enum value as string.</returns>
        public static string GetEnumValue(object? item)
        {
            if (item == null) return string.Empty;

            var enumField = item.GetType()
                .GetTypeInfo()
                .GetField(item.ToString() ?? string.Empty);

            var enumDescriptionAttribute = enumField != null
                ? (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false)
                : null;

            var value = enumDescriptionAttribute != null
                ? enumDescriptionAttribute.Length > 0
                    ? enumDescriptionAttribute[0].Description ?? string.Empty
                    : item.ToString() ?? string.Empty
                : string.Empty;

            return value;
        }
    }
}
