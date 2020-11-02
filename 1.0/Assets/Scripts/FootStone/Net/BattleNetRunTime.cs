using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FootStone
{
    public class BattleNetRunTime
    {
        private string m_ip;
        private int m_port;
        private Socket m_socketClient;
        private NetAsynSend m_nSend;
        private NetAsynReceive m_nRece;
        private BetterList<NetMessage> m_listSend = new BetterList<NetMessage>();
        private BetterList<NetMessage> m_listRecevice = new BetterList<NetMessage>();
        private Action m_connectedCb;

        public enum NetState
        {
            Disconnected,
            Connecting,
            ConnSucc,
            ConnFail,
            Queue,
        }

        private NetState m_netState = NetState.Disconnected;

        public void Conn(string ip, int port, Action coonCb)
        {
            m_netState = NetState.Connecting;

            m_ip = ip;
            m_port = port;
            m_connectedCb = coonCb;

            IPAddress ipAddres = null;
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                IPAddress[] address = null;
                try
                {
                    address = Dns.GetHostAddresses(m_ip);
                }
                catch (Exception e)
                {
                    Debug.Log("客户端本地，连接异常, ip:" + m_ip + " port:" + m_port + " " + e);
                    m_netState = NetState.ConnFail;
                    return;
                }

                if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    Debug.Log("Connect InterNetworkV6");
                    m_socketClient = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }
                else
                {
                    Debug.Log("Connect InterNetworkV4");
                    m_socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                ipAddres = address[0];

            }
            else
            {
                Debug.Log("battle Connect 4g, 源 ip:" + m_ip);

                m_socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress.TryParse(m_ip, out ipAddres);
            }

            if (ipAddres == null)
            {
                Debug.Log("战斗网络-客户端本地，连接异常, ip:" + m_ip + " port:" + m_port);
                m_netState = NetState.ConnFail;
                return;
            }

            m_nSend = new NetAsynSend(m_socketClient);
            m_nSend.SetIsFsp(true);
            m_nSend.Start();
            m_nSend.SetIsNeedCrypter(false);
            m_nRece = new NetAsynReceive(m_socketClient);
            m_nRece.Start();
            m_nRece.SetIsNeedCrypter(false);

            IPEndPoint serverIP = new IPEndPoint(ipAddres, m_port);
            m_socketClient.BeginConnect(serverIP, ConnSucc, null);
        }

        private void ConnSucc(IAsyncResult ar)
        {
            try
            {
                m_socketClient.EndConnect(ar);
                m_netState = NetState.ConnSucc;
            }
            catch (Exception e)
            {
                Debug.Log("战斗网络-客户端本地：连接异常:" + e);
                m_netState = NetState.ConnFail;
            }
            Debug.Log("战斗网络-连接成功");
        }

        public void Stop()
        {
            if (m_nSend != null)
            {
                m_nSend.Stop();
                m_nSend = null;
            }
            if (m_nRece != null)
            {
                m_nRece.Stop();
                m_nRece = null;
            }
            if (m_socketClient != null)
            {
                m_socketClient.Close();
                m_socketClient = null;
            }
        }
        
        public void FixedUpdate()
        {

            if (m_netState == NetState.ConnSucc)
            {
                if (m_connectedCb != null)
                {
                    m_connectedCb();
                    m_connectedCb = null;
                }
                _UpdateReceive();
                _UpdateSend(); 
            } 
        }

        private void _UpdateReceive()
        {
            if (m_nRece != null)
            {
                m_nRece.GetReceviceMsg(ref m_listRecevice);
                for (int i = 0; i < m_listRecevice.Count; i++)
                {
                    var msg = m_listRecevice[i];
                    if (msg != null)
                    {
                        if (!msg.OnRecv())
                        {
                            Debug.LogWarning("消息处理失败----" + msg.m_uID.ToString());
                        }
                    }
                }
                m_listRecevice.Clear();
            }
        }

        private void _UpdateSend()
        {
            if (null != m_nSend)
            {
                if (m_nSend.SendMessage(ref m_listSend))
                {
                    m_listSend.Clear();
                }
            }
        } 

        public void SendMessage(NetMessage msg)
        {
            m_listSend.Add(msg);
        }
    }
}
