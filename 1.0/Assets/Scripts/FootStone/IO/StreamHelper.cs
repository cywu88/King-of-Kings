/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**Describe：  
********************************************************************/

using System;
using System.IO;
using UnityEngine;
using System.Text;

namespace FootStone 
{ 
    public class StreamHelper
    {
        public static byte[] NewByte(int len)
        {
            return new byte[len];
        }

        //纯粹流式
        public StreamHelper(Stream stream)
        {
            m_stream = stream;
        }

        public StreamHelper(int bufferSize)
        {
            m_byte = new byte[bufferSize];
            m_stream = new MemoryStream(m_byte);
        }

        public StreamHelper(byte[] udata)
        {
            m_byte = udata;
            m_stream = new MemoryStream(udata);
        }

        public Stream GetStream()
        {
            return m_stream;
        }

        public void Close()
        {
            if (null != m_stream)
            {
                m_stream.Close();
            }
        }

        public byte[] GetBuffer()
        {
            return m_byte;
        }

        public void Seek(int iOff)
        {
            if (iOff < m_stream.Length)
            {
                m_stream.Position = iOff;
            }
        }

        public long Lenght()
        {
            return null != m_stream ? m_stream.Length : null != m_byte ? m_byte.Length : 0;
        }

        public int GetOffset()
        {
            if (m_stream != null)
                return (int)m_stream.Position;
            else
                return 0;

        }


        public void Reset()
        {
            m_stream.Position = 0;
            if (null != m_byte)
            {
                Array.Clear(m_byte, 0, m_byte.Length);
            }
        }

        public int Read(ref byte[] uDate)
        {
            if (m_stream == null || null == uDate)
            {
                return -1;
            }
            int iRef = m_stream.Read(uDate, 0, uDate.Length);
            return iRef;
        }

        public void Write(ref byte[] uDate)
        {
            if (m_stream == null || null == uDate)
            {
                return;
            }

            m_stream.Write(uDate, 0, uDate.Length);
        }

        public void Write(ref byte[] uDate, int len)
        {
            if (m_stream == null || null == uDate)
            {
                return;
            }

            m_stream.Write(uDate, 0, len);
        }

        private byte[] byteBit;
        public void WriteByte(byte uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            if (byteBit == null)
                byteBit = new byte[StringHelper.s_ByteSize];

            byteBit[0] = uDate;
            m_stream.Write(byteBit, 0, StringHelper.s_ByteSize);
        }

        public int ReadByte(ref byte uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (byteBit == null)
                byteBit = new byte[StringHelper.s_ByteSize];

            int iRef = m_stream.Read(byteBit, 0, StringHelper.s_ByteSize);
            uDate = byteBit[0];
            return iRef;
        }

        private byte[] intBit;
        public int ReadInt(ref int uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (intBit == null)
                intBit = new byte[StringHelper.s_IntSize];

            int iRef = m_stream.Read(intBit, 0, StringHelper.s_IntSize);
            uDate = BitConverter.ToInt32(intBit, 0);
            return iRef;
        }
        public int ReadInt()
        {
            if (intBit == null)
                intBit = new byte[StringHelper.s_IntSize];

            m_stream.Read(intBit, 0, StringHelper.s_IntSize);
            return BitConverter.ToInt32(intBit, 0);
        }

        public void WriteInt(int uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_IntSize);
        }

        public int ReadUInt(ref uint uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (intBit == null)
                intBit = new byte[StringHelper.s_IntSize];

            int iRef = m_stream.Read(intBit, 0, StringHelper.s_IntSize);
            uDate = BitConverter.ToUInt32(intBit, 0);
            return iRef;
        }

        public uint ReadUInt()
        {
            if (m_stream == null)
            {
                return 0;
            }
            if (intBit == null)
                intBit = new byte[StringHelper.s_IntSize];

            m_stream.Read(intBit, 0, StringHelper.s_IntSize);
            return BitConverter.ToUInt32(intBit, 0);
        }

