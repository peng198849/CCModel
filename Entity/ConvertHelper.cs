using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public sealed class ConvertHelper
    {
        private ConvertHelper() { }

        public static string ToString(object source)
        {
            if (source == null)
                return null;

            return source.ToString();
        }

        public static DateTime ToDateTime(object source)
        {
            if (source == null)
                return DateTime.MinValue;

            DateTime resultDateTime = DateTime.MinValue;
            if (!DateTime.TryParse(source.ToString(), out resultDateTime))
                return DateTime.MinValue;

            return resultDateTime;
        }

        public static int ToInt32(object source)
        {
            if (source == null)
                return int.MinValue;

            int result = int.MinValue;
            if (!int.TryParse(source.ToString(), out result))
                return int.MinValue;

            return result;
        }

        public static long ToInt64(object source)
        {
            if (source == null)
                return long.MinValue;

            long result = long.MinValue;
            if (!long.TryParse(source.ToString(), out result))
                return long.MinValue;

            return result;
        }
    }
}
