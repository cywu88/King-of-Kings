using System.Net.Sockets;
using System.Threading;

namespace FootStone
{
    public class NetBase
    {
        public NetBase(string strName, Socket sock, uint uSize)
        {
            m_strThreadName = strName;
            m_socket = sock;
            m_uBufferSize = uSize;     //缓冲区大小
        }

        public virtual void Start()
        {
            m_Thread = new Thread(this.Work);
            m_Thread.Name = m_strThreadName;
            m_Thread.Start();
        }

        public virtual void Stop()
        {
            m_bShutDown = true;
            m_event.Set();
            while (m_bIng) { }; //等待直到做完事情
            m_socket = null;
            m_listMsg.Clear();
            m_listDelegates.Clear();
            m_Thread = null;
            m_event.Close();
            m_event = null;
        }

        public virtual void Work()
        {
            while (null != m_event && !m_bShutDown && null != m_socket)
            {
                m_event.WaitOne();
                m_bIng = true;
                SubWork();
                m_bIng = false;
            }
        }

        bool IsSupend() { return m_bSuspend; }

        public virtual void Suspend(bool bForce)
        {
            if (null == m_event)
            {
                return;
            }

            if (m_bSuspend && !bForce)
            {
                return;
            }
            m_bSuspend = true;
            m_event.Reset();
        }

        public virtual void Resume(bool bForce)
        {
            if (null == m_event)
            {
                return;
            }

            if (!m_bSuspend && !bForce)
            {
                return;
            }
            m_bSuspend = false;
            m_event.Set();
        }

        public virtual void WaitSocket()
        {

        }

        public void SetShutDown(bool bShutDown) { m_bShutDown = bShutDown; }
        protected virtual void SubWork() { }

        protected bool m_bIng = false;
        protected bool m_bSuspend = true;
        protected string m_strThreadName;
        protected ManualResetEvent m_event = new ManualResetEvent(false);
        protected Socket m_socket = null;
        protected uint m_uBufferSize = 8192;   //默认8k
        //protected Mutex m_Lock = new Mutex(true);
        protected bool m_bShutDown = false;
        protected Thread m_Thread = null;
        protected BetterList<NetMessage> m_listMsg = new BetterList<NetMessage>();
        protected BetterList<NetMessage> m_listDelegates = new BetterList<NetMessage>();
    }
}
