/******************************************************************
**Author:  Foolyoo	
**Date:    2020-11-01 
**描述：  CmdRoot管理器
********************************************************************/

using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace FootStone
{
    public class CmdRootSinkManager
    {
        public CmdRootSinkManager()
        {
            dicCmdRoot = new Dictionary<uint, CmdRootBase>();
        }

        public CmdRootBase getCmdRootById(uint id)
        {
            CmdRootBase val;
            if (dicCmdRoot.TryGetValue((id), out val))
            {
                return val;
            } 
            return null;
        }

        public bool Init()
        {
            dicCmdRoot.Add(DMsgKeyRoot.CMD_ROOT_LOGIN, CmdRootLogin.Instance());


            return true;
        }

        public static CmdRootSinkManager Instance()
        {
            if (s_cmdRootManager == null)
            {
                s_cmdRootManager = new CmdRootSinkManager();
            }
            return s_cmdRootManager;
        }

        public Dictionary<uint, CmdRootBase> dicCmdRoot;
        public static CmdRootSinkManager s_cmdRootManager;
    }
}