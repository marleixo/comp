using System;
using System.Reflection;
using System.ComponentModel;

namespace DMExport.Library.Helpers
{
    public class EnumHelper
    {
        /// <summary>
        /// Gets field name by Enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString();
        }
    }
}


