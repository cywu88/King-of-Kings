using System;
using System.Net.Sockets;
 
namespace FootStone
{
    public delegate void MessageWork();
	public class NetAsynBase
	{
        public NetAsynBase(Socket sock)
        {
            m_socket = sock;
        }

        //public NetAsynBase(WebSocket sock)
        //{
        //    m_webSocket = sock;
        //}

        public virtual void Start()
        {

        }

        public virtual void Stop()
        {
			WaitSocket();
            
			m_socket = null;
            //m_webSocket = null;
            m_listMsg.Clear();
        }
		
		//强制接受或者发送完所有消息
		public virtual void WaitSocket()
		{
				
		}

        protected Socket m_socket = null;
        //protected WebSocket m_webSocket = null;
        protected BetterList<NetMessage> m_listMsg = new BetterList<NetMessage>();
		protected IAsyncResult m_ar;
	}
}
