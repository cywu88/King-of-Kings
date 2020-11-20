using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public enum PackageType : byte
    {
        PKG_HANDSHAKE = 1,
        PKG_HANDSHAKE_ACK = 2,
        PKG_HEARTBEAT = 3,
        PKG_MESSAGE = 4,
        PKG_KICK = 5
    }
    public enum PackageMask : byte
    {
        Compressed = 1,
        Dummy1 = 2,
        Dummy2 = 4,
        Dummy3 = 8,
    }

    public enum MessageType : byte
    {
        MSG_NOTIFY = 0,
        MSG_REQUEST_C2S = 1,
        MSG_RESPONSE_S2C = 2,
        MSG_RPC_REQUEST_S2C = 3,
        MSG_RPC_RESPONSE_C2S = 4, 
    }
     

    public class IProtocol
    { 
        public const int MESSAGE_ID_OFFSET = 0;  //包ID偏移
        public const int MESSAGE_BODY_SIZE_OFFSET = 4; //包体大小偏移
        public const int MESSAGE_VERSION_OFFSET = 8;    //包版本偏移
        public const int MESSAGE_EXTRA_OFFSET = 12;  //额外数据
        public const int MESSAGE_BODY_OFFSET = 16; //包体偏移
        public const int MESSAGE_HEAD_SIZE = 16;//包头大小

        public const int MESSAGE_VERSION = 1;

        public static int MESSAGE_MAX_VALUE = 1000;
        public static int MESSAGE_MIN_VALUE = 0;


        public int PkgLength { get; protected set; }
    }

}
