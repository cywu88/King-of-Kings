using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FootStone;

public class GameManager
{
    public static GameManager Instance { get; private set; }
    static GameManager()
    {
        Instance = new GameManager();
    }

    //初始化游戏逻辑
    public void InitGame()
    {

    }

    public void Start()
    {

    }
     
    /// <summary>
    /// 接受帧消息入口，添加到FSP管理器
    /// </summary>
    public void AddServerFrameMsg(NetMessage msg)
    {

    }

}