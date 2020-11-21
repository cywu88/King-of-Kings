
public class ResourceMgr_New : SingletonMonobehaviour<ResourceMgr_New>
{
    public byte[] LoadLua(string luamodule)
    {
        return UabManager.LoadLua(luamodule);
    } 
}