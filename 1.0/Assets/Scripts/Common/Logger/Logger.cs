using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Logger
{
    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    static public void Log(string s, params object[] p)
    {
        Debug.Log(DateTime.Now + " -- " + (p != null && p.Length > 0 ? string.Format(s, p) : s));
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    static public void Log(object o)
    {
        Debug.Log(o);
    }

    static public void LogError(string s, params object[] p)
    {
#if UNITY_EDITOR || LOGGER_ON
        Debug.LogError((p != null && p.Length > 0 ? string.Format(s, p) : s));
#else
        AddError(string.Format("clientversion:{0} uid: {1} device:{2} ip:{3} platname:{4} platChannel:{5} scenename:{6} debug_build_ver:{7} \n {8} ",
        clientVerstion, loginUid, (SystemInfo.deviceModel + "/" + SystemInfo.deviceUniqueIdentifier), localIP, platName, platChannel, sceneName, DEBUG_BUILD_VER,
        (p != null && p.Length > 0 ? string.Format(s, p) : s)));
#endif
    }

}
