// Assets/Editor/Tools/Performance/MemoryMonitor.cs
using UnityEngine;

public static class MemoryMonitor
{
    private static long initialMemory = 0;

    public static void Initialize()
    {
        initialMemory = System.GC.GetTotalMemory(false);
    }

    public static string GetMemoryUsage()
    {
        long currentMemory = System.GC.GetTotalMemory(false);
        long usedMemory = currentMemory - initialMemory;
        double memoryMB = usedMemory / (1024.0 * 1024.0);
        return $"{memoryMB:F2} MB";
    }

    public static void LogMemoryUsage(string context)
    {
        Debug.Log($"{context} - 内存使用: {GetMemoryUsage()}");
    }
}