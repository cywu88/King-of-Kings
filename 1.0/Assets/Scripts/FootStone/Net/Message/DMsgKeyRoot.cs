/******************************************************************
**Authro: foolyoo	
**描述：  消息的Root定义
********************************************************************/
namespace FootStone
{
    public struct DMsgKeyRoot
    {
        // 登录态消息码
        public const ushort CMD_ROOT_LOGIN = 1;
        // 选择人物态消息码
        public const ushort CMD_ROOT_SELECTACTOR = 2;
          
        // FSP
        public const ushort CMD_ROOT_FSP = 3;

        // 最大ROOT消息码
        public const ushort MD_ROOT_MAX = 20;



    }
}
