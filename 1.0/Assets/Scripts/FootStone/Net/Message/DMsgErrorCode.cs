/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**描述：  错误消息定义
********************************************************************/
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;

namespace Lusuo
{   
    // 消息结构
    [StructLayout(LayoutKind.Sequential , Pack =1 )]
    public struct SC_ERRORCODE_MSGFORMAT
    {
        public ushort m_wKeyRoot;				// CMD_ROOT_ERROR
        public ushort m_wErrorCode;			// 错误码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_szErrorData;		// 错误数据
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte [] m_szErrorDescs;		// 错误描述

        public static int GetSize()
        {
            return 128 + 512 + 4;
        }
	
    };

    /// <summary>
    /// 错误码定义
    /// </summary>
    public struct ErrorCode
    {
        // 无此用户或密码不正确
        public const uint ERROR_CODE_INVALIDUSER = 1;

        // 已被禁号
        public const uint ERROR_CODE_PADLOCK = 2;

        // 角色已被禁止登陆
        public const uint ERROR_CODE_PADLOCKACTOR = 3;

        // 登陆服务器忙
        public const uint ERROR_CODE_LOGINBUSY = 4;

        // 场景服务器忙
        public const uint ERROR_CODE_SCENEBUSY = 5;

        // 人物所在城市不合法
        public const uint ERROR_CODE_INVALIDCITY = 6;

        // 人物坐标不合法
        public const uint ERROR_CODE_INVALIDLOC = 7;

        // 此账号已登陆
        public const uint ERROR_CODE_REPEATER = 8;

        // 创建人物失败
        public const uint ERROR_CODE_CREATEFAILED = 9;

        // 删除人物失败
        public const uint ERROR_CODE_DELETEFAILED = 10;

        // 找不到指定场景服务器
        public const uint ERROR_CODE_NOFINDSCENE = 11;

        // 数据库操作失败
        public const uint ERROR_CODE_DBFAILED = 12;

        // 提交的角色数据不正确
        public const uint ERROR_CODE_PUTACTORDATA = 13;

        // 切换地图失败
        public const uint ERROR_CODE_TANSMAP = 14;

        // 服务器主动踢人
        public const uint ERROR_CODE_KICK = 15;


        // 断线重连
        public const uint ERROR_CODE_RECONECT_AGAIN = 16;
    }
}
