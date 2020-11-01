/******************************************************************
**Authro: foolyoo	
**日期:   2020年11月1日 
**描述：  消息处理基类
********************************************************************/
using System.Text;
namespace FootStone
{
    public class NetMessage
    {
        public bool m_NeedCrypter = true;
        public static int s_messageId = 0;
        public NetMessage(uint uID, ushort uDataLen)
        {
            m_uID = uID;
            m_uDataLenght = uDataLen;
            s_messageId++;
        }

        public void SetNeedCrypter(bool IsNeedCrypter)
        {
            m_NeedCrypter = IsNeedCrypter;
        }

        public int GetMessageId()
        {
            return s_messageId;
        }

        /// <summary>
        /// 是否立刻处理
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRightAway()
        {
            return false;
        }

        /// <summary>
        /// 是否是桢消息,桢消息放入桢缓存队列中进行处理
        /// </summary>
        /// <returns></returns>
        public virtual bool IsFspMessage()
        {
            return false;
        }

        /// <summary>
        /// 用于回放系统的消息
        /// </summary>
        public bool IsReplayMessage { get; set; }
        
        /// <summary>
        /// 获取msgId对应的值
        /// </summary>
        /// <returns></returns>
        public uint GetMsgId()
        {
            return m_uID;
        }

        #region  发送消息消息需要继承的接口
        /// <summary>
        /// 发送的总长度
        /// </summary>
        public virtual void MakeMessage()
        {
            //uId长度不进行计算
            if (m_NeedCrypter)
            {
                m_uMsgLenght = StringHelper.s_ShortSize +  (int)STREAM_KEY_XXTEA.GetSize() + m_uDataLenght; 
            }
            else
            {
                m_uMsgLenght = StringHelper.s_ShortSize + m_uDataLenght;
            }
            
        }

        /// <summary>
        /// 写长度
        /// </summary>
        /// <param name="lf">写字符流</param>
        /// <returns>写的长度</returns>
        public virtual int ToByte(StreamHelper lf)
        {
            //系统回放消息只保存内容
            if(IsReplayMessage)
            {
                return 0;
            }

            //需要加密
            if (m_NeedCrypter)
            {
                lf.WriteUShort((ushort)(m_uDataLenght + STREAM_KEY_XXTEA.GetSize()));
                // 随机生成密钥;
                MEncrypter.S_Key_XXTEA.Init();
                MEncrypter.RandomStreamXXTeaKeyArray();
                MEncrypter.S_Key_XXTEA.key[0] = (uint)MEncrypter.c2sXXTeaKeyArray[0];
                MEncrypter.S_Key_XXTEA.key[1] = (uint)MEncrypter.c2sXXTeaKeyArray[1];
                MEncrypter.S_Key_XXTEA.key[2] = (uint)MEncrypter.c2sXXTeaKeyArray[2];
                MEncrypter.S_Key_XXTEA.key[3] = (uint)MEncrypter.c2sXXTeaKeyArray[3];
                MEncrypter.S_Key_XXTEA.wLen =(ushort) m_uDataLenght;

                if (MEncrypter.m_byLastSendFlag == 255)
                    MEncrypter.m_byLastSendFlag = 0;
                else
                    MEncrypter.m_byLastSendFlag++;

                MEncrypter.S_Key_XXTEA.byFlag = MEncrypter.m_byLastSendFlag;
                MEncrypter.S_Key_XXTEA.byCrc = 1;
                // 循环校验
                //if( ENABLE_CRC_CHECK )
                //{
                //    tempKey.byCrc = CalculateCRC( inBuffer , inLen );
                //}
                //写入加密
                //写入4个key
                for (int i = 0; i < 4; ++i)
                {
                    lf.WriteUInt(MEncrypter.S_Key_XXTEA.key[i]);
                }
                lf.WriteByte(MEncrypter.S_Key_XXTEA.byCrc);
                lf.WriteByte(MEncrypter.S_Key_XXTEA.byFlag);
                lf.WriteUShort(MEncrypter.S_Key_XXTEA.wLen);
            }
            else
            {
                lf.WriteUShort(m_uDataLenght);
            }

            //如果是桢消息，需要发送指令解析Id
            if (IsFspMessage())
            {
                lf.WriteUShort(DMsgKeyRoot.CMD_ROOT_FSP);
                lf.WriteUShort((ushort)m_uID);
            }
            return (int)m_uMsgLenght;
        }

        /// <summary>
        /// 进行加密
        /// </summary>
        /// <param name="lf">写字符流</param>
        /// <returns>写的长度</returns>
        public virtual int Encrypt(StreamHelper lf)
        {
            //需要加密
            if (m_NeedCrypter)
            {
                MEncrypter.Encrypt(ref lf.m_byte, lf.GetOffset() - m_uDataLenght, m_uDataLenght, ref MEncrypter.c2sXXTeaKeyArray, true);
            }
            return (int)m_uMsgLenght;
        }
        #endregion

        #region 接受消息需要继承的接口
        /// <summary>
        /// 转换字节
        /// </summary>
        /// <param name="lf"></param>
        /// <returns></returns>
        public virtual bool FromByte(StreamHelper lf)
        {
            return false;
        }
        /// <summary>
        /// 处理消息回调
        /// </summary>
        /// <returns></returns>
        public virtual bool OnRecv()
        {
            return false;
        }

        /// <summary>
        /// 逻辑处理
        /// </summary>
        /// <returns></returns>
        public virtual bool OnRecv(bool isLoigc)
        {
            return false;
        }
        #endregion

        public virtual void DebugMessage(out string strDebug)
        {
            StringBuilder strHelp = new StringBuilder();
            strHelp.AppendFormat("消息ID--%d, 消息数据长--%d, 消息数据总长--%d", m_uID, m_uDataLenght, m_uMsgLenght);
            strDebug = strHelp.ToString();
        }

        public uint m_uID;
        public ushort m_uDataLenght;
        public int m_uMsgLenght;
    }
}
