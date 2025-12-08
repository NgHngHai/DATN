using UnityEngine;
using UnityEditor;


[InitializeOnLoad]
public class HierarchyShortcuts
{
    static HierarchyShortcuts()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
        {
            GameObject go = Selection.activeGameObject;
            if (go != null)
            {
                go.SetActive(!go.activeSelf);
                e.Use();
            }
        }
    }
}
