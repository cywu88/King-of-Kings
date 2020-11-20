using FootStone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaLauncher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = new NetConnector();
        a.Connect("127.0.0.1", 1255);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
