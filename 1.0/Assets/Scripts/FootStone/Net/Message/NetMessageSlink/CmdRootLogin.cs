/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**描述：  登陆消息转换类
********************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace FootStone
{
    public class CmdRootLogin : CmdRootBase
    {
        public CmdRootLogin()
        {
            //RegNetMessage((int)NetMessageDefine.LSCL_ActorListRet);
        }


        private static CmdRootLogin s_cmdRootChat;
        public static CmdRootLogin Instance()
        {
            if (s_cmdRootChat == null)
            {
                s_cmdRootChat = new CmdRootLogin();
            }
            return s_cmdRootChat;

        }
        public override uint GetRootId()
        {
            return DMsgKeyRoot.CMD_ROOT_LOGIN;
        }


        //消息解析
        public override NetMessage ByteToNetMessage(ref StreamHelper ls)
        {
            ushort mainKey = 0;
            ls.ReadUShort(ref mainKey);
            switch (mainKey)
            { 
                default:
                    Debug.Log("CmdRootLogin ByteToNetMessage subkey" + mainKey);
                    return null;
            }
        }


    }
}