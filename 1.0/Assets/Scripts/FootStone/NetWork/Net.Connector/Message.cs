using System;

namespace FootStone
{ 
    public class MessagePool
    {
        private readonly ObjectPool<SendMessage> s_SendPool;
        private readonly ObjectPool<RecvMessage> s_RecvPool;

        internal MessagePool()
        { 
            this.s_SendPool = new ObjectPool<SendMessage>(newSend);
            this.s_RecvPool = new ObjectPool<RecvMessage>(newRecv);
        }
        private SendMessage newSend() { return new SendMessage(this); }
        private RecvMessage newRecv() { return new RecvMessage(this); }

        internal SendMessage AllocSend()
        {
            SendMessage ret = s_SendPool.Get(); 
            return ret;
        }

        internal RecvMessage AllocRecv()
        {
            RecvMessage ret = s_RecvPool.Get();
            ret.BufferLength = RecvMessage.MESSAGE_HEAD_SIZE;
            ret.BufferPosition = 0;
            return ret;
        }

        internal class SendMessage : ISendMessage
        {
            private readonly MessagePool pool;
            internal object token;
            internal SendMessage(MessagePool pool)  
            {
                this.pool = pool;
            }
             
            internal void Dispose()
            {
                this.token = null;
                base.Disposing();
                pool.s_SendPool.Release(this);
            }
        }

        internal class RecvMessage : IRecvMessage
        {
            private readonly MessagePool pool;
            internal object token;
            internal RecvMessage(MessagePool pool) 
            {
                this.pool = pool;
            }
            internal void Dispose()
            {
                this.token = null;
                base.Disposing();
                pool.s_RecvPool.Release(this);
            }

        }
    }
}
