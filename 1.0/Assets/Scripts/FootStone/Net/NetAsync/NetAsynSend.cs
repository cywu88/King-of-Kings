using UnityEngine;
using System;
using System.Net.Sockets;
 
namespace FootStone
{
    public class NetAsynSend : NetAsynBase
    {
        public NetAsynSend(Socket sock)
            : base(sock)
        {
        }
        //public NetAsynSend(WebSocket sock, uint uSize)
        //    : base(sock)
        //{
        //}
		
		public override void Start()
        {
            m_outPutStream = new SocketOutputStream();
            base.Start();
        }


        public void SetIsNeedCrypter(bool bNeed)
        {
            m_outPutStream.SetIsNeedCrypter(bNeed);
        }


        public override void Stop()
        {
            base.Stop();
            if (null != m_outPutStream)
			{
                m_outPutStream.Close();
                m_outPutStream = null;
			}
        }

        public bool SendMessage(ref BetterList<NetMessage> listMsg)
        {
            if (listMsg.Count == 0)
            {
                return false;
            }
            var e = listMsg.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    m_listMsg.Add(e.Current);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
            }
            finally
            {
                e.Dispose();
            }
            SendMessage();
            return true;
        }

		//外部不准调用
        private void SendMessage()
        {
            if (m_bSending)
            {
                //下帧在发送
                return;
            }

            //有消息才需要发送
            if (m_listMsg.Count > 0)
            {
                SendToSever();
            }
        }
		
		public override void WaitSocket()
		{
			if(null != m_ar && m_bSending)
			{
				m_socket.EndSend(m_ar);
			}
		}

        //发送一帧的数据到服务器。
        void SendToSever()
        {
            m_bSending = true;
              
            {
                //如果还没有建立连接试着建立连接
                int iMsgNums = m_outPutStream.MakeBuffer(m_listMsg);
                if (iMsgNums > 0)
                {
                    try
                    {
                        if (m_socket != null)
                        {
                            //Debug.Log("最终发送消息");
                            m_socket.BeginSend(m_outPutStream.m_steam.GetBuffer(), 0, m_outPutStream.m_steam.GetOffset(), SocketFlags.None, new AsyncCallback(SendedEnd), null);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.Message);
                        SendedEnd(false);
                    }
                }
            }
        }

        private void SendedEnd(bool bResult)
        {
            m_bSending = false;
            m_ar = null;
            m_outPutStream.m_steam.Reset();
        }

        private void SendedEnd(IAsyncResult ar)
        {
            m_socket.EndSend(ar);
            m_bSending = false;
            m_ar = null;
            m_outPutStream.m_steam.Reset();
        }

        public void SetIsFsp(bool bIsFsp)
        {
            m_bFsp = bIsFsp;
        }


        protected bool m_bFsp = false;
        protected bool m_bSending;
        protected SocketOutputStream m_outPutStream = null;
    }
}
