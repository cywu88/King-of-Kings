using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace FootStone
{
    public class NetRecevice : NetBase
    {
        public NetRecevice(Socket sock,uint uSize)
            : base("recevice", sock, uSize)
        {
            
        }
		
		public override void Start()
        {
			m_stream = new StreamHelper(new byte[m_uBufferSize]);
            base.Start();
			Resume(true);
        }

        public override void Stop()
        {
            base.Stop();
			if(m_stream != null)
			{
				m_stream.Close();
            	m_stream = null;	
			}
            
            for (int i = 0; i < m_listBackStream.Count; i ++ )
            {
                m_listBackStream[i].Close();
            }
            m_listBackStream.Clear();
            m_listBackStream = null;
        }

        public override void Work()
        {
            while (null != m_event && !m_bShutDown && null != m_socket)
            {
                m_event.WaitOne(15);
                m_bIng = true;
                SubWork();
                m_bIng = false;
            }
        }

		public override void WaitSocket()
		{
				while(m_bIng){};
		}

        public void GetReceviceMsg(ref BetterList<NetMessage> listMsg)
        {
            if (m_listMsg.Count == 0)
            {
                return;
            }
			Monitor.Enter(this);
            foreach (NetMessage msg in m_listMsg)
            {
                listMsg.Add(msg);
            }
            m_listMsg.Clear();
			Monitor.Exit(this);
        }
		
		protected override void SubWork()
        {
            if (null != m_socket)
            {
				if(m_socket.Connected)
				{
					ReceiveMessage();
		            //接收完了。转换成消息等待前台拿
		            GetReceviceToMsg();	
				}
            }
        }
		
        //
        private void ReceiveMessage()
        {
            StreamHelper stream = __ReceviceMessage(ref m_stream);
            while (null != stream)
            {
                stream = __ReceviceMessage(ref stream);
            }
            m_stream.Reset();
        }

        private StreamHelper __ReceviceMessage(ref StreamHelper lf)
        {
            //先用基本缓冲读一次
            int iLen = 0;
            try
            {
                iLen = m_socket.Receive(lf.GetBuffer(), (int)m_uBufferSize, SocketFlags.None);
                if (iLen <= 0)
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
				Debug.LogError(ex.Message);
                //Console.WriteLine(ex.Message);
            }
            
            //说明都读不完
            if (iLen >= m_uBufferSize)
            {
                __GenMsg();
                //还没有读完
                //用新的流继续读
                //把没有读完的数据写过来
                StreamHelper back = new StreamHelper(new byte[m_uBufferSize]);
                byte[] lastbyte = new byte[lf.GetRest()];
                lf.Read(ref lastbyte);
                back.Write(ref lastbyte);
                //以后可以利用这个优化
                m_listBackStream.Add(back);
                return back;
            }
            else
            {
                //基本缓冲可以读完
                while (iLen > lf.GetOffset())
                {
                    if (-2 == __GenMsg(ref lf))
                    {
                        //断开因为有消息不认识了。说明客户端有问题了
                        //Stop();
                    }
                }
                return null;
            }
        }

        private void __GenMsg()
        {
            //int iMinSize = StringHelper.s_IntSize + StringHelper.s_ShortSize;
            //基本流读完了
            while (__GenMsg(ref m_stream) != -1) { }
        }

        private int __GenMsg(ref StreamHelper lf)
        {
            //如果不够读出一份完整数据。回读
            int iMinSize = StringHelper.s_IntSize + StringHelper.s_ShortSize;
            if (m_uBufferSize - lf.GetOffset() < iMinSize)
            {
                return -1;
            }
            uint uID        = 0;
            ushort uLen  = 0;
            lf.ReadUInt(ref uID);
            lf.ReadUShort(ref uLen);
            //如果不够读一次了
            if (m_uBufferSize - (ushort)lf.GetOffset() < uLen)
            {
                //返回
                lf.Seek(lf.GetOffset() - iMinSize);
                return -1;
            }
            //
            NetMessage msg = NetManager.Inst.CreateNetMessage(uID);
            if(null == msg)
            {
                Debug.LogError("未知的消息" + uID.ToString());
                //客户端被修改
                return -2;
            }
            msg.m_uDataLenght = uLen;
            msg.FromByte(ref lf);
            m_listDelegates.Add(msg);
            return (int)m_uBufferSize - lf.GetOffset();
        }

        //得到接收的消息
        private void GetReceviceToMsg()
        {
			Monitor.Enter(this);
            foreach (NetMessage msg in m_listDelegates)
            {
                m_listMsg.Add(msg);
            }
            m_listDelegates.Clear();
			Monitor.Exit(this);
            //新申请的内存用完了。
            for (int i = 0; i < m_listBackStream.Count; i++)
            {
                m_listBackStream[i].Close();
            }
            m_listBackStream.Clear();
        }
		
        protected StreamHelper m_stream = null;
        protected BetterList<StreamHelper> m_listBackStream = new BetterList<StreamHelper>();
    }
}