        public void WriteUInt(uint uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_IntSize);
        }

        private byte[] shortBit;
        public int ReadUShort(ref ushort uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (shortBit == null)
                shortBit = new byte[StringHelper.s_ShortSize];

            int iRef = m_stream.Read(shortBit, 0, StringHelper.s_ShortSize);
            uDate = BitConverter.ToUInt16(shortBit, 0);
            return iRef;
        }

        public ushort ReadUShort()
        {
            if (m_stream == null)
            {
                return 0;
            }
            if (shortBit == null)
                shortBit = new byte[StringHelper.s_ShortSize];

            m_stream.Read(shortBit, 0, StringHelper.s_ShortSize);
            return BitConverter.ToUInt16(shortBit, 0);
        }

        public void WriteUShort(ushort uDate)
        {
            if (m_stream == null)
            {
                return;
            }

            m_stream.Write(BitConverter.GetBytes(uDate), 0, StringHelper.s_ShortSize);
        }

        private byte[] floatBit;
        public float ReadFloat()
        {
            if (m_stream == null)
            {
                return 0.0f;
            }
            if (floatBit == null)
                floatBit = new byte[StringHelper.s_FloatSize];

            var iRef = m_stream.Read(floatBit, 0, StringHelper.s_FloatSize);
            var fData = BitConverter.ToSingle(floatBit, 0);
            return fData;
        }

        public int ReadFloat(ref float uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (floatBit == null)
                floatBit = new byte[StringHelper.s_FloatSize];

            int iRef = m_stream.Read(floatBit, 0, StringHelper.s_FloatSize);
            uDate = BitConverter.ToSingle(floatBit, 0);
            return iRef;
        }

        public void WriteFloat(float uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_FloatSize);
        }

        public int ReadLong(ref long uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            byte[] bit = new byte[StringHelper.s_LongSize];
            int iRef = m_stream.Read(bit, 0, StringHelper.s_LongSize);
            uDate = BitConverter.ToInt64(bit, 0);
            return iRef;
        }

        public long ReadLong()
        {
            if (m_stream == null)
            {
                return -1;
            }
            byte[] bit = new byte[StringHelper.s_LongSize];
            m_stream.Read(bit, 0, StringHelper.s_LongSize);
            return BitConverter.ToInt64(bit, 0);
        }

        public int GetRest()
        {
            if (null == m_stream)
            {
                return -1;
            }
            return (int)m_stream.Length - (int)m_stream.Position;
        }

        public void WriteLong(long uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_LongSize);
        }

        public void WriteULong(ulong uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_LongSize);
        }

        public ulong ReadULong()
        {
            if (m_stream == null)
            {
                return 0;
            }
            byte[] bit = new byte[StringHelper.s_LongSize];
            m_stream.Read(bit, 0, StringHelper.s_LongSize);
            return BitConverter.ToUInt64(bit, 0);
        }

        private byte[] boolBit;
        public int ReadBool(ref bool uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            if (boolBit == null)
                boolBit = new byte[StringHelper.s_BoolSize];

            int iRef = m_stream.Read(boolBit, 0, StringHelper.s_BoolSize);
            uDate = BitConverter.ToBoolean(boolBit, 0);
            return iRef;
        }

        public bool ReadBool()
        {
            if (m_stream == null)
            {
                return false;
            }
            if (boolBit == null)
                boolBit = new byte[StringHelper.s_BoolSize];

            m_stream.Read(boolBit, 0, StringHelper.s_BoolSize);
            return BitConverter.ToBoolean(boolBit, 0);
        }

        public void WriteBool(bool uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_BoolSize);
        }

        public void WriteVector2(float x, float y)
        {
            WriteFloat(x);
            WriteFloat(y);
        }

        public int ReadVector2(ref float x, ref float y)
        {
            int iRef = ReadFloat(ref x);
            iRef = ReadFloat(ref y);
            return iRef;
        }

        public void WriteVector3(float x, float y, float z)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
        }

        public int ReadVector3(ref float x, ref float y, ref float z)
        {
            int iRef = ReadFloat(ref x);
            iRef = ReadFloat(ref y);
            iRef = ReadFloat(ref z);
            return iRef;
        }

        //#if UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER
        public void ReadColor(ref Color col)
        {
            ReadFloat(ref col.r);
            ReadFloat(ref col.g);
            ReadFloat(ref col.b);
            ReadFloat(ref col.a);
        }

        public void WriteColor(ref Color col)
        {
            WriteFloat(col.r);
            WriteFloat(col.g);
            WriteFloat(col.b);
            WriteFloat(col.a);
        }

        public void WriteVector2(ref Vector2 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
        }

        public int ReadVector2(ref Vector2 vec)
        {
            int iRef = ReadFloat(ref vec.x);
            iRef = ReadFloat(ref vec.y);
            return iRef;
        }

        public void WriteVector3(ref Vector3 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
        }

        public int ReadVector3(ref Vector3 vec)
        {
            int iRef = ReadFloat(ref vec.x);
            iRef = ReadFloat(ref vec.y);
            iRef = ReadFloat(ref vec.z);
            return iRef;
        }

        public void ReadVector4(ref Vector4 vec)
        {
            ReadFloat(ref vec.x);
            ReadFloat(ref vec.y);
            ReadFloat(ref vec.z);
            ReadFloat(ref vec.w);
        }

        public void WriteVector4(ref Vector4 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
            WriteFloat(vec.w);
        }

        public void ReadQuaternion(ref Quaternion quat)
        {
            ReadFloat(ref quat.x);
            ReadFloat(ref quat.y);
            ReadFloat(ref quat.z);
            ReadFloat(ref quat.w);
        }

        public void WriteQuaternion(ref Quaternion quat)
        {
            WriteFloat(quat.x);
            WriteFloat(quat.y);
            WriteFloat(quat.z);
            WriteFloat(quat.w);
        }
        //#endif
        public void WriteFloat4(float x, float y, float z, float w)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
            WriteFloat(w);
        }

        public int ReadFloat4(ref float x, ref float y, ref float z, ref float w)
        {
            int iRef = ReadFloat(ref x);
            iRef = ReadFloat(ref y);
            iRef = ReadFloat(ref z);
            iRef = ReadFloat(ref w);
            return iRef;
        }


        public int ReadDouble(ref double uDate)
        {
            if (m_stream == null)
            {
                return -1;
            }
            byte[] bit = new byte[StringHelper.s_DoubleSize];
            int iRef = m_stream.Read(bit, 0, StringHelper.s_DoubleSize);
            uDate = BitConverter.ToDouble(bit, 0);
            return iRef;
        }

        public void WriteDouble(double uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, StringHelper.s_DoubleSize);
        }

        public bool ReadString(out string strOut)
        {
            strOut = null;
            if (null == m_stream)
            {
                return false;
            }
            int iLenght = 0;
            ReadInt(ref iLenght);
            if (iLenght <= 0 && iLenght > 1024 * 1024 * 1024)
            {
                strOut = null;
                return false;
            }
            byte[] uData = new byte[iLenght];
            Read(ref uData);
            //读出来的都是unicode
            strOut = Encoding.UTF8.GetString(uData);
            //strOut = StringHelper.UTF82Unicode(ref uData);
            return true;
        }

        public bool ReadString(out string strOut, int size)
        {
            strOut = null;
            if (null == m_stream)
            {
                return false;
            }
            byte[] uData = new byte[size];
            Read(ref uData);
            //读出来的都是unicode
            strOut = Encoding.UTF8.GetString(uData);
            return true;
        }

        public static uint StringInFileSize(ref string strOut)
        {
            if (string.IsNullOrEmpty(strOut))
            {
                return (uint)StringHelper.s_UIntSize;
            }
            byte[] uData = Encoding.UTF8.GetBytes(strOut);
            return (uint)StringHelper.s_UIntSize + (uint)uData.Length;
        }

        public bool WriteString(string strOut)
        {
            if (m_stream == null)
            {
                return false;
            }

            byte[] uData = null;
            int iLenght = 0;
            if (!string.IsNullOrEmpty(strOut))
            {
                uData = Encoding.UTF8.GetBytes(strOut);
                iLenght = uData.Length;
                WriteInt(iLenght);
                Write(ref uData);
            }
            else
            {
                WriteInt(iLenght);
            }
            return true;
        }

        protected Stream m_stream = null;
        public byte[] m_byte = null;
        // 外部设置当前流的字节长度
        public int m_byteLen;
    }
}