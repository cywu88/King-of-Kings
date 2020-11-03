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
        protected readonly MemoryStream buffer = new MemoryStream();

        public byte[] Buffer
        {
            get { return buffer.GetBuffer(); }
        }
        public int BufferPosition
        {
            get { return (int)buffer.Position; }
            set { buffer.Position = value; }
        }
        public int BufferLength
        {
            get { return (int)buffer.Length; }
            set
            {
                if (value < FIXED_HEAD_SIZE)
                {
                    throw new Exception("缓冲区不能小于固定长度 " + FIXED_HEAD_SIZE);
                }
                buffer.SetLength(value);
            }
        }
        public void FillBuffer(byte[] data, int offset, int count)
        {
            this.buffer.Write(data, offset, count);
        }
    }

    public abstract class ISendMessage : IProtocolMessage
    {

    }


    public abstract class IRecvMessage : IProtocolMessage
    {

    }

}
