using System;
using System.Runtime.InteropServices;

namespace FootStone
{
    /////////////////////////////////////////////////////////
    //// 描  述：服务器发给客户端的登陆态消息码
    ////
    ////
    /////////////////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SC_LOGIN_HEAD
    {
        public ushort m_wKeyRoot;
        public ushort m_wKeyMain;

        SC_LOGIN_HEAD(ushort wKeyMain)
        {
            m_wKeyRoot = DMsgKeyRoot.CMD_ROOT_LOGIN;
            m_wKeyMain = wKeyMain;
        }
        public static int GetSize()
        {
            return StringHelper.s_ShortSize + StringHelper.s_ShortSize;
        }
    };

    /////////////////////////////////////////////////////////
    //// 描  述：客户端发给服务器的登陆态消息码
    //// 
    ////
    /////////////////////////////////////////////////////////
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CS_LOGIN_HEAD
    {
        public ushort m_wKeyRoot;
        public ushort m_wKeyMain;

        CS_LOGIN_HEAD(ushort wKeyMain)
        {
            m_wKeyRoot = DMsgKeyRoot.CMD_ROOT_LOGIN;
            m_wKeyMain = wKeyMain;
        }
        public static int GetSize()
        {
            return StringHelper.s_ShortSize + StringHelper.s_ShortSize;
        }
    };


}