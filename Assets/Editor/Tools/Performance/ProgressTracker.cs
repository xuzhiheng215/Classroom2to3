// Assets/Editor/Tools/Performance/ProgressTracker.cs
using UnityEngine;

public static class ProgressTracker
{
    private static int totalObjects = 0;
    private static int scannedObjects = 0;

    public static void Reset(int total)
    {
        totalObjects = total;
        scannedObjects = 0;
    }

    public static void Update(int scanned)
    {
        scannedObjects = scanned;
    }

    public static string GetProgressText()
    {
        if (totalObjects == 0) return "准备扫描...";
        
        float percentage = (float)scannedObjects / totalObjects * 100f;
        return $"已扫描 {scannedObjects}/{totalObjects} 物体 ({percentage:F1}%)";
    }

    public static float GetProgressValue()
    {
        if (totalObjects == 0) return 0f;
        return (float)scannedObjects / totalObjects;
    }
}