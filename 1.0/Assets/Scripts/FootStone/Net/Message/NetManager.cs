/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**Describe： 消息的管理类
********************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace FootStone
{
    public delegate NetMessage CreateMessageEvent();
	public class NetManager 
	{
        public NetManager()
        {
            RegNetMessage((uint)NetMessageDefine.CLLS_Login, CLLS_Login.CreateMessage); 
        }

        public bool RegNetMessage(uint uID, CreateMessageEvent mes)
        {
            if (m_MsgMgr.ContainsKey(uID))
            {
                Debug.LogError("重复注册消息ID" + uID.ToString());
                return false;
            }

            m_MsgMgr[uID] = mes;
            return true;
        }

        public NetMessage CreateNetMessage(uint uID)
        {
            if (!m_MsgMgr.ContainsKey(uID))
            {
                Debug.LogError("无效消息ID" + uID.ToString());
                return null;
            }
            return m_MsgMgr[uID]();
        }

        public static NetManager Inst = null;
        Dictionary<uint, CreateMessageEvent> m_MsgMgr = new Dictionary<uint, CreateMessageEvent>();
	}
}
