using UnityEngine;
using UnityEditor;

public static class ClearEditorPrefs
{
    [MenuItem("Tools/Clear Editor Prefs")]
    static void Clear()
    {
        EditorPrefs.DeleteAll();
        Debug.Log("EditorPrefs cleared");
    }
}