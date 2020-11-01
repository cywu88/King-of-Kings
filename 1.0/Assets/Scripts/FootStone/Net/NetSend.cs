using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace FootStone
{
    public class NetSend : NetBase
    {
        public NetSend(Socket sock, uint uSize)
            : base("send", sock, uSize)
        {
        }

        public override void Start()
        {
            m_stream = new StreamHelper(new byte[m_uBufferSize]);
            base.Start();
        }

        public override void Stop()
        {
            UnityEngine.Debug.Log("m_nSend.base.Stop()");
            base.Stop();
            {
                m_stream.Close();
                m_stream = null;
            }
        }

        public void SendMessage(ref BetterList<NetMessage> listMsg)
        {
            if (listMsg.Count == 0)
            {
                return;
            }
            bool bResume = false;
            Monitor.Enter(this);
            foreach (NetMessage msg in listMsg)
            {
                m_listMsg.Add(msg);
            }
            bResume = m_listMsg.Count > 0;
            Monitor.Exit(this);
            if (bResume)
            {
                Resume(true);
            }
        }

        protected override void SubWork()
        {
            //u sisiter 调式的时候这里无限呗调用。正常情况又不会被调了。我就说应该没问题啊。妈的你敢不敢出一个稳定的调式插件。
            if (null != m_socket && m_socket.Connected)
            {
                //如果还没有建立连接试着建立连接
                int iMsgNums = MakeBuffer(ref m_stream);
                if (iMsgNums > 0)
                {
                    try
                    {
                        m_socket.Send(m_stream.GetBuffer(), 0, m_stream.GetOffset(), SocketFlags.None);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                    m_stream.Reset();
                }
            }

            //发送完了挂起
            Suspend(true);
        }

        private int MakeBuffer(ref StreamHelper lf)
        {
            if (m_listMsg.Count > 0)
            {
                Monitor.Enter(this);
                foreach (NetMessage msg in m_listMsg)
                {
                    m_listDelegates.Add(msg);
                }
                m_listMsg.Clear();
                Monitor.Exit(this);
            }
            if (m_listDelegates.Count > 0)
            {
                return __MakeBuffer(ref lf);
            }
            return 0;
        }

        private int __MakeBuffer(ref StreamHelper lf)
        {
            int iFreeBuffer = (int)m_uBufferSize;
            int i = 0;
            for (; i < m_listDelegates.Count; i++)
            {
                m_listDelegates[i].MakeMessage();
                //不够了。下帧再发
                if (iFreeBuffer - m_listDelegates[i].m_uMsgLenght <= 0)
                {
                    //TODO:以后扩展为可以支持任意大小的缓冲
                    //Debug.LogWarning("消息缓冲区不够了。消息" + m_listDelegates[i].m_uID.ToString() + "隔帧发送");
                }
                else
                {
                    iFreeBuffer -= m_listDelegates[i].ToByte(ref lf);
                }
            }
            if (i < m_listDelegates.Count)
            {
                for (int j = 0; j < i; j++)
                {
                    m_listDelegates.RemoveAt(0);
                }
                //m_listDelegates.RemoveRange(0, i);
            }
            else
            {
                m_listDelegates.Clear();
            }
            return i;
        }

        public int m_uRunTime = 0;
        protected StreamHelper m_stream = null;
    }
}
