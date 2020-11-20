using System;
using System.IO;
using System.Net.Sockets;

namespace FootStone
{
    public class NetTCP : NetClientAdapter
    {
        
        protected readonly NetConnector client;

        private long total_recv_bytes;
        private long total_sent_bytes;

        private TcpClient _tcp;

        public event OnConnectHandler onConnect;
        public event OnMessageHandler onMessage;

        public bool connecting { get; private set; }
         


        public NetTCP(NetConnector client)
        {
            this.client = client;
        }

        public bool Connect(string host, int port)
        {
            if (_tcp != null)
            {
                _tcp.Close();
                _tcp = null;
            }
            try
            {
                bool nodelay = true; 
                _tcp = new TcpClient(AddressFamily.InterNetwork) { NoDelay = nodelay };
                _tcp.Client.NoDelay = nodelay;
                _tcp.BeginConnect(host, port, _end_connect, this._tcp);
                connecting = true;
            }
            catch (Exception ex)
            {
                OnException(ex);
            }

            return true;
        }

        public TcpClient GetSocket()
        {
            lock (this) { return _tcp; }
        }


        public  void Send(ISendMessage send_object)
        {
            var so = GetSocket();
            if (so != null && so.Connected)
            {
                _start_send(so, send_object as MessagePool.SendMessage);
            }
        }


        /// <summary>
        /// 有错误发生
        /// </summary>
        /// <param name="ex"></param>
        private void OnException(Exception ex)
        {
        
        }

        /// <summary>
        /// 连接完成处理
        /// </summary>
        /// <param name="ar"></param>
        private void _end_connect(IAsyncResult result)
        {
            connecting = false;
            var so = (result.AsyncState as TcpClient);
            try
            {
                so.EndConnect(result);
                if (so.Connected)
                { 
                    _start_receive_head(so);
                    //_send_handshake(so, info._user);
                }
                else
                {
                    _run_close(so, CloseReason.TimeOut, null);
                }
            }
            catch (SocketException err)
            {
                if (err.SocketErrorCode == SocketError.TimedOut)
                {
                    _on_error(so, CloseReason.TimeOut, err);
                }
                else
                {
                    _on_error(so, CloseReason.Error, err);
                }
            }
            catch (Exception err)
            {
                _on_error(so, CloseReason.Error, err);
            }
        }

        private void _start_receive_head(TcpClient so, MessagePool.RecvMessage recv_object = null)
        { 
            try
            {
                if (!so.Connected)
                {
                    if (recv_object != null) recv_object.Dispose(); 
                    return;
                }
                if (recv_object == null)
                {
                    recv_object = client.msg_pool.AllocRecv();
                    recv_object.token = this._tcp;
                }
                IAsyncResult result = so.GetStream().BeginRead(
                    recv_object.buffer,
                    recv_object.BufferPosition,
                    recv_object.BufferLength - recv_object.BufferPosition,
                    _end_receive_head, recv_object);
            }
            catch (Exception err)
            {
                if (recv_object != null) recv_object.Dispose();
                _on_error(so, CloseReason.Error, err);
            }
        }

        private void _end_receive_head(IAsyncResult result)
        {
            var recv_object = result.AsyncState as MessagePool.RecvMessage;
            var so = recv_object.token as TcpClient;
            try
            {
                if (!so.Connected)
                {
                    recv_object.Dispose();
                    _run_close(so, CloseReason.Disconnect, null);
                    return;
                }
                int length = so.GetStream().EndRead(result);
                if (length > 0)
                {
                    total_recv_bytes += length;
                    recv_object.BufferPosition += length;
                    if (recv_object.BufferPosition == IRecvMessage.MESSAGE_HEAD_SIZE)
                    {
                        recv_object.ReadHead();
                        if (recv_object.PkgLength > 0)
                        {
                            _start_receive_body(recv_object);
                        }
                        else
                        {
                            _received_package(recv_object);
                            _start_receive_head(so);
                        }
                    }
                    else if (recv_object.BufferPosition > IRecvMessage.MESSAGE_HEAD_SIZE)
                    {
                        throw new IOException("endReceiveHead : Receive head overfollow");
                    }
                    else
                    {
                        _start_receive_head(so,recv_object);
                    }
                }
                else
                {
                    recv_object.Dispose();
                    _run_close(so, CloseReason.Disconnect, null);
                }
            }
            catch (Exception err)
            {
                if (recv_object != null) recv_object.Dispose();
                _on_error(so, CloseReason.Error, err);
            }
        }

