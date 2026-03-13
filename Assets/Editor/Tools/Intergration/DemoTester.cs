// Assets/Editor/Tools/Integration/DemoTester.cs
using UnityEditor;
using UnityEngine;

public class DemoTester
{
    [MenuItem("Tools/VR工具演示模式")]
    static void StartDemoMode()
    {
        Debug.Log("🎬 ===============================");
        Debug.Log("🎬 VR场景分析工具 - 演示模式启动");
        Debug.Log("🎬 ===============================");
        Debug.Log("");
        Debug.Log("演示步骤:");
        Debug.Log("1. 打开工具窗口: Window → VR Protocol Analyzer");
        Debug.Log("2. 点击'扫描场景'按钮");
        Debug.Log("3. 查看物体数量显示");
        Debug.Log("4. (可选)点击'导出JSON'");
        Debug.Log("");
        Debug.Log("📹 录制建议:");
        Debug.Log("- 镜头1: 菜单打开工具窗口 (5秒)");
        Debug.Log("- 镜头2: 点击扫描按钮 (5秒)");
        Debug.Log("- 镜头3: 显示扫描结果 (5秒)");
        Debug.Log("- 镜头4: 展示代码结构 (5秒)");
        Debug.Log("");
        Debug.Log("✅ 演示说明已输出到Console");
        
        // 自动打开工具窗口
        EditorApplication.ExecuteMenuItem("Window/VR Protocol Analyzer");
    }
}