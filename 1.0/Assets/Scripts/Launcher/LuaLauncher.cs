using UnityEngine;
using FootStone;
using PBMessage; 


public class LuaLauncher : MonoBehaviour
{
    NetConnector connecor;
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAccept(GM_Accept recvData)
    {
        if (recvData == null)
        {
            return;
        }

       
    }


    void OnDisable()
    {
        Debug.Log("disable");
    }


    void OnDestroy()
    {
        this.UnRegisterReceiver();
        Debug.Log("destroy"); 
    }


}
