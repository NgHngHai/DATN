#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class CustomShortcuts 
{
    [MenuItem("Custom Shortcuts/Open Save Folder")]
    public static void OpenSaveFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Custom Shortcuts/Delete Save File")]
    public static void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/game_save.dat";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Deleted Save File!");
        }
        else
        {
            Debug.Log("No Save File to delete.");
        }
    }
    
    [MenuItem("Custom Shortcuts/At <Scene> Tab/Toggle Scene 2D - 3D %#t")]
    private static void Toggle2D()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        sceneView.in2DMode = !sceneView.in2DMode;
        sceneView.Repaint();
    }


    [MenuItem("Custom Shortcuts/At <Scene> Tab/Toggle Gizmos %#y")]
    private static void ToggleGizmos()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        sceneView.drawGizmos = !sceneView.drawGizmos;
        sceneView.Repaint();
    }


    [MenuItem("Custom Shortcuts/Navigate/to <Scripts Folder> %&#1", false, 1)]
    public static void ExpandScriptsFolder()
    {
        string folderPath = "Assets/Scripts";

        // Enumerator: chỉ lấy 1 entry đầu tiên, không duyệt hết
        string entry = Directory.EnumerateFileSystemEntries(folderPath).FirstOrDefault();

        if (string.IsNullOrEmpty(entry))
        {
            Debug.LogWarning("No entries found in: " + folderPath);
            return;
        }

        string assetPath = entry.Replace("\\", "/");
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
        else
        {
            Debug.LogWarning("Could not load asset: " + assetPath);
        }
    }

    [MenuItem("Custom Shortcuts/Navigate/to <Sprites Folder> %&#2", false, 2)]
    public static void ExpandSpritesFolder()
    {
        string folderPath = "Assets/Sprites";

        // Enumerator: chỉ lấy 1 entry đầu tiên, không duyệt hết
        string entry = Directory.EnumerateFileSystemEntries(folderPath).FirstOrDefault();

        if (string.IsNullOrEmpty(entry))
        {
            Debug.LogWarning("No entries found in: " + folderPath);
            return;
        }

        string assetPath = entry.Replace("\\", "/");
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }

    [MenuItem("Custom Shortcuts/Navigate/to <Prefabs Folder> %&#3", false, 3)]
    public static void ExpandPrefabsFolder()
    {
        string folderPath = "Assets/Prefabs";
        string entry = Directory.EnumerateFileSystemEntries(folderPath).FirstOrDefault();

        if (string.IsNullOrEmpty(entry))
        {
            return;
        }

        string assetPath = entry.Replace("\\", "/");
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }

    [MenuItem("Custom Shortcuts/Navigate/to <Scenes Folder> %&#4", false, 4)]
    public static void ExpandScenesFolder()
    {
        string folderPath = "Assets/Scenes";
        string entry = Directory.EnumerateFileSystemEntries(folderPath).FirstOrDefault();

        if (string.IsNullOrEmpty(entry))
        {
            return;
        }

        string assetPath = entry.Replace("\\", "/");
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }
}
#endif