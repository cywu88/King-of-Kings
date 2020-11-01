using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FootStone
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SSUserLoginContext
    {
        public bool bRegist;
        public uint dwVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] szNetworkCard;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szUserName;			                //账号orQQUID
        public int lPassPodPwd;							// 密宝的随机数字
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public byte[] szMD5SureCode;                    	// 验证码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szPassword;			// 密码
        public int platId;                  //平台Id
        public int platUid;                //平台uId
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] serverName;                //服务器Id;

        public void init()
        {
            bRegist = false;
            szNetworkCard = new byte[512];
            szUserName = new byte[32];
            szMD5SureCode = new byte[33];
            szPassword = new byte[32];
            serverName = new byte[32];
        }


        public void SetUserName(string name)
        {
            byte[] temp = Encoding.UTF8.GetBytes(name);

            for (int i = 0; i < temp.Length; ++i)
            {
                szUserName[i] = temp[i];
            }
        }

        public void SetServerName(string name)
        {
            byte[] temp = Encoding.UTF8.GetBytes(name);

            for (int i = 0; i < temp.Length; ++i)
            {
                serverName[i] = temp[i];
            }
        }
         
        public void SetSureCode(string sureCode)
        {
            byte[] temp = Encoding.UTF8.GetBytes(sureCode);

            for (int i = 0; i < temp.Length; ++i)
            {
                szMD5SureCode[i] = temp[i];
            }
        }

        public static int GetSize()
        {
            return 1 + 4 + 512 + 32 + 33 + 32 + 4 + 12 + 28;
        }
    };
}
