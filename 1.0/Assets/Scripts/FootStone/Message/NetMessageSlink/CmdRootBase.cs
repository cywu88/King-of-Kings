/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**描述：  消息转换的基类，提供的主要功能
          1.将服务器发送过来的消息在这里进行解析对应的处理类
********************************************************************/
using UnityEngine;

namespace FootStone
{
    public class CmdRootBase
    {
        public CmdRootBase()
        {

        }

        //public void RegNetMessage(int uId)
        //{
        //    NetRunTime.Inst.RegNetMessageNotify(uId, OnRecv);  
        //}

        public virtual uint GetRootId()
        {
            Debug.LogError("GetRootId方法必须实现");
            return 0;
        }

        public virtual NetMessage ByteToNetMessage(ref StreamHelper ls)
        {
            Debug.LogError("ByteToNetMessage方法必须实现");
            return null;
        }

    }
}
