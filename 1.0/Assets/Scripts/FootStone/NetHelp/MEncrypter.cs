using System;
using System.Runtime.InteropServices;

namespace FootStone
{
    public class MEncrypter
    {
        public static byte m_byLastSendFlag = 0;
        public static STREAM_KEY_XXTEA S_Key_XXTEA =new STREAM_KEY_XXTEA();
      
        private const uint DELTA = 0x9e3779b9;

        public static bool Encrypt(ref byte[] inOut, int iStartIndex, int iLength, ref uint[] k, bool bIsEncrypt)
        {

            int n = iLength / 4;
            if (n < 2)
            {
                //这个加密函数不支持小于8个字节的加密数据
                return true;
            }

            uint[] v = GetUintArray(inOut, iLength, iStartIndex);
            uint y = v[0], sum = 0, e, z = v[n - 1];
            int p, q;
            if (bIsEncrypt)
            {
                q = 6 + 52 / n;
                while (q-- > 0)
                {
                    sum += DELTA;
                    e = (sum >> 2) & 3;
                    for (p = 0; p < n - 1; p++)
                    {
                        y = v[p + 1];
                        z = v[p] += ((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                    }
                    y = v[0];
                    z = v[n - 1] += ((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                }

                Put(ref v, ref inOut, iStartIndex);
                return true;
            }
            else if (!bIsEncrypt)
            {
                q = 6 + 52 / n;
                sum = (uint)(q * DELTA);
                while (sum != 0)
                {
                    e = (sum >> 2) & 3;
                    for (p = n - 1; p > 0; p--)
                    {
                        z = v[p - 1];
                        y = v[p] -= ((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                    }
                    z = v[n - 1];
                    y = v[0] -= ((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                    sum -= DELTA;
                }

                Put(ref v, ref inOut, iStartIndex);
                return true;
            }

            return false;
        }

        public static uint[] GetUintArray(byte[] cmd, int length, int indexer)
        {
            int sizeOfUint = sizeof(uint);
            int iCount = length / sizeOfUint;
            uint[] destinationArray = new uint[iCount];

            for (int i = 0; i < iCount; ++i, indexer += sizeOfUint)
            {
                destinationArray[i] = BitConverter.ToUInt32(cmd, indexer);
            }

            return destinationArray;
        }

        public static void Put(ref uint[] newValue, ref byte[] buffer, int iStartIndex)
        {
            int sizeOfUint = sizeof(uint);
            for (int i = 0; i < newValue.Length; ++i, iStartIndex += sizeOfUint)
            {
                Array.Copy(BitConverter.GetBytes(newValue[i]), 0, buffer, iStartIndex, sizeOfUint);
            }
        }

        public static uint[] c2sXXTeaKeyArray = new uint[4];
        public static System.Random s_c2sXXTeaKeyArrayRandom_ = new System.Random((int)System.DateTime.Now.Ticks);
        //private static STREAM_KEY_XXTEA m_messageHead;
        // 随机生成密钥
        public static void RandomStreamXXTeaKeyArray()
        {
            for (int i = 0; i < 4; ++i)
            {
                c2sXXTeaKeyArray[i] = (uint)s_c2sXXTeaKeyArrayRandom_.Next();

                //Debug.Log("c2sXXTeaKeyArray[" + i + "] =" + c2sXXTeaKeyArray[i]);
            }
        }

    }
    public struct STREAM_KEY_XXTEA
    {
        [MarshalAs(UnmanagedType.SysUInt, SizeConst = 4)]
        public uint[] key;		   // 加密的key
        public byte byCrc;         // 当前位置
        public byte byFlag;		  // 递增标识
        public ushort wLen;		  // 流数据长度

        public void Init()
        {
            if (key == null)
            {
                key = new UInt32[4];
            }
        }

        public static uint GetSize()
        {
            return 20;
        }
    }
}

