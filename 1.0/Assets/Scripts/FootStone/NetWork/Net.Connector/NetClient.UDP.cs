using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public class NetUDP : NetClientAdapter
    {
        public event OnConnectHandler onConnect;
        public event OnMessageHandler onMessage;

        public bool Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Send(ISendMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
