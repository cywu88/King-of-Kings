using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public abstract class IProtocolMessage : IProtocol
    {
        protected readonly MemoryStream stream = new MemoryStream();

        public byte[] buffer
        {
            get { return stream.GetBuffer(); }
        }

        public int BufferPosition
        {
            get { return (int)stream.Position; }
            set { stream.Position = value; }
        }
        public int BufferLength
        {
            get { return (int)stream.Length; }
            set
            {
                if (value < MESSAGE_HEAD_SIZE)
                {
                    throw new Exception("缓冲区不能小于固定长度 " + MESSAGE_HEAD_SIZE);
                }
                stream.SetLength(value);
            }
        }
        public void FillBuffer(byte[] data, int offset, int count)
        {
            this.stream.Write(data, offset, count);
        }

        public static bool Decode(byte[] buffer, int offset, ref int value)
        {
            if (buffer == null || buffer.Length < MESSAGE_HEAD_SIZE || offset + 4 > buffer.Length)
            {
                return false;
            }

            value = BitConverter.ToInt32(buffer, offset);

            return true;
        }
         
        public byte[] body()
        {
            int bodySize = -1;
            if (Decode(buffer, MESSAGE_BODY_SIZE_OFFSET, ref bodySize))
            {
                byte[] body = new byte[bodySize];
                Buffer.BlockCopy(buffer, MESSAGE_BODY_OFFSET, body, 0, bodySize);
                return body;
            }
            return null;
        }   


        protected abstract void Disposing();
    }

    public abstract class ISendMessage : IProtocolMessage
    {
        public object SendingObject { get; private set; }

        public void InitWithMessage(int messageId, byte[] data, int extra = 0)
        {
            this.BufferLength = MESSAGE_HEAD_SIZE + data.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(messageId), 0, buffer, MESSAGE_ID_OFFSET, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, buffer, MESSAGE_BODY_SIZE_OFFSET, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(MESSAGE_VERSION), 0, buffer, MESSAGE_VERSION_OFFSET, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(extra), 0, buffer, MESSAGE_EXTRA_OFFSET, 4);
            Buffer.BlockCopy(data, 0, buffer, MESSAGE_BODY_OFFSET, data.Length);
        } 

        public void InitWithMessage<T>(int messageId, T data, int extra = 0) where T : class, ProtoBuf.IExtensible
        {
            byte[] bytes = ProtoTransfer.SerializeProtoBuf<T>(data);
            this.InitWithMessage(messageId, bytes, extra);
        }
 
          
        protected override void Disposing()
        {
     
        } 
    }


    public abstract class IRecvMessage : IProtocolMessage
    {
        public void ReadHead()
        {
            var p = stream.Position;
            try
            {
                stream.Position = 0;
                int bodySize = 0;
                if (Decode(buffer, IRecvMessage.MESSAGE_BODY_SIZE_OFFSET, ref bodySize))
                {
                    this.PkgLength = bodySize;
                } 
                this.BufferLength = PkgLength + MESSAGE_HEAD_SIZE;
            }
            finally { stream.Position = p; }
        }

        protected override void Disposing()
        {
     
        }

    }


}
