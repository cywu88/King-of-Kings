/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**Describe：  Socker发送消息流处理
********************************************************************/
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_WEBGL
using UnityEngine;
#endif

namespace FootStone
{
    public class SocketOutputStream
    {
        public const int DEFAULTSOCKETINPUTBUFFERSIZE = 65535;
        public SocketOutputStream(bool IsNeedCrypter = true)
        {
            m_steam = new StreamHelper(new byte[DEFAULTSOCKETINPUTBUFFERSIZE]);
            m_m_BufferLen = DEFAULTSOCKETINPUTBUFFERSIZE;
            m_IsNeedCrypter = IsNeedCrypter;
        }

        public void SetIsNeedCrypter(bool bNeed)
        {
            m_IsNeedCrypter = bNeed;
        }

        public int MakeBuffer(BetterList<NetMessage> lstMsg)
        {
            int iFreeBuffer = (int)m_m_BufferLen;
            int i = 0;
            for (; i < lstMsg.Count; i++)
            {
                //Debug.Log("合并消息：" + lstMsg[i].GetMessageId());
                //if (lstMsg[i].GetMsgId() == 116)
                //{
                //    BattleFtpHandler.Inst.UploadBytes("MakeBuffer : lstMsg[i].GetMsgId() == 116", "MakeBuffer_test_" + DateTime.Now.ToString("HHmmss"));
                //}

                lstMsg[i].MakeMessage();
                //不够了。下帧再发
                if (iFreeBuffer - lstMsg[i].m_uMsgLenght <= 0)
                {
                    //TODO:以后扩展为可以支持任意大小的缓冲
                    //Debug.LogWarning("消息缓冲区不够了。消息" + m_listDelegates[i].m_uID.ToString() + "隔帧发送");
                }
                else
                {
                    int nOffset = m_steam.GetOffset();
                    int nMsgLen = lstMsg[i].ToByte(m_steam);
                    int nWriteLen = m_steam.GetOffset() - nOffset;
                    if (nWriteLen != nMsgLen)
                    {
                        Debug.LogError(string.Format("{0}消息时 消息长度为{1} 实际发送的消息长度却是{2}", (NetMessageDefine)lstMsg[i].m_uID, nMsgLen, nWriteLen));
                    }
                    iFreeBuffer -= nMsgLen;

                    if (m_IsNeedCrypter)
                    {
                        lstMsg[i].Encrypt(m_steam);
                    }
                }
            }
            if (i < lstMsg.Count)
            {
                for (int j = 0; j < i ; j++)
                {
                    lstMsg.RemoveAt(0);
                }
                //lstMsg.RemoveRange(0, i);
            }
            else
            {
                lstMsg.Clear();
            }
            return i;
        }

        public void Close()
        {
            m_steam.Close();
        }

        private bool m_IsNeedCrypter = true;
        private int m_m_BufferLen;
        public StreamHelper m_steam;
    }
}