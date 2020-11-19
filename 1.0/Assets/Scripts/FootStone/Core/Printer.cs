using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public static class Printer
    {
        public static string ToVisibleName(this Type type)
        {
            if (type.DeclaringType != null)
            {
                return type.DeclaringType.ToTypeDefineName() + "." + type.ToTypeDefineName();
            }
            return type.ToTypeDefineName();
        }

        public static void PrintTitle(this TextWriter output, string name, object value, string prefix = "    ", int namePlaceHolder = 24)
        {
            output.WriteLine(string.Format("{0}{1} : {2}]",
                prefix,
                CUtils.FillPlaceHolder("[" + name, namePlaceHolder, ' ', 1),
                value));
        }
 
        public static void PrintLine(this TextWriter output, string name, object value, string prefix = "    ", int namePlaceHolder = 24)
        {
            output.WriteLine(string.Format("{0}{1} = {2}",
                   prefix,
                   CUtils.FillPlaceHolder(name, namePlaceHolder, ' ', 1),
                   value));
        }

    }
}
