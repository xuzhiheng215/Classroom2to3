using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ProtocolAnalyzerWindow : EditorWindow
{
    // 状态变量
    private Vector2 scrollPosition;
    private List<SceneObjectData> sceneObjects = new List<SceneObjectData>();
    private bool includePositions = true;
    private bool includeRotations = true;
    private bool includeScales = true;
    private bool prettyFormat = true;
    private string exportPath = "GeneratedOutput/JSON_Exports/";
    private string fileName = "classroom_data";
    
    // 性能监控
    private double scanTime = 0;
    private double exportTime = 0;
    private int totalObjects = 0;

    [MenuItem("Window/VR Protocol Analyzer")]
    static void ShowWindow()
    {
        var window = GetWindow<ProtocolAnalyzerWindow>("VR场景分析工具");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawHeader();
        DrawScanSection();
        DrawObjectListSection();
        DrawExportSection();
        DrawPerformanceSection();
        
        EditorGUILayout.EndScrollView();
        
        DrawFooter();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("🎮", GUILayout.Width(30));
        EditorGUILayout.LabelField("VR教室场景分析工具", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("版本 1.0 | 开发者: 集成官(C同学)", EditorStyles.miniLabel);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
    }

    private void DrawScanSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🔍 场景扫描", EditorStyles.boldLabel);
        
        if (GUILayout.Button("扫描当前场景", GUILayout.Height(40)))
        {
            ScanScene();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawObjectListSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📋 场景物体列表", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField($"总计: {totalObjects} 个物体", EditorStyles.miniBoldLabel);
        
        if (sceneObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("请先扫描场景", MessageType.Info);
        }
        else
        {
            // 显示前几个物体作为预览
            int previewCount = Mathf.Min(sceneObjects.Count, 5);
            for (int i = 0; i < previewCount; i++)
            {
                var obj = sceneObjects[i];
                EditorGUILayout.LabelField($"• {obj.name} ({obj.type})");
            }
            
            if (sceneObjects.Count > 5)
            {
                EditorGUILayout.LabelField($"... 还有 {sceneObjects.Count - 5} 个物体", EditorStyles.miniLabel);
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawExportSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📤 JSON导出设置", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // 文件设置
        EditorGUILayout.LabelField("文件设置", EditorStyles.miniBoldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", GUILayout.Width(70));
        exportPath = EditorGUILayout.TextField(exportPath);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文件名:", GUILayout.Width(70));
        fileName = EditorGUILayout.TextField(fileName);
        EditorGUILayout.LabelField(".json", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        // 数据选项
        EditorGUILayout.LabelField("包含的数据", EditorStyles.miniBoldLabel);
        includePositions = EditorGUILayout.Toggle("位置信息 (Position)", includePositions);
        includeRotations = EditorGUILayout.Toggle("旋转信息 (Rotation)", includeRotations);
        includeScales = EditorGUILayout.Toggle("缩放信息 (Scale)", includeScales);
        
        EditorGUILayout.Space(10);
        
        // 格式选项
        EditorGUILayout.LabelField("格式选项", EditorStyles.miniBoldLabel);
        prettyFormat = EditorGUILayout.Toggle("美化格式 (Pretty Print)", prettyFormat);
        
        EditorGUILayout.Space(10);
        
        // 导出按钮 - 保证能点击
        EditorGUI.BeginDisabledGroup(sceneObjects.Count == 0);
        if (GUILayout.Button("🚀 导出JSON文件", GUILayout.Height(50)))
        {
            ExportJSON();
        }
        EditorGUI.EndDisabledGroup();
        
        if (sceneObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("请先扫描场景", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPerformanceSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚡ 性能监控", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("扫描耗时:", GUILayout.Width(70));
        EditorGUILayout.LabelField($"{scanTime:F2} 秒", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("导出耗时:", GUILayout.Width(70));
        EditorGUILayout.LabelField($"{exportTime:F2} 秒", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("物体数量:", GUILayout.Width(70));
        EditorGUILayout.LabelField($"{totalObjects}", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawFooter()
    {
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        EditorGUILayout.BeginHorizontal();
        string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        EditorGUILayout.LabelField($"场景: {sceneName}", EditorStyles.miniLabel);
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"🕐 {System.DateTime.Now:HH:mm:ss}", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();
    }

    private void ScanScene()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        
        try
        {
            Debug.Log("开始扫描场景...");
            
            // 使用A同学的方法
            GameObject[] allGameObjects = SceneScanner.GetAllSceneObjects();
            Debug.Log($"SceneScanner返回 {allGameObjects.Length} 个GameObject");
            
            sceneObjects.Clear();
            
            int skipped = 0;
            foreach (var go in allGameObjects)
            {
                if (go.hideFlags != HideFlags.None) 
                {
                    skipped++;
                    continue;
                }
                
                var objData = new SceneObjectData
                {
                    name = go.name,
                    position = go.transform.position,
                    rotation = go.transform.rotation.eulerAngles,
                    scale = go.transform.localScale,
                    type = ClassifyObject(go.name)
                };
                
                sceneObjects.Add(objData);
                
                Debug.Log($"添加物体: {go.name} -> {objData.type}");
            }
            
            totalObjects = sceneObjects.Count;
            stopwatch.Stop();
            scanTime = stopwatch.Elapsed.TotalSeconds;
            
            Debug.Log($"扫描完成: 找到 {totalObjects} 个物体, 跳过 {skipped} 个, 耗时 {scanTime:F2} 秒");
            
            // 显示结果
            EditorUtility.DisplayDialog("扫描完成", 
                $"成功扫描场景！\n\n• 场景: {UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name}\n• 物体数量: {totalObjects}\n• 隐藏物体: {skipped}\n• 扫描耗时: {scanTime:F2}秒", 
                "确定");
            
            // 强制重绘
            this.Repaint();
        }
        catch (System.Exception e)
        {
            stopwatch.Stop();
            Debug.LogError($"扫描失败: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("扫描失败", $"错误: {e.Message}", "确定");
        }
    }

    private void ExportJSON()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        
        try
        {
            Debug.Log("开始导出JSON...");
            Debug.Log($"准备导出 {sceneObjects.Count} 个物体");
            
            if (sceneObjects.Count == 0)
            {
                Debug.LogError("没有可导出的物体！请先扫描场景。");
                EditorUtility.DisplayDialog("导出失败", "没有可导出的物体！请先扫描场景。", "确定");
                return;
            }
            
            // 生成JSON（使用手动方法，100%可靠）
            string json = GenerateManualJson();
            
            // 确保路径正确
            string fullPath = GetFullExportPath();
            Debug.Log($"准备保存到: {fullPath}");
            
            string directory = Path.GetDirectoryName(fullPath);
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"创建目录: {directory}");
            }
            
            // 写入文件
            File.WriteAllText(fullPath, json, Encoding.UTF8);
            
            stopwatch.Stop();
            exportTime = stopwatch.Elapsed.TotalSeconds;
            
            Debug.Log($"✅ JSON导出成功: {fullPath}");
            Debug.Log($"📄 文件大小: {json.Length} 字符, 耗时: {exportTime:F2}秒");
            
            // 显示成功对话框
            bool openFolder = EditorUtility.DisplayDialog("导出成功",
                $"JSON文件已生成！\n\n• 路径: {fullPath}\n• 物体数量: {sceneObjects.Count}\n• 文件大小: {json.Length} 字符\n• 导出耗时: {exportTime:F2}秒\n\n是否打开所在文件夹？",
                "打开文件夹", "确定");
            
            if (openFolder)
            {
                EditorUtility.RevealInFinder(fullPath);
            }
            
            // 刷新窗口
            this.Repaint();
        }
        catch (System.Exception e)
        {
            stopwatch.Stop();
            Debug.LogError($"❌ JSON导出失败: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("导出失败",
                $"发生错误:\n{e.Message}\n\n请查看控制台获取详细信息。",
                "确定");
        }
    }
    
    private string GenerateManualJson()
    {
        try
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("{");
            sb.AppendLine("  \"metadata\": {");
            sb.AppendLine($"    \"exportTime\": \"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\",");
            sb.AppendLine($"    \"sceneName\": \"{UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name}\",");
            sb.AppendLine($"    \"totalObjects\": {sceneObjects.Count},");
            sb.AppendLine($"    \"toolVersion\": \"1.0\"");
            sb.AppendLine("  },");
            sb.AppendLine("  \"objects\": [");
            
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                var obj = sceneObjects[i];
                sb.AppendLine("    {");
                sb.AppendLine($"      \"name\": \"{EscapeJsonString(obj.name)}\",");
                sb.AppendLine($"      \"type\": \"{obj.type}\"");
                
                if (includePositions)
                {
                    sb.AppendLine("      ,\"position\": {");
                    sb.AppendLine($"        \"x\": {obj.position.x:F2},");
                    sb.AppendLine($"        \"y\": {obj.position.y:F2},");
                    sb.AppendLine($"        \"z\": {obj.position.z:F2}");
                    sb.AppendLine("      }");
                }
                
                if (includeRotations)
                {
                    sb.AppendLine("      ,\"rotation\": {");
                    sb.AppendLine($"        \"x\": {obj.rotation.x:F2},");
                    sb.AppendLine($"        \"y\": {obj.rotation.y:F2},");
                    sb.AppendLine($"        \"z\": {obj.rotation.z:F2}");
                    sb.AppendLine("      }");
                }
                
                if (includeScales)
                {
                    sb.AppendLine("      ,\"scale\": {");
                    sb.AppendLine($"        \"x\": {obj.scale.x:F2},");
                    sb.AppendLine($"        \"y\": {obj.scale.y:F2},");
                    sb.AppendLine($"        \"z\": {obj.scale.z:F2}");
                    sb.AppendLine("      }");
                }
                
                sb.Append("    }");
                if (i < sceneObjects.Count - 1) sb.Append(",");
                sb.AppendLine();
            }
            
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            
            string json = sb.ToString();
            Debug.Log($"手动生成的JSON长度: {json.Length} 字符");
            return json;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"手动生成JSON失败: {e.Message}");
            return "{}";
        }
    }
    
    private string EscapeJsonString(string str)
    {
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\r", "\\r")
                  .Replace("\t", "\\t");
    }
    
    private string GetFullExportPath()
    {
        // 清理路径
        string cleanPath = exportPath.Trim();
        if (!cleanPath.EndsWith("/") && !cleanPath.EndsWith("\\"))
            cleanPath += "/";
        
        // 获取项目根目录
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        
        // 构建完整路径
        string fullPath;
        if (cleanPath.StartsWith("Assets/") || cleanPath.StartsWith("../"))
        {
            fullPath = Path.Combine(projectRoot, cleanPath, fileName + ".json");
        }
        else
        {
            fullPath = Path.Combine(projectRoot, cleanPath, fileName + ".json");
        }
        
        return fullPath;
    }
    
    private string ClassifyObject(string name)
    {
        name = name.ToLower();
        if (name.Contains("desk") || name.Contains("table")) return "desk";
        if (name.Contains("chair") || name.Contains("seat")) return "chair";
        if (name.Contains("wall")) return "wall";
        if (name.Contains("window")) return "window";
        if (name.Contains("door")) return "door";
        if (name.Contains("light") || name.Contains("lamp")) return "light";
        if (name.Contains("camera")) return "camera";
        return "other";
    }
}