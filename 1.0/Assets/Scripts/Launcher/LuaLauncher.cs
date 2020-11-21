using UnityEngine;
using UniRx.Async;
using XLua;

public class LuaLauncher : MonoBehaviour
{
    LuaEnv env;
    LuaTable ltentry;

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
        //actstart?.Invoke(); 

    }

        // Update is called once per frame
    void Update()
    {
        
    }
     

    void OnDisable()
    {
        Debug.Log("disable");
    }
     

}
