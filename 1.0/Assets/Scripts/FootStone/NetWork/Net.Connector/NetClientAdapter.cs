using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public interface NetClientAdapter
    {
        bool Connect(string host, int port);
        bool Disconnect();
        void Send(ISendMessage msg);

        event OnConnectHandler onConnect;
        event OnMessageHandler onMessage;
    }
}
