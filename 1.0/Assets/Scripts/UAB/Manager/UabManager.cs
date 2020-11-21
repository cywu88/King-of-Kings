using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UniRx.Async;

using Object = UnityEngine.Object;

public partial class UabManager {
    public static bool showLog = false;

    public static bool useUab;
    public static bool initialized {
        get;
        private set;
    }

    public static int LocalVersion = -1;
    public static int OnlineVersion = -1;

    private static string processResPath(string respath) {
        return respath.Replace('.', '/');
    }

    public static Object LoadFromResources(string respath, Type specifyType = null) {
        respath = processResPath(respath);
        Object rtn = Resources.Load(respath, specifyType);
        return rtn;
    }
    public static T LoadFromResources<T>(string respath) where T : Object {
        Object o = LoadFromResources(respath, typeof(T));
        return (T)o;
    }

    public static async UniTask<Object> LoadFromResourcesAsync(string respath, Type specifyType = null) {
        respath = processResPath(respath);
        ResourceRequest req = Resources.LoadAsync(respath, specifyType);
        await req;
        Object rtn = req.asset;
        return rtn;
    }
    public static async UniTask<Object> LoadFromResourcesAsync<T>(string respath) where T : Object {
        Object o = await LoadFromResourcesAsync(respath, typeof(T));
        return (T)o;
    }

    public static T LoadAsset<T>(string respath) where T : Object {
        Object o = LoadAsset(respath, typeof(T));
        return (T)o;
    }
    public static async UniTask<T> LoadAssetAsync<T>(string respath) where T : Object {
        Object o = await LoadAssetAsync(respath, typeof(T));
        return (T)o;
    }

    public static async UniTask LoadScene(string scenename, LoadSceneMode mode, bool async = false) {
        PreloadScene(scenename);
        if (async) {
            AsyncOperation req = SceneManager.LoadSceneAsync(scenename, mode);
            req.allowSceneActivation = true;
            await req;
        } else {
            SceneManager.LoadScene(scenename, mode);
        }
    }
}