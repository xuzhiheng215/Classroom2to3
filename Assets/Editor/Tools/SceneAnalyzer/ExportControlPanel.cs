using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class ExportControlPanel
{
    private static bool isExporting = false;
    private static float exportProgress = 0f;
    private static string exportStatus = "";
    private static double startTime;

    // 导出设置
    private static string exportPath = "GeneratedOutput/JSON_Exports/";
    private static string fileName = "classroom_data";
    private static bool includePositions = true;
    private static bool includeRotations = true;
    private static bool includeScales = true;
    private static bool prettyFormat = true;

    public static void DrawPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📤 JSON导出设置", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        DrawFileSettings();
        EditorGUILayout.Space(10);
        DrawDataOptions();
        EditorGUILayout.Space(10);
        DrawFormatOptions();
        EditorGUILayout.Space(10);
        DrawExportButton();
        EditorGUILayout.EndVertical();
    }

    private static void DrawFileSettings()
    {
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
    }

    private static void DrawDataOptions()
    {
        EditorGUILayout.LabelField("包含的数据", EditorStyles.miniBoldLabel);
        includePositions = EditorGUILayout.Toggle("位置信息 (Position)", includePositions);
        includeRotations = EditorGUILayout.Toggle("旋转信息 (Rotation)", includeRotations);
        includeScales = EditorGUILayout.Toggle("缩放信息 (Scale)", includeScales);
    }

    private static void DrawFormatOptions()
    {
        EditorGUILayout.LabelField("格式选项", EditorStyles.miniBoldLabel);
        prettyFormat = EditorGUILayout.Toggle("美化格式 (Pretty Print)", prettyFormat);
        EditorGUILayout.LabelField("缩进: 2个空格", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("编码: UTF-8", EditorStyles.miniLabel);
    }

    private static void DrawExportButton()
    {
        if (GUILayout.Button("🚀 开始导出JSON数据", GUILayout.Height(40)))
        {
            StartExport();
        }
    }

    private static void StartExport()
    {
        isExporting = true;
        exportProgress = 0f;
        exportStatus = "正在准备导出...";
        startTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += UpdateExportProgress;
    }

    private static void UpdateExportProgress()
    {
        if (!isExporting) return;

        try
        {
            if (exportProgress < 0.3f)
            {
                exportProgress += 0.015f;
                exportStatus = $"正在收集场景数据... {Mathf.RoundToInt(exportProgress * 100)}%";
                return;
            }
            else if (exportProgress < 0.6f)
            {
                // 实际执行导出
                PerformActualExport();
                exportProgress = 0.9f;
                exportStatus = "正在写入文件... 90%";
                return;
            }
            else if (exportProgress < 1f)
            {
                exportProgress += 0.015f;
                exportStatus = $"正在写入文件... {Mathf.RoundToInt(exportProgress * 100)}%";
                return;
            }
            else
            {
                exportProgress = 1f;
                exportStatus = "✅ 导出完成！";
                isExporting = false;
                EditorApplication.update -= UpdateExportProgress;

                string fullPath = GetFullExportPath();
                bool openFolder = EditorUtility.DisplayDialog("导出成功",
                    $"JSON文件已保存到:\n{fullPath}\n\n是否打开所在文件夹？",
                    "打开文件夹", "确定");

                if (openFolder)
                {
                    string folderPath = Path.GetDirectoryName(fullPath);
                    EditorUtility.RevealInFinder(folderPath);
                }
            }
        }
        catch (System.Exception e)
        {
            isExporting = false;
            EditorApplication.update -= UpdateExportProgress;
            Debug.LogError($"导出失败: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("导出失败",
                $"发生错误:\n{e.Message}\n\n请查看控制台获取详细信息。",
                "确定");
        }
    }

    private static void PerformActualExport()
    {
        Debug.Log("=== 开始实际导出 ===");
        
        // 使用A同学的方法获取所有物体
        GameObject[] allObjects = SceneScanner.GetAllSceneObjects();
        Debug.Log($"获取到 {allObjects.Length} 个游戏物体");
        
        // 转换为数据列表
        List<SceneObjectData> sceneObjects = new List<SceneObjectData>();
        foreach (var go in allObjects)
        {
            if (go.hideFlags != HideFlags.None) continue;
            
            sceneObjects.Add(new SceneObjectData
            {
                name = go.name,
                position = go.transform.position,
                rotation = go.transform.rotation.eulerAngles,
                scale = go.transform.localScale,
                type = ClassifyObject(go.name)
            });
        }
        
        Debug.Log($"处理了 {sceneObjects.Count} 个有效物体");
        
        // 构建JSON数据
        var exportData = new
        {
            metadata = new
            {
                exportTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name,
                totalObjects = sceneObjects.Count
            },
            objects = sceneObjects
        };
        
        // 生成JSON
        string json = JsonUtility.ToJson(exportData, prettyFormat);
        
        // 保存文件
        string fullPath = GetFullExportPath();
        string directory = Path.GetDirectoryName(fullPath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log($"创建目录: {directory}");
        }
        
        File.WriteAllText(fullPath, json, Encoding.UTF8);
        Debug.Log($"✅ JSON文件已保存: {fullPath}");
        Debug.Log($"📄 文件大小: {json.Length} 字符");
        
        // 刷新AssetDatabase（如果文件在Assets目录内）
        if (fullPath.Contains("/Assets/"))
        {
            AssetDatabase.Refresh();
        }
    }
    
    private static string GetFullExportPath()
    {
        // 确保路径正确
        string cleanPath = exportPath.Trim();
        if (!cleanPath.EndsWith("/") && !cleanPath.EndsWith("\\"))
            cleanPath += "/";
        
        string fullPath;
        if (cleanPath.StartsWith("Assets/") || cleanPath.StartsWith("../"))
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            fullPath = Path.Combine(projectRoot, cleanPath, fileName + ".json");
        }
        else if (Path.IsPathRooted(cleanPath))
        {
            fullPath = Path.Combine(cleanPath, fileName + ".json");
        }
        else
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            fullPath = Path.Combine(projectRoot, cleanPath, fileName + ".json");
        }
        
        return fullPath;
    }
    
    private static string ClassifyObject(string name)
    {
        if (name.StartsWith("Desk_")) return "desk";
        if (name.StartsWith("Chair_")) return "chair";
        if (name.StartsWith("Window_")) return "window";
        if (name.StartsWith("Door_")) return "door";
        if (name.StartsWith("Wall_")) return "wall";
        return "other";
    }

    public static void DrawProgress()
    {
        if (!isExporting) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📊 导出进度", EditorStyles.boldLabel);

        Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(progressRect, exportProgress, exportStatus);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"进度: {Mathf.RoundToInt(exportProgress * 100)}%", GUILayout.Width(80));

        if (exportProgress > 0.1f)
        {
            double elapsed = EditorApplication.timeSinceStartup - startTime;
            double estimatedTime = (elapsed / exportProgress) * (1 - exportProgress);
            EditorGUILayout.LabelField($"预计剩余: {estimatedTime:F1}秒", EditorStyles.miniLabel);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("取消", GUILayout.Width(60)))
        {
            CancelExport();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private static void CancelExport()
    {
        isExporting = false;
        exportProgress = 0f;
        exportStatus = "导出已取消";
        EditorApplication.update -= UpdateExportProgress;
        EditorUtility.DisplayDialog("取消", "JSON导出已取消", "确定");
    }
}

[System.Serializable]
public class SceneObjectData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string type;
}