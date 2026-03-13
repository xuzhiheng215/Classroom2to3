using UnityEngine;

public static class ObjectClassifier
{
    public enum ObjectType
    {
        Unknown,
        Desk,
        Chair,
        Blackboard,
        Bookcase,
        Clock,
        Globe,
        Light,
        Door,
        Window,
        Shelf,
        TeacherDesk,
        AirConditioner,
        Noticeboard,
        Pin,
        Brush,
        Chalk,
        Books
    }

    public static ObjectType ClassifyObject(GameObject obj)
    {
        if (obj == null) return ObjectType.Unknown;
        
        string objectName = obj.name.ToLower();
        
        if (objectName.Contains("desk"))
        {
            if (objectName.Contains("teacher"))
                return ObjectType.TeacherDesk;
            return ObjectType.Desk;
        }
        
        if (objectName.Contains("chair"))
            return ObjectType.Chair;
        
        if (objectName.Contains("blackboard"))
            return ObjectType.Blackboard;
        
        if (objectName.Contains("bookcase"))
            return ObjectType.Bookcase;
        
        if (objectName.Contains("clock"))
            return ObjectType.Clock;
        
        if (objectName.Contains("globe"))
            return ObjectType.Globe;
        
        if (objectName.Contains("light"))
            return ObjectType.Light;
        
        if (objectName.Contains("door"))
            return ObjectType.Door;
        
        if (objectName.Contains("window"))
            return ObjectType.Window;
        
        if (objectName.Contains("shelf"))
            return ObjectType.Shelf;
        
        if (objectName.Contains("airconditioner") || objectName.Contains("ac") || objectName.Contains("vent"))
            return ObjectType.AirConditioner;
        
        if (objectName.Contains("noticeboard"))
            return ObjectType.Noticeboard;
        
        if (objectName.Contains("pin"))
            return ObjectType.Pin;
        
        if (objectName.Contains("brush"))
            return ObjectType.Brush;
        
        if (objectName.Contains("chalk"))
            return ObjectType.Chalk;
        
        if (objectName.Contains("book"))
            return ObjectType.Books;
        
        return ObjectType.Unknown;
    }

    public static string GetTypeName(ObjectType type)
    {
        return type.ToString();
    }

    public static int GetTypeCount(ObjectType type, GameObject[] allObjects)
    {
        int count = 0;
        foreach (var obj in allObjects)
        {
            if (ClassifyObject(obj) == type)
                count++;
        }
        return count;
    }

    public static GameObject[] GetObjectsByType(ObjectType type, GameObject[] allObjects)
    {
        return System.Array.FindAll(allObjects, obj => ClassifyObject(obj) == type);
    }

    public static void LogObjectTypes(GameObject[] allObjects)
    {
        System.Collections.Generic.Dictionary<ObjectType, int> typeCount = new System.Collections.Generic.Dictionary<ObjectType, int>();
        
        foreach (var obj in allObjects)
        {
            ObjectType type = ClassifyObject(obj);
            if (typeCount.ContainsKey(type))
                typeCount[type]++;
            else
                typeCount[type] = 1;
        }
        
        Debug.Log("Object Type Classification Results:");
        foreach (var kvp in typeCount)
        {
            Debug.Log($"{GetTypeName(kvp.Key)}: {kvp.Value} objects");
        }
    }
}