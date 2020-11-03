using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public class NetTCP : NetClientAdapter
    {
        protected readonly NetConnector client;

        public NetTCP(NetConnector client)
        {
            this.client = client;
        }

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
