using UnityEngine;
using UniRx.Async;
using XLua;
using System;

public class LuaLauncher : MonoBehaviour
{
    LuaEnv env;
    LuaTable ltentry;

    Action actstart;
    Action actupdate;
    public bool NeedDisplose;

    private byte[] loader(ref string luapath)
    {
        return ResourceMgr_New.Instance.LoadLua(luapath);
    }

    private void bootstrap()
    {
        env = new LuaEnv();
        env.AddLoader(loader);
         
        string bootstrapPath = "Launcher.bootstrap_1";
        byte[] bt = loader(ref bootstrapPath);

        object[] objs = env.DoString(bt, "bootstrap_1");
        if (objs == null || objs.Length == 0)
        {
            Debug.LogError($"lua launcher load bootstrap fail. path: {bootstrapPath}");
            return;
        }

        ltentry = objs[0] as LuaTable;
        if (ltentry == null)
        {
            Debug.LogError($"get ltentry fail. objs.length = {objs.Length}");
            return;
        }

        ltentry.Set("_mono", this);
        ltentry.Get("start", out actstart);
        ltentry.Get("update", out actupdate);
        if (actstart == null)
        {
            Debug.LogError($"fail to get action start in bootstrap");
        }

        NeedDisplose = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        //Test
        //GameManager.Instance.Start();
         
        StartBootStrap(); 
    }
     

    private async void StartBootStrap(int level = 1)
    {
        await UabManager.Initialize();
        if (!UabManager.initialized)
        {
            Debug.LogError("UabManager initialize fail");
            return;
        }
        Debug.Log("UabManager initialize success");
         
        bootstrap();

        actstart?.Invoke(); 

    }

    // Update is called once per frame
    private void Update()
    {
        if (NeedDisplose)
        {
            NeedDisplose = false;
            Dispose();
            return;
        }
        if (env == null)
        {
            return;
        }
        actupdate?.Invoke();
    }

    public void Dispose()
    {
        env.Tick();
        env.FullGc();
        env.Dispose(true);
        env = null;

        ltentry = null;
        actstart = null;
        actupdate = null;

        GC.Collect();

        Debug.Log("LuaLauncher disposed..."); 
    }


    void OnDisable()
    {
        Debug.Log("disable");
    }
     

}
