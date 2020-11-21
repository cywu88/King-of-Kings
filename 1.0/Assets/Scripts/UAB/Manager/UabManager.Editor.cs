#if UNITY_EDITOR && !USE_UAB
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UniRx.Async;
using System;

using Object = UnityEngine.Object;

public partial class UabManager {
    private static Dictionary<string, string> editorResmap;
    private static void initEditorResMap(string root) {
        string[] files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
        foreach (string file in files) {
            string filepath = file.Replace('\\', '/');
            if (filepath.EndsWith(".meta") ||
                filepath.EndsWith(".overrideController") ||
                (filepath.EndsWith(".asset") && !filepath.Contains("Font"))
                ) {
                continue;
            } else {
                string loadpath = filepath.Substring(root.Length + 1);
                loadpath = Path.ChangeExtension(loadpath, null).ToLower();
                if (editorResmap.ContainsKey(loadpath)) {
                    if (showLog)
                        UabLog.D(UabLog.LogPrefix.Manager, "already exist pathname", filepath, loadpath, editorResmap[loadpath]);
                } else {
                    editorResmap.Add(loadpath, filepath);
                }
            }
        }
    }

    private static string getEditorAssetPath(string respath) {
        respath = respath.Replace('.', '/');
        respath = respath.ToLower();
        if (editorResmap.ContainsKey(respath)) {
            return editorResmap[respath];
        } else {
            UabLog.E(UabLog.LogPrefix.Manager, "editorResmap didn't contain res", respath);
            return null;
        }
    }

    private static Object loadFromEditor(string respath, Type specifyType = null) {
        Object rtn = null;
        string assetpath = getEditorAssetPath(respath);
        if (assetpath != null) {
            if (specifyType == null) {
                rtn = AssetDatabase.LoadMainAssetAtPath(assetpath);
            } else {
                rtn = AssetDatabase.LoadAssetAtPath(assetpath, specifyType);
            }
        }
        return rtn;
    }

    public static async UniTask Initialize(Action<bool> callback = null) {
        editorResmap = new Dictionary<string, string>();
        initEditorResMap(UabDef.RESOURCES_AB_ROOT);
        initEditorResMap(UabDef.RESOURCES_ROOT);
        initialized = true;
        UabLog.D(UabLog.LogPrefix.Manager, "done load editor res map...");
    }

    public static Object LoadAsset(string respath, Type specifyType = null) {
        respath = processResPath(respath);
        Object rtn = loadFromEditor(respath, specifyType);
        if (rtn == null) {
            rtn = LoadFromResources(respath, specifyType);
        }
        return rtn;
    }

    public static async UniTask<Object> LoadAssetAsync(string respath, Type specifyType = null) {
        respath = processResPath(respath);
        Object rtn = loadFromEditor(respath, specifyType);
        if (rtn == null) {
            rtn = await LoadFromResourcesAsync(respath, specifyType);
        }
        return rtn;
    }

    public static void PreloadScene(string scenename) { }
    public static void UnLoadSceneAB(string scenename) { }

    public static byte[] LoadLua(string luamodule) {
        luamodule = processResPath(luamodule);
        byte[] bt = null;
        string luapath = $"Assets/Lua/{luamodule}.lua";
        if (File.Exists(luapath)) {
            bt = File.ReadAllBytes(luapath);
        } else {
            UabLog.E(UabLog.LogPrefix.Manager, $"can not load lua from editor [ {luamodule} ]");
        }
        if (bt == null) {
            TextAsset ta = LoadFromResources<TextAsset>($"Lua/{luamodule}");
            if (ta != null) {
                bt = ta.bytes;
            }
        }
        return bt;
    }

    public static void UnloadAll(bool _) {
        if (editorResmap != null) {
            editorResmap.Clear();
        }
    }

    public static string GetVideoPath(string vname) {
        return getEditorAssetPath(vname);
    }

    public static void TryUnloadAsset(string _) { }
    public static void Hot() {
    }

    public static void LoadAtlas(string respath, List<Sprite> sps = null, List<Texture2D> texs = null) {
        string path = UabFileUtil.PathCombine(UabDef.RESOURCES_AB_ROOT, respath);
        path = processResPath(path);
        string[] pngfiles = Directory.GetFiles(path, "*.png");
        foreach (string file in pngfiles) {
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(file);
            for (int i = 0; i < objs.Length; i++) {
                if (objs[i].GetType() == typeof(Sprite) && sps != null) {
                    sps.Add(objs[i] as Sprite);
                } else if (objs[i].GetType() == typeof(Texture2D) && texs != null) {
                    texs.Add(objs[i] as Texture2D);
                }
            }
        }
    }

    public static void LoadHotfix(Action<bool> callback = null) {
        callback?.Invoke(true);
    }

    public static void Update(float deltaTime) {}
}
#endif