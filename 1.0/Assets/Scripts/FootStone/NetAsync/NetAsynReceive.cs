using UnityEngine;
using System;
using System.Net.Sockets;

namespace FootStone
{
    public class NetAsynReceive : NetAsynBase
    {
        public NetAsynReceive(Socket sock)
            : base(sock)
        {
            
        }

        //public NetAsynRecevice(WebSocket websock)
        //    :base(websock)
        //{

        //}

        public override void Start()
        {
            m_socketStream = new SocketInputStream();
            base.Start();
        }

        public void SetIsNeedCrypter(bool bNeed)
        {
            m_socketStream.SetIsNeedCrypter(bNeed);
        }

        public override void Stop()
        {
            base.Stop();
            if (null != m_socketStream)
			{
                m_socketStream.Close();
                m_socketStream = null;
			}
        }

        BetterList<NetMessage> m_tempMsgList = new BetterList<NetMessage>();

        /// <summary>
        /// 每一帧读取消息调用
        /// </summary>
        /// <param name="listMsg"></param>
        public void GetReceviceMsg(ref BetterList<NetMessage> listMsg)
        {
            if (m_listMsg.Count > 0)
            {
                m_tempMsgList.Clear();
                foreach(NetMessage ne in m_listMsg)
                {
                    m_tempMsgList.Add(ne);
                }

                foreach (NetMessage msg in m_tempMsgList)
                {
                    listMsg.Add(msg);
                    m_listMsg.Remove(msg);
                }
            }

            if (m_bRecing)
            {
                return;
            }
			_GetMessageFromServer();
        }
		
		public override void WaitSocket()
		{
			if(null != m_ar && m_bRecing)
			{
				m_socket.EndReceive(m_ar);
			}	
		}

        /// <summary>
        /// 调用消息获取
        /// </summary>
        private void _GetMessageFromServer()
        {
            m_bRecing = true;
            __ReceviceMessage();
        }

        /// <summary>
        /// 获取服务器消息,将消息读取到m_packStream里面,每次最大获取的消息长度了8192。
        /// ╮(╯▽╰)╭~结果测试8192认为是消息最好的数字
        /// </summary>
        private void __ReceviceMessage()
        {
            try
            {
                if (m_socket != null)
                {
                    m_socket.BeginReceive(m_packStream.m_bytes, 0, SocketStream.DEFAULTSOCKETMAXBUFFER, SocketFlags.None, new AsyncCallback(this.ReceviceEnd), null);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        /// <summary>
        /// 消息接收完之后回调处理
        /// </summary>
        /// <param name="ar"></param>
		private void ReceviceEnd(IAsyncResult ar)
        {
            if (null == m_socket) return;
            int num = m_socket.EndReceive(ar);
            //填充消息
            m_socketStream.Fill(m_packStream, num);
            //产生消息           
            m_socketStream.GenMsg(ref m_listMsg);
            //处理完消息再接受消息
            m_bRecing = false;
        }
       
        protected bool m_bRecing;
        protected SocketInputStream m_socketStream = null;
        protected SocketStream m_packStream = new SocketStream();
    }
}
