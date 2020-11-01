/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**描述：  登陆消息Main定义
********************************************************************/
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace Lusuo
{
    public struct DMsgKeyMainLogin
    {
        public const ushort ROOTLOGIN_SC_MAIN_USERLOGIN = 1;
        public const ushort ROOTLOGIN_CS_MAIN_USERLOGIN = 1;

        // 调至选择人物态
        public const ushort ROOTLOGIN_SC_MAIN_TURNSELECT = 2;
        public const ushort ROOTLOGIN_CS_MAIN_TURNSELECT = 2;

        // 队列位置，从0开始
        public const ushort ROOTLOGIN_SC_MAIN_LISTNUM = 3;
        public const ushort ROOTLOGIN_CS_MAIN_LISTNUM = 3;

        // 发送客户端版本号
        public const ushort ROOTLOGIN_SC_MAIN_CLIENTVER = 4;
        public const ushort ROOTLOGIN_CS_MAIN_CLIENTVER = 4;

        // QQ用户登录
        public const ushort ROOTLOGIN_SC_MAIN_USERLOGIN_QQ = 5;
        public const ushort ROOTLOGIN_CS_MAIN_USERLOGIN_QQ = 5;
    }

    public struct DMsgKeyMainSelectActor
    {
        // 选择人物
        public const ushort ROOTSELECTACTOR_SC_MAIN_SELECT = 1;
        public const ushort ROOTSELECTACTOR_CS_MAIN_SELECT = 1;

        // 将调到运行态
        public const ushort ROOTSELECTACTOR_SC_MAIN_TURNRUN = 2;
        public const ushort ROOTSELECTACTOR_CS_MAIN_TURNRUN = 2;

        // 创建角色
        public const ushort ROOTSELECTACTOR_SC_MAIN_CREATEACTOR = 3;
        public const ushort ROOTSELECTACTOR_CS_MAIN_CREATEACTOR = 3;

        // 删除角色
        public const ushort ROOTSELECTACTOR_SC_MAIN_DELETEACTOR = 4;
        public const ushort ROOTSELECTACTOR_CS_MAIN_DELETEACTOR = 4;

        // 发送登录随机数
        public const ushort ROOTSELECTACTOR_SC_MAIN_ENTERCODE = 5;
        public const ushort ROOTSELECTACTOR_CS_MAIN_ENTERCODE = 5;

        // 发送MAC
        public const ushort ROOTSELECTACTOR_SC_MAIN_MAC = 6;
        public const ushort ROOTSELECTACTOR_CS_MAIN_MAC = 6;

        // QQ用户选择人物
        public const ushort ROOTSELECTACTOR_SC_MAIN_SELECT_QQ = 7;
        public const ushort ROOTSELECTACTOR_CS_MAIN_SELECT_QQ = 7;

        // 角色改名
        public const ushort ROOTSELECTACTOR_SC_MAIN_CHANGENAME = 8;
        public const ushort ROOTSELECTACTOR_CS_MAIN_CHANGENAME = 8;

        // 直接发送GM命令
        public const ushort  ROOTLOGIN_SC_MAIN_GM	         	= 9;
        public const ushort  ROOTLOGIN_CS_MAIN_GM		        = 9;

        // 跨服的命令处理
        public const ushort  ROOTLOGIN_SG_MAIN_GM	         	= 10;
        public const ushort  ROOTLOGIN_GS_MAIN_GM		        = 10;

        // 直接跨服登录
        public const ushort  ROOTLOGIN_SC_MAIN_CROSS_SERVER	    = 11;
        public const ushort  ROOTLOGIN_CS_MAIN_CROSS_SERVER     = 11;
    }
}
