// Assets/Editor/Tools/Performance/PerformanceProfiler.cs
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;

public static class PerformanceProfiler
{
    private static Stopwatch stopwatch = new Stopwatch();
    
    // 扩展功能：性能数据记录
    private static StringBuilder performanceLog = new StringBuilder();
    private static long startMemory = 0;
    
    // 起步代码部分（保持原样）
    public static void StartTiming()
    {
        stopwatch.Start();
    }
    
    public static void StopAndLog()
    {
        stopwatch.Stop();
        Debug.Log($"操作耗时: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    // 扩展功能：增强计时功能
    public static void StartProfiling()
    {
        stopwatch.Restart();
        startMemory = System.GC.GetTotalMemory(false);
        performanceLog.Clear();
        performanceLog.AppendLine("=== 性能分析开始 ===");
        performanceLog.AppendLine($"开始时间: {System.DateTime.Now:HH:mm:ss}");
        performanceLog.AppendLine($"初始内存: {startMemory / (1024 * 1024)} MB");
    }
    
    public static void RecordCheckpoint(string checkpointName)
    {
        if (stopwatch.IsRunning)
        {
            long currentMemory = System.GC.GetTotalMemory(false);
            long memoryUsed = currentMemory - startMemory;
            double memoryMB = memoryUsed / (1024.0 * 1024.0);
            
            performanceLog.AppendLine($"[{checkpointName}]");
            performanceLog.AppendLine($"  耗时: {stopwatch.Elapsed.TotalSeconds:F2}秒");
            performanceLog.AppendLine($"  内存增量: {memoryMB:F2} MB");
            
            Debug.Log($"{checkpointName} - 耗时: {stopwatch.Elapsed.TotalSeconds:F2}秒, 内存: {memoryMB:F2} MB");
        }
    }
    
    public static void StopProfilingAndReport()
    {
        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
            
            long endMemory = System.GC.GetTotalMemory(false);
            long totalMemoryUsed = endMemory - startMemory;
            double totalMemoryMB = totalMemoryUsed / (1024.0 * 1024.0);
            
            performanceLog.AppendLine("=== 性能分析结束 ===");
            performanceLog.AppendLine($"总耗时: {stopwatch.Elapsed.TotalSeconds:F2}秒");
            performanceLog.AppendLine($"总内存增量: {totalMemoryMB:F2} MB");
            performanceLog.AppendLine($"结束时间: {System.DateTime.Now:HH:mm:ss}");
            
            // 保存性能报告
            SavePerformanceReport();
        }
    }
    
    public static string GetPerformanceSummary()
    {
        if (!stopwatch.IsRunning && stopwatch.Elapsed.TotalSeconds == 0)
        {
            return "性能数据: 未开始分析";
        }
        
        long currentMemory = System.GC.GetTotalMemory(false);
        long memoryUsed = currentMemory - startMemory;
        double memoryMB = memoryUsed / (1024.0 * 1024.0);
        
        return $"耗时: {stopwatch.Elapsed.TotalSeconds:F2}秒, 内存: {memoryMB:F2} MB";
    }
    
    public static string GetFormattedTime()
    {
        if (!stopwatch.IsRunning && stopwatch.Elapsed.TotalSeconds == 0)
        {
            return "0.00秒";
        }
        
        return $"{stopwatch.Elapsed.TotalSeconds:F2}秒";
    }
    
    public static string GetFormattedMemory()
    {
        if (startMemory == 0)
        {
            return "0.00 MB";
        }
        
        long currentMemory = System.GC.GetTotalMemory(false);
        long memoryUsed = currentMemory - startMemory;
        double memoryMB = memoryUsed / (1024.0 * 1024.0);
        
        return $"{memoryMB:F2} MB";
    }
    
    public static void LogPerformanceData(string context, int objectCount)
    {
        if (stopwatch.IsRunning)
        {
            string time = GetFormattedTime();
            string memory = GetFormattedMemory();
            Debug.Log($"[性能] {context}: {objectCount}物体, {time}, {memory}");
        }
    }
    
    private static void SavePerformanceReport()
    {
        string reportDir = "Assets/GeneratedOutput/Reports/";
        
        try
        {
            if (!Directory.Exists(reportDir))
            {
                Directory.CreateDirectory(reportDir);
            }
            
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string reportPath = $"{reportDir}performance_{timestamp}.txt";
            
            File.WriteAllText(reportPath, performanceLog.ToString());
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log($"性能报告已保存: {reportPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存性能报告失败: {e.Message}");
        }
    }
    
    // 工具方法：格式化性能信息显示
    public static string FormatPerformanceInfo(int scanned, int total)
    {
        string time = GetFormattedTime();
        string memory = GetFormattedMemory();
        return $"已扫描{scanned}/{total}物体，耗时{time}，内存{memory}";
    }
    
    // 工具方法：重置性能分析器
    public static void Reset()
    {
        stopwatch.Reset();
        startMemory = 0;
        performanceLog.Clear();
    }
    
    // 工具方法：检查性能状态
    public static bool IsProfilingRunning()
    {
        return stopwatch.IsRunning;
    }
    
    // 工具方法：获取性能数据用于优化建议
    public static (float timeSeconds, double memoryMB) GetPerformanceMetrics()
    {
        float timeSeconds = (float)stopwatch.Elapsed.TotalSeconds;
        
        double memoryMB = 0;
        if (startMemory > 0)
        {
            long currentMemory = System.GC.GetTotalMemory(false);
            memoryMB = (currentMemory - startMemory) / (1024.0 * 1024.0);
        }
        
        return (timeSeconds, memoryMB);
    }
}

