using PBMessage;
using System;
using System.IO;

namespace FootStone
{
    public class TypeCodec
    {
        
    }

    public static class ProtoTransfer
    {
        public static byte[] SerializeProtoBuf<T>(T data) where T : class, ProtoBuf.IExtensible
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, data);
                byte[] bytes = ms.ToArray(); 
                ms.Close(); 
                return bytes;
            }
        }

        public static T DeserializeProtoBuf<T>(IProtocolMessage buffer) where T : class, ProtoBuf.IExtensible
        {
            return DeserializeProtoBuf<T>(buffer.body());
        }

        public static T DeserializeProtoBuf<T>(byte[] data) where T : class, ProtoBuf.IExtensible
        {
            if (data == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(data))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(ms);
                return t;
            }
        }

        public static object DeserializeProtoBuf(byte[] data, Type type)
        {
            if (data == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null, type);
            }
        }
    }


    public delegate void OnConnectHandler();
    public delegate void OnMessageHandler(IRecvMessage msg);


    public class NetConnector
    {
        public enum ClientID
        {
            Frame,
        }

        protected internal readonly MessagePool msg_pool;
        protected internal readonly NetClientAdapter adapter;


      
        public NetConnector()
        {
            this.msg_pool = new MessagePool();
            this.adapter = NetClientFactory.Instance.CreateAdapter(this);
            this.adapter.onConnect += OnConnect;
        }

        private void OnConnect()
        {
            throw new NotImplementedException();
        }

        public virtual bool Connect(string host, int port)
        {
            return this.adapter.Connect(host, port);
        }
         

        protected internal void send<T>(ClientID clientID, MessageID id, T data) where T : class, ProtoBuf.IExtensible
        {
            try
            { 
                var send = msg_pool.AllocSend();
                byte[] bytes = ProtoTransfer.SerializeProtoBuf<T>(data);
                send.InitWithMessage((int)id, bytes);
                adapter.Send(send);
            }
            catch (Exception err)
            {
               
            }
        }
         

    }
}
