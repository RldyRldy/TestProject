using UnityEngine;
using UnityEditor;
using System.IO;

public static class CustomEditorUtility
{
    // create empty scriptableobject
    public static void CreateAsset<T>(string name = "") where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        CreateAsset<T>(asset, name);
    }

    public static void CreateAsset<T>(T asset, string name = "") where T : ScriptableObject
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        if (name.Length <= 0)
        {
            name = "New " + typeof(T).Name;
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}