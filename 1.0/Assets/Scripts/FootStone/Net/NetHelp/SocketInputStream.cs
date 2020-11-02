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



        /// <summary>
        /// 将获取到btye数据转换成为对应的消息，交给逻辑处理
        /// </summary>
        /// <param name="lstMsg"></param>
        public void GenMsg(ref BetterList<NetMessage> lstMsg)
        {
            //int iMinSize = StringHelper.s_IntSize + StringHelper.s_ShortSize;
            //基本流读完了
            bool bRun = true;
            while (bRun)
            {
                if (!__GenMsg(lstMsg))
                {
                    break;
                }
            }
            m_Head = m_steam.GetOffset();
        }
        /// <summary>
        /// 解密处理，将消息的字节进行转换，转换为对应的netMessage消息类
        /// 服务器一条对应的消息，对应一个类。
        /// 通过： 主消息key + 二级消息key + 三级消息key进行解释
        /// 比如： 生物模块 + 宠物模块 + 宠物技能升级
        /// 警告：这里最重要的处理在于如果消息获取的不是一个完成的消息，将保留消息，等到下次读取完之后处理
        /// </summary>
        /// <param name="lstMsg">lst引用，将读取的netMessage保留到lstmsg里面</param>
        /// <returns>是否继续读取数据</returns>
        private bool __GenMsg(BetterList<NetMessage> lstMsg)
        {
            //如果不够读出一份完整数据。回读
            int iMinSize = StringHelper.s_ShortSize;
            m_steam.Seek(m_Head); //头部读数据
            int nNeedReadBuffer = m_Tail - m_Head;

            ushort uLen = 0;
            if (m_IsNeedCrypter)
            {
                if (nNeedReadBuffer < iMinSize + STREAM_KEY_XXTEA.GetSize())
                {
                    return false; ;
                }

                uLen = m_steam.ReadUShort();

                for (int i = 0; i < 4; i++)
                {
                    m_xxtea.key[i] = m_steam.ReadUInt();
                }
                m_steam.ReadByte(ref m_xxtea.byCrc);
                m_steam.ReadByte(ref m_xxtea.byFlag);
                m_steam.ReadUShort(ref m_xxtea.wLen);

                //等待组包
                if (nNeedReadBuffer < m_xxtea.wLen + STREAM_KEY_XXTEA.GetSize() + 2)
                {
                    m_steam.Seek(m_Head);
                    return false;
                }

                //解密数据
                MEncrypter.Encrypt(ref m_steam.m_byte, (int)(m_Head + STREAM_KEY_XXTEA.GetSize() + 2), m_xxtea.wLen, ref m_xxtea.key, false);
            }
            else
            {
                if (nNeedReadBuffer < iMinSize)
                {
                    return false; ;
                }

                uLen = m_steam.ReadUShort();

                //等待组包
                if (nNeedReadBuffer < uLen + 2)
                {
                    m_steam.Seek(m_Head);
                    return false;
                }
            }


            ushort uId = m_steam.ReadUShort();


            //一级消息解释。获取对应的cmdRootBase类
            CmdRootBase cmdRoot = CmdRootSinkManager.Instance().getCmdRootById(uId);
            if (cmdRoot == null)
            {
                Debug.LogError(string.Format("找不到root{0}对应的解析消息", uId));
                return false;
            }
            //在对应的cmdRootbase进行解析，获取对应的类,返回对应的类
            NetMessage msg = cmdRoot.ByteToNetMessage(ref m_steam);

            if (null == msg)
            {
                //放弃这条消息
                m_Head = m_Head + uLen + 2;
                m_steam.Seek(m_Head);
                Debug.LogWarning(string.Format("未知的消息:{0}", uId));
                return true;
            }

            if (m_IsNeedCrypter)
                msg.m_uDataLenght = (ushort)(uLen - STREAM_KEY_XXTEA.GetSize());
            else
                msg.m_uDataLenght = (ushort)uLen;

            try
            {
                //类里面进行对应的字节解释，将服务器数据保存到相应的类数据里面
                if (!msg.FromByte(ref m_steam))
                {
                    //放弃这条消息
                    m_Head = m_Head + uLen + 2;
                    m_steam.Seek(m_Head);
                    Debug.LogWarning("消息的字节处理出现了问题" + msg.GetType().Name);
                    return true; //
                }
            }
            catch (System.Exception ex)
            {
                string strErrorMsg = string.Format("{0}消息解析异常！！！！！！{1}", (NetMessageDefine)msg.m_uID, ex.Message);
                Debug.LogError(strErrorMsg);
                Debug.LogError(string.Format("{0}", ex.StackTrace));
            }


            if (m_steam.GetOffset() != uLen + m_Head + 2)
            {
                Debug.LogWarning("消息的字节处理出现了问题" + msg.GetType().Name + String.Format("应读取{0}byte，实际读取{1}byte", uLen, m_steam.GetOffset() - m_Head - 2));
                m_Head = m_Head + uLen + 2;
                m_steam.Seek(m_Head);
                return true;
            }


            if (msg.IsFspMessage())
            {
                try
                {
                    GameManager.Instance.AddServerFrameMsg(msg);
                }
                catch (System.Exception ex)
                {
                    string strErrorMsg = string.Format("AddServerFrameMsg ERROR  id={0} error={1}！！！！！！", (NetMessageDefine)msg.m_uID, ex.Message);
                    Debug.LogError(strErrorMsg);
                }
            }
            else
            {
                if (msg.IsRightAway())
                {
                    msg.OnRecv();
                }
                else
                {
                    //将类保存到lstmsg列表，之后统一处理
                    lstMsg.Add(msg);
                }
            }


            //继续解释下一个类消息
            m_Head = m_steam.GetOffset();
            return true;
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