using System;
using UnityEngine;

/// <summary>
/// singleton tool
/// lvchengyuan
/// </summary>
/// <typeparam name="T"></typeparam>

public class Singleton<T> : IDisposable where T : new()
{
    static Singleton()
    {

    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeGC() { }

    private void Dispose(Boolean disposing)
    {
        if (disposing)
            DisposeGC();

    }

    protected static T _Object = default(T);

    public static T Instance
    {
        get
        {
            if (null == _Object)
            {
                _Object = new T();
                if (null == _Object)
                {
                    UnityEngine.Debug.LogError("Error Create Singleton !" + _Object.GetType().ToString());
                }
            }
            return (_Object);
        }
        set { _Object = value; }
    }

    public static Boolean Instantiated { get { return (null != _Object); } }
}


public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
{
    private static T _S;

    public static T Instance
    {
        get
        {
            if (_S == null)
            {
                _S = (T)GameObject.FindObjectOfType(typeof(T));
                if (_S == null)
                {
                    GameObject instanceObject = new GameObject(typeof(T).Name);
                    _S = instanceObject.AddComponent<T>();

                    GameObject parent = GameObject.Find("Singleton");
                    if (parent == null)
                    {
                        parent = new GameObject("Singleton");
                        GameObject.DontDestroyOnLoad(parent);
                    }
                    parent.transform.SetAsLastSibling();
                    instanceObject.transform.parent = parent.transform;

                }
            }
            return _S;
        }
    }

    /*
    * 没有任何实现的函数，用于保证SingletonMonobehaviour在使用前已创建
    */
    public void Startup()
    {

    }

    private void Awake()
    {
        if (_S == null)
        {
            _S = this as T;
        }

        DontDestroyOnLoad(gameObject);
        Init();
    }

    protected virtual void Init()
    {

    }

    public void DestroySelf()
    {
        Dispose();
        SingletonMonobehaviour<T>._S = null;
        UnityEngine.Object.Destroy(gameObject);
    }

    public virtual void Dispose()
    {

    }

}