        private void _start_receive_body(MessagePool.RecvMessage recv_object)
        { 
            var so = this._tcp;
            try
            {
                if (!so.Connected)
                {
                    recv_object.Dispose();
                    _run_close(so, CloseReason.Disconnect, null);
                    return;
                }
                so.GetStream().BeginRead(
                    recv_object.buffer,
                    recv_object.BufferPosition,
                    recv_object.BufferLength - recv_object.BufferPosition,
                    _end_receive_body, recv_object);
            }
            catch (Exception err)
            {
                recv_object.Dispose();
                _on_error(so, CloseReason.Error, err);
            }
        }

        private void _end_receive_body(IAsyncResult result)
        {
            var recv_object = result.AsyncState as MessagePool.RecvMessage;
            var so = recv_object.token as TcpClient;
            try
            {
                if (!so.Connected)
                {
                    recv_object.Dispose();
                    _run_close(so, CloseReason.Disconnect, null);
                    return;
                }
                int length = so.GetStream().EndRead(result);
                if (length > 0)
                {
                    total_recv_bytes += length;
                    recv_object.BufferPosition += length;
                    if (recv_object.BufferPosition == recv_object.BufferLength)
                    {
                        _received_package(recv_object);
                        _start_receive_head(so);
                    }
                    else if (recv_object.BufferPosition > recv_object.BufferLength)
                    {
                        throw new IOException("endReceiveBody : Receive body overfollow");
                    }
                    else
                    {
                        _start_receive_body(recv_object);
                    }
                }
                else
                {
                    recv_object.Dispose();
                    _run_close(so, CloseReason.Disconnect, null);
                }
            }
            catch (Exception err)
            {
                if (recv_object != null) recv_object.Dispose();
                _on_error(so, CloseReason.Error, err);
            }
        }

        private void _received_package(MessagePool.RecvMessage recv_object)
        {
            _received_message(recv_object);
        }

        private void _received_message(MessagePool.RecvMessage message)
        {
            if (onMessage != null)
            {
                onMessage(message);
            }
        }


        private bool _run_close(TcpClient s, CloseReason reason, string message)
        {
            
            return false;
        }

        private void _on_error(TcpClient s, CloseReason reason, Exception err)
        {
            //log.Error(err.Message + " : " + host, err);
            if (err.InnerException is SocketException && reason == CloseReason.Error)
            {
                reason = CloseReason.Disconnect;
            }
            _run_close(s, reason, err.Message); 
        }

         
        //--------------------------------------------------------------------------------------------------
        private void _start_send(TcpClient so, MessagePool.SendMessage send_object)
        {
            try
            {
                send_object.token = so;
                //if (send_object.PkgLength >= Config.MaxPackageSize)
                //{
                //    throw new Exception(string.Format("PkgLength:{0} out of limit:{1} ", send_object.PkgLength, PomeloClientFactory.Instance.Config.MaxPackageSize));
                //}
                int len = send_object.BufferLength;
                so.GetStream().BeginWrite(send_object.buffer, 0, len, _end_send, send_object);
                total_sent_bytes += len;
            }
            catch (Exception err)
            {
                send_object.Dispose();
                _on_error(so, CloseReason.Error, err);
            }
        }
        private void _end_send(IAsyncResult asyncSend)
        {
            var send_object = asyncSend.AsyncState as MessagePool.SendMessage;
            try
            {
                var so = send_object.token as TcpClient;
                try
                {
                    so.GetStream().EndWrite(asyncSend);
                }
                catch (Exception err)
                {
                    _on_error(so, CloseReason.Error, err);
                }
            }
            finally
            {
                send_object.Dispose();
            }
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        //--------------------------------------------------------------------------------------------------


    }
}
