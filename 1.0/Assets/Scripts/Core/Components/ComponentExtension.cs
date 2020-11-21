using UnityEngine;
using XLua;

[LuaCallCSharp]
public static partial class ComponentExtension
{
    public static void AddParent(this Transform t, Transform parent, float size = 1f)
    {
        if (parent == null)
        {
            return;
        }

        t.SetParent(parent);
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one * size;
        t.localEulerAngles = Vector3.zero;

        RectTransform rt = t.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
