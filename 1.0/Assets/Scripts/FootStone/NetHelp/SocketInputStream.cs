/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**Describe： Socker接受消息流处理
********************************************************************/
using System;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_WEBGL
using UnityEngine;
#endif

namespace FootStone
{
    public class SocketInputStream
    {
        public const int DEFAULTSOCKETINPUTBUFFERSIZE = 65535;

        public SocketInputStream(bool IsNeedCrypter = true)
        {
            m_steam = new StreamHelper(new byte[DEFAULTSOCKETINPUTBUFFERSIZE]);
            m_m_BufferLen = DEFAULTSOCKETINPUTBUFFERSIZE;
            m_xxtea = new STREAM_KEY_XXTEA();
            m_xxtea.Init();
            m_IsNeedCrypter = IsNeedCrypter;
        }

        public void SetIsNeedCrypter(bool bNeed)
        {
            m_IsNeedCrypter = bNeed;
        }

 
        //填充数据
        public bool Fill(byte[] szData)
        {
            int recLen = szData.Length;
            int nFree = 0;

            if (m_Head <= m_Tail)
            {
                //
                // H   T		LEN=10
                // 0123456789
                // abcd......
                //
                nFree = m_m_BufferLen - m_Tail;
                if (nFree >= recLen)
                {
                    //直接填充数据
                    m_steam.Seek(m_Tail); //尾部写入数据
                    m_steam.Write(ref szData, recLen);
                    m_Tail += recLen;
                }
                else
                {
                    if (!ReSize(recLen + 1))
                    {
                        return false;
                    }
                    m_steam.Write(ref szData, recLen);
                    m_Tail += recLen;
                }
            }
            else
            {
                //这种清空应该不会出现
                Debug.LogError(string.Format("socket通信出现错误！！{0},{1}", m_Head, m_Tail));
                //
                //     T  H		LEN=10
                // 0123456789
                // abcd...efg
                //
            }
            return true;
        }
        /// <summary>
        /// m_socket.BeginReceive获取的数据保存在socketStream流里面
        /// 将socketStream流里面的数据转移到我们的数据存储区
        /// </summary>
        /// <param name="stream">接收到的数据</param>
        /// <param name="recLen">接收到的数据长度</param>
        /// <returns></returns>
        public bool Fill(SocketStream stream, int recLen)
        {
            int nFree = 0;

            if (m_Head <= m_Tail)
            {
                //
                // H   T		LEN=10
                // 0123456789
                // abcd......
                //
                nFree = m_m_BufferLen - m_Tail;
                if (nFree >= recLen)
                {
                    //直接填充数据
                    m_steam.Seek(m_Tail); //尾部写入数据
                    m_steam.Write(ref stream.m_bytes, recLen);
                    m_Tail += recLen;
                }
                else
                {
                    if (!ReSize(recLen + 1))
                    {
                        return false;
                    }
                    m_steam.Write(ref stream.m_bytes, recLen);
                    m_Tail += recLen;
                }
            }
            else
            {
                //这种清空应该不会出现
                Debug.LogError(string.Format("socket通信出现错误！！{0},{1}", m_Head, m_Tail));
                //
                //     T  H		LEN=10
                // 0123456789
                // abcd...efg
                //
            }
            return true;
        }
        /// <summary>
        /// 重新分配存储空间
        /// </summary>
        /// <param name="size">将数据的存储空间夸大到m_mbufflen+len</param>
        /// <returns></returns>
        public bool ReSize(int size)
        {
            size = Math.Max(size, m_m_BufferLen >> 1);
            int newBufferLen = m_m_BufferLen + size;
            int len = Length();
            StreamHelper newBuffer = new StreamHelper(new byte[newBufferLen]);
            //将之前已经处理的数据抛弃掉
            newBuffer.GetStream().Write(m_steam.GetBuffer(), m_Head, len);        
            m_steam.Close();
            m_steam = newBuffer;
            m_m_BufferLen = newBufferLen;
            m_Tail = len;
            m_Head = 0;

            return true;
        }
        int Length()
        {
            if (m_Head < m_Tail)
                return m_Tail - m_Head;
            return 0;
        }
        public void Close()
        {
            m_steam.Close();
        }
        private int m_Head; //消息处理头
        private int m_Tail; //消息处理尾
        private int m_m_BufferLen; //消息处理长度
        private STREAM_KEY_XXTEA m_xxtea; //加密处理
        public StreamHelper m_steam; //消息存储流
        private bool m_IsNeedCrypter = true;
    }
}