// Assets/Editor/Tools/Performance/OptimizationSuggestor.cs
using UnityEngine;
using System.Collections.Generic;

public static class OptimizationSuggestor
{
    public static List<string> GenerateSuggestions(int totalObjects, float elapsedSeconds, double memoryMB)
    {
        List<string> suggestions = new List<string>();

        // 基于物体数量的建议
        if (totalObjects > 1000)
        {
            suggestions.Add("• 场景物体过多，建议分批处理（每批不超过500个）");
        }
        else if (totalObjects > 500)
        {
            suggestions.Add("• 物体数量适中，但建议使用对象池管理频繁创建/销毁的对象");
        }

        // 基于耗时的建议
        if (elapsedSeconds > 10)
        {
            suggestions.Add("• 扫描耗时过长，建议使用异步任务或协程");
        }
        else if (elapsedSeconds > 5)
        {
            suggestions.Add("• 扫描时间较长，可以考虑优化查找算法");
        }

        // 基于内存的建议
        if (memoryMB > 100)
        {
            suggestions.Add("• 内存使用较高，建议及时清理无用资源");
        }
        else if (memoryMB > 50)
        {
            suggestions.Add("• 内存使用适中，但长时间运行需关注内存增长");
        }

        // 如果没有特别需要优化的地方
        if (suggestions.Count == 0)
        {
            suggestions.Add("• 当前性能表现良好，无需特殊优化");
        }

        return suggestions;
    }

    public static void LogSuggestions(List<string> suggestions)
    {
        Debug.Log("=== 优化建议 ===");
        foreach (var suggestion in suggestions)
        {
            Debug.Log(suggestion);
        }
    }
}