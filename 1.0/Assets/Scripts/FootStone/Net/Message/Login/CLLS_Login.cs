/******************************************************************
**描述：  玩家登陆消息定义类
********************************************************************/
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using System;

namespace FootStone
{
    public class CLLS_Login : NetMessage
    {
        public CLLS_Login()
            : base((int)NetMessageDefine.CLLS_Login, 0)
        {
            head.m_wKeyRoot = DMsgKeyRoot.CMD_ROOT_LOGIN;
            head.m_wKeyMain = DMsgKeyMainLogin.ROOTLOGIN_CS_MAIN_USERLOGIN;
            loginContext.init();
        }

        static public NetMessage CreateMessage()
        {
            return new CLLS_Login();
        }

        public override int ToByte(ref StreamHelper lf)
        {
            base.ToByte(ref lf);

            lf.WriteUShort(head.m_wKeyRoot);
            lf.WriteUShort(head.m_wKeyMain);

            lf.WriteBool(loginContext.bRegist);
            lf.WriteUInt(loginContext.dwVersion);
            lf.Write(ref loginContext.szNetworkCard);
            lf.Write(ref loginContext.szUserName);
            lf.WriteInt(loginContext.lPassPodPwd);
            lf.Write(ref loginContext.szMD5SureCode);
            lf.Write(ref loginContext.szPassword);
            lf.WriteInt(loginContext.platId);
            lf.WriteInt(loginContext.platUid);
            lf.Write(ref loginContext.serverName);
            return (int)m_uMsgLenght;
        }

        public override void MakeMessage()
        {
            m_uDataLenght = (ushort)(CS_LOGIN_HEAD.GetSize() + SSUserLoginContext.GetSize());
            base.MakeMessage();
        }

        public override void DebugMessage(out string strDebug)
        {
            base.DebugMessage(out strDebug);
        }

        public CS_LOGIN_HEAD head = new CS_LOGIN_HEAD();
        public SSUserLoginContext loginContext = new SSUserLoginContext();
    }

}


