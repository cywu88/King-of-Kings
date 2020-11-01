using System.Text;
using System.Security.Cryptography;

namespace FootStone
{
    public class StringHelper
    {
        public static int s_IntSize = 4;
        public static int s_UIntSize = 4;
        public static int s_LongSize = 8;
        public static int s_FloatSize = 4;
        public static int s_Float2Size = 8;
        public static int s_Float3Size = 12;
        public static int s_Float4Size = 16;
        public static int s_DoubleSize = 8;
        public static int s_CharSize = 1;
        public static int s_ByteSize = 1;
        public static int s_BoolSize = 1;
        public static int s_ShortSize = 2;
        //去除一个字符串里面的所有/n/t/r空格
        //返回一定清除了几个
        //参数2 是否删除 /r
        //参数3 是否删除 /t
        //参数4 是否删除 空格
        //参数5 是否删除 /n
        public static void Strip(ref string strOut, bool bR, bool bT, bool bSpace, bool bN, bool bLK, bool bRK, bool bC)
        {
            if (bR)
            {
                strOut.Replace("\r", "");
            }
            if (bT)
            {
                strOut.Replace("\t", "");
            }
            if (bSpace)
            {
                strOut.Replace(" ", "");
            }
            if (bN)
            {
                strOut.Replace("\n", "");
            }
            if (bLK)
            {
                strOut.Replace("<", "");
            }
            if (bRK)
            {
                strOut.Replace(">", "");
            }
            if (bC)
            {
                strOut.Replace("\"", "");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void StandardPath(ref string strPath)
        {
            if (strPath == null && strPath.Length == 0)
            {
                return;
            }
            strPath.Replace("//", "\\");
            strPath.Replace("/", "\\");
            //如果最后没有\\同时又没有.那么这个路径还不完整在拼接一个
            if (strPath[(strPath.Length - 1)] != '\\')
            {
                int iLastDot = strPath.LastIndexOf('.');
                //找不到点或者点是第一个。
                if (iLastDot <= 0)
                {
                    //找不到点肯定不是全路径了
                    strPath += "\\";
                }
            }
            strPath.ToLower();
        }

        public static string GetUnicodeStringByBytes(ref byte[] by)
        {
            return Encoding.Unicode.GetString(by);
        }

        public static string GetUTF8StringByBytes(ref byte[] byArray)
        {
            string str = Encoding.UTF8.GetString(byArray);
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static string GetString(string str)
        {
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static string GetUTF8StringByBytes(byte[] byArray)
        {
            string str = Encoding.UTF8.GetString(byArray);
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static byte[] GetBytesByStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Encoding.UTF8.GetBytes("");
            }
            return Encoding.UTF8.GetBytes(str.Trim());
        }

        /// <summary>
        /// 根据preBytes（取它的长度）直接给字节数组赋值
        /// </summary>
        public static void GetBytesByBytes(ref byte[] lastBytes, ref byte[] preBytes)
        {
            for (int i = 0; i < preBytes.Length; ++i)
            {
                lastBytes[i] = preBytes[i];
            }
        }

        public static byte[] GetMD5(ref byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        public static byte[] GetMD5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }
    }
}