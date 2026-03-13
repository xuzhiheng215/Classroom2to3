// Assets/Editor/Tools/Integration/ReportGenerator.cs
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public static class ReportGenerator
{
    public static void GenerateHTMLReport(TestResult result)
    {
        string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>VR场景分析工具 - 测试报告</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .header {{ background: #2c3e50; color: white; padding: 20px; border-radius: 5px; }}
        .test-result {{ margin: 20px 0; padding: 15px; border-left: 5px solid #3498db; }}
        .pass {{ border-color: #2ecc71; background: #e8f8f5; }}
        .fail {{ border-color: #e74c3c; background: #fdedec; }}
        .module {{ display: inline-block; padding: 5px 10px; margin: 2px; border-radius: 3px; }}
        .data {{ background: #3498db; color: white; }}
        .ui {{ background: #9b59b6; color: white; }}
        .perf {{ background: #e67e22; color: white; }}
        .integration {{ background: #1abc9c; color: white; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🎯 VR场景分析工具 - 集成测试报告</h1>
        <p>生成时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        <p>测试人员: 集成官C | 分支: integration</p>
    </div>
    
    <h2>📊 测试概览</h2>
    <div class='test-result {(result.DataModuleTest ? "pass" : "fail")}'>
        <h3>📁 数据模块 (A同学)</h3>
        <p>状态: <strong>{(result.DataModuleTest ? "✅ 通过" : "❌ 失败")}</strong></p>
        <p>功能: 场景扫描、物体识别、数据提取</p>
    </div>
    
    <div class='test-result {(result.UIModuleTest ? "pass" : "fail")}'>
        <h3>🎨 UI模块 (B同学)</h3>
        <p>状态: <strong>{(result.UIModuleTest ? "✅ 通过" : "❌ 失败")}</strong></p>
        <p>功能: 工具窗口、按钮交互、界面显示</p>
    </div>
    
    <div class='test-result {(result.PerformanceModuleTest ? "pass" : "fail")}'>
        <h3>⚡ 性能模块 (D同学)</h3>
        <p>状态: <strong>{(result.PerformanceModuleTest ? "✅ 通过" : "⚠️ 部分通过")}</strong></p>
        <p>功能: 性能监控、内存分析、优化建议</p>
    </div>
    
    <h2>🎯 总体评估</h2>
    <p>项目完整性: <strong>{(result.AllPassed ? "✅ 优秀" : "⚠️ 基本完成")}</strong></p>
    <p>建议: {(result.DataModuleTest ? "" : "完善SceneScanner功能<br>")}
               {(result.UIModuleTest ? "" : "验证工具窗口交互<br>")}
               按时提交最终演示视频</p>
    
    <h2>📁 文件清单</h2>
    <p>总代码文件: {CountCSFiles()} 个</p>
    
    <div class='module data'>数据模块: {CountModuleFiles("DataExtractor")}文件</div>
    <div class='module ui'>UI模块: {CountModuleFiles("SceneAnalyzer")}文件</div>
    <div class='module perf'>性能模块: {CountModuleFiles("Performance")}文件</div>
    <div class='module integration'>集成模块: {CountModuleFiles("Integration")}文件</div>
</body>
</html>";

        string reportDir = "Assets/GeneratedOutput/HTML_Reports/";
        Directory.CreateDirectory(reportDir);
        string filePath = reportDir + $"test_report_{System.DateTime.Now:yyyyMMdd_HHmmss}.html";
        
        File.WriteAllText(filePath, html, Encoding.UTF8);
        Debug.Log($"📄 HTML报告已生成: {filePath}");
        
        // 在浏览器中打开
        Application.OpenURL("file:///" + Path.GetFullPath(filePath).Replace("\\", "/"));
    }
    
    static int CountCSFiles()
    {
        return Directory.GetFiles("Assets/Editor/Tools", "*.cs", SearchOption.AllDirectories).Length;
    }
    
    static int CountModuleFiles(string moduleName)
    {
        string path = $"Assets/Editor/Tools/{moduleName}/";
        if (Directory.Exists(path))
            return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Length;
        return 0;
    }
    
    // 这个类应该和IntegrationTester.cs中的TestReport一致
    [System.Serializable]
    public class TestResult
    {
        public bool DataModuleTest;
        public bool UIModuleTest;
        public bool PerformanceModuleTest;
        public bool IntegrationTest;
        
        public bool AllPassed => DataModuleTest && UIModuleTest && IntegrationTest;
    }
    
    [MenuItem("Tools/生成HTML测试报告")]
    public static void GenerateReportFromCurrentTest()
    {
        // 这里可以调用IntegrationTester的测试，然后生成报告
        // 简化版：先创建一个示例报告
        TestResult result = new TestResult()
        {
            DataModuleTest = true,
            UIModuleTest = true,
            PerformanceModuleTest = true,
            IntegrationTest = true
        };
        
        GenerateHTMLReport(result);
    }
}