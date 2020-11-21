using UnityEngine;

public class ResourceMgr_New : SingletonMonobehaviour<ResourceMgr_New>
{
    public byte[] LoadLua(string luamodule)
    {
        return UabManager.LoadLua(luamodule);
    }

    public GameObject CreateGameObjectFromResources(string respath, Transform parent)
    {
        respath = respath.Replace('.', '/');
        var o = Resources.Load(respath);
        if (o == null)
        {
            return null;
        }
        GameObject go = Instantiate(o) as GameObject;
        if (parent != null)
        {
            go.transform.AddParent(parent);
        }

        return go;
    }
}