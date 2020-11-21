using System.Collections.Generic;
using FootStone;
using PBMessage;
using UnityEngine;

public class FSPParam
{
    public const int clientFrameInterval = 30;        //客户端帧率
    public const float clientFrameScTime = 0.032f;        //客户端帧率时间(秒)
    public const long clientFrameScLongTime = (long)((double)clientFrameScTime );        //客户端帧率时间(秒)
    public const int clientFrameMsTime = 32;        //客户端帧率时间(毫秒)
    public const int serverTimeout = 15000;          //服务器判断客户端的超时
    public const int clientFrameRateMultiple = 2;   //客户端和服务器端的帧率倍数
    public const bool enableSpeedUp = true;         //  是否可以加速
    public const int defaultSpeed = 1;  //默认的数量
    public const int frameBufferSize = 0; //需要缓存的帧数
    public const bool enableAutoBuffer = true;
    public const int maxFrameId = 1800; //最大的帧率数
    public const bool useLocal = false;  //是否使用本地数据
}

public class GameManager
{
    public static GameManager Instance { get; private set; }
    static GameManager()
    {
        Instance = new GameManager();
    }

    private NetConnector connecor;

    public FSPManager m_mgrFSP;      // FSP管理器

    //初始化游戏逻辑
    public void InitGame()
    {
        this.RegisterReceiver();

        connecor = new NetConnector();
        connecor.Connect("127.0.0.1", 1255);
    }

    public void RegisterReceiver()
    {
        #region Message
        MessageDispatch.RegisterReceiver<GM_Accept>(MessageID.GM_ACCEPT_SC, OnAccept);
        #endregion
    }

    public void UnRegisterReceiver()
    {
        #region Message
        MessageDispatch.UnRegisterReceiver<GM_Accept>(MessageID.GM_ACCEPT_SC, OnAccept);

        #endregion
    }

    public void Stop()
    {
        this.UnRegisterReceiver();
        Debug.Log("destroy");
    }


    private void OnAccept(GM_Accept recvData)
    {
        if (recvData == null)
        {
            return;
        } 
    }

    public void Start()
    {
        //启动帧同步逻辑
        m_mgrFSP = new FSPManager();

    }


    private List<NetMessage> listPreFrameMsg = new List<NetMessage>();

    /// <summary>
    /// 接受帧消息入口，添加到FSP管理器
    /// </summary>
    public void AddServerFrameMsg(NetMessage msg)
    {
        if (m_mgrFSP == null)
        {
            // Debug.LogWarning("AddServerFrameMsg m_mgrFSP == null");
            listPreFrameMsg.Add(msg);
            return;
        }

        if (listPreFrameMsg.Count > 0)
        {
            for (int nIndex = 0; nIndex < listPreFrameMsg.Count; nIndex++)
            {
                m_mgrFSP.AddServerFrameMsg(listPreFrameMsg[nIndex]);
            }
            listPreFrameMsg.Clear();
        }

        m_mgrFSP.AddServerFrameMsg(msg);
    }


    //每桢消息处理
    public void EnterFrame(int frameIndex)
    {


    }

}