using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public class NetClientFactory
    {
        public static NetClientFactory Instance
        {
            get; private set;
        }
        static NetClientFactory()
        {
            new NetClientFactory();
        }
        public NetClientFactory()
        {
            Instance = this;
        }
        public virtual NetClientAdapter CreateAdapter(NetConnector client)
        {
            return new NetTCP(client);
        }
    }

    public enum CloseReason : int
    {
        Unknown = 0,

        ClientClose = 1,

        Disconnect = 2,

        KickByServer = 3,

        Error = 4,

        TimeOut = 5,

    }
}
