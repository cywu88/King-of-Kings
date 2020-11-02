/******************************************************************
**Author:Foolyoo	
**Date:2020-11-01 
**Describe: Socket消息内容
********************************************************************/
using System;
using System.Collections;

namespace FootStone
{
    public class SocketStream
    {
        public const int DEFAULTSOCKETMAXBUFFER = 8192;
        
        public SocketStream()
        {
               
        }


        public byte[] m_bytes = new byte[DEFAULTSOCKETMAXBUFFER]; 
      
    }
}