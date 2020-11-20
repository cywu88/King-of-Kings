using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorShortCutKeys : ScriptableObject
{
    [MenuItem("Edit/Launcher _F5")]
    static void PlayGame()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Launcher.unity", OpenSceneMode.Single);
            //EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
