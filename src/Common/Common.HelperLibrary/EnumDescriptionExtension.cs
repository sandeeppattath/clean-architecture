using System.ComponentModel;

namespace Common.HelperLibrary
{
    public static class EnumDescriptionExtension
    {
        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            string description;
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
                else
                {
                    description = "-";
                }
            }
            else
            {
                description = "-";
            }

            return description;
        }
    }
}
