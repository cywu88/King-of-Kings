using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// 返回书写习惯的类型字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToTypeDefineName(this Type type)
        {
            var pname = type.ToPrimitiveName();
            if (pname != null) { return pname; }
            if (type.IsGenericType)
            {
                using (var auto = StringBuilderObjectPool.AllocAutoRelease())
                {
                    var sb = auto.Output;
                    int tidx = type.Name.IndexOf('`');
                    if (tidx > 0)
                    {
                        sb.Append(type.Name.Substring(0, tidx));
                    }
                    else
                    {
                        sb.Append(type.Name);
                    }
                    sb.Append("<");
                    Type[] g_args = type.GetGenericArguments();
                    for (int i = 0; i < g_args.Length; i++)
                    {
                        sb.Append(ToTypeDefineName(g_args[i]));
                        if (i < g_args.Length - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                    sb.Append(">");
                    return sb.ToString();
                }
            }
            return type.Name;
        }

        public static string ToPrimitiveName(this Type type)
        {
            if (type == typeof(object)) return "object";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(char)) return "char";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "double";
            if (type == typeof(short)) return "short";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(float)) return "float";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(void)) return "void";
            return null;
        }

    }
}
