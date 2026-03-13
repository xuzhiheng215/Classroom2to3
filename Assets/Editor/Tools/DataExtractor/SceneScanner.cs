using UnityEditor;
using UnityEngine;

public static class SceneScanner
{
    public static int GetSceneObjectCount()
    {
        return Object.FindObjectsOfType<GameObject>().Length;
    }

    public static void LogAllObjectNames()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            Debug.Log($"物体名称: {obj.name} - 位置: {obj.transform.position}");
        }
    }

    public static GameObject[] GetAllSceneObjects()
    {
        return Object.FindObjectsOfType<GameObject>();
    }

    public static GameObject[] GetObjectsByType(string typeName)
    {
        GameObject[] allObjects = GetAllSceneObjects();
        return System.Array.FindAll(allObjects, obj => 
            obj.name.Contains(typeName, System.StringComparison.OrdinalIgnoreCase));
    }

    public static Vector3 GetObjectPosition(GameObject obj)
    {
        return obj != null ? obj.transform.position : Vector3.zero;
    }

    public static string GetObjectHierarchy(GameObject obj)
    {
        if (obj == null) return string.Empty;
        
        string hierarchy = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            hierarchy = $"{parent.name}/{hierarchy}";
            parent = parent.parent;
        }
        return hierarchy;
    }
}