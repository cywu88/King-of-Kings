using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public static class CUtils
    {
        public static readonly Encoding UTF8 = new UTF8Encoding(false, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="count"></param>
        /// <param name="placeholder"></param>
        /// <param name="anchor">0=left, 1=right, 2=center</param>
        /// <returns></returns>
        public static string FillPlaceHolder(string src, int count, char placeholder = ' ', int anchor = 0)
        {
            if (src.Length >= count) { return src; }
            using (var auto = StringBuilderObjectPool.AllocAutoRelease())
            {
                var sb = auto.Output;
                if (anchor == 2)
                {
                    var left_space = (count - src.Length) >> 1;
                    var right_space = count - src.Length - left_space;
                    for (int i = 0; i < left_space; ++i)
                    {
                        sb.Append(placeholder);
                    }
                    sb.Append(src);
                    for (int i = 0; i < right_space; ++i)
                    {
                        sb.Append(placeholder);
                    }
                }
                else
                {
                    var space = count - src.Length;
                    if (anchor == 0) { sb.Append(src); }
                    for (int i = 0; i < space; ++i)
                    {
                        sb.Append(placeholder);
                    }
                    if (anchor == 1) { sb.Append(src); }
                }
                return sb.ToString();
            }
        }


        public static string ArrayToString<T>(this T[] list, Func<T, string> tostring, string split = ", ", string prefix = "", string suffix = "")
        {
            using (var auto = StringBuilderObjectPool.AllocAutoRelease())
            {
                var sb = auto.Output;
                for (int i = 0; i < list.Length; i++)
                {
                    T obj = list[i];
                    sb.Append(prefix + tostring(obj));
                    if (i < list.Length - 1)
                    {
                        sb.Append(split);
                    }
                    sb.Append(suffix);
                }
                return sb.ToString();
            }
        }
    }
}
