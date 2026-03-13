using UnityEditor;
using UnityEngine;

public static class ProgressDisplay
{
    private static float progress = 0f;
    private static bool isActive = false;
    private static string status = "";
    private static double startTime;
    private static double estimatedTime = 0;

    public static void StartProgress(string title, string initialStatus)
    {
        progress = 0f;
        status = $"{title}: {initialStatus}";
        isActive = true;
        startTime = EditorApplication.timeSinceStartup;
        estimatedTime = 0;
    }

    public static void UpdateProgress(float newProgress, string newStatus)
    {
        progress = Mathf.Clamp01(newProgress);
        status = newStatus;

        // 估算剩余时间
        if (progress > 0.1f)
        {
            double elapsed = EditorApplication.timeSinceStartup - startTime;
            estimatedTime = (elapsed / progress) * (1 - progress);
        }
    }

    public static void DrawProgressBar()
    {
        if (!isActive) return;

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("进度监控", EditorStyles.boldLabel);

        // 进度条
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(rect, progress, status);

        // 详细信息
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField($"进度: {Mathf.RoundToInt(progress * 100)}%",
            GUILayout.Width(80));

        if (estimatedTime > 0)
        {
            string timeText = $"预计剩余: {estimatedTime:F1}秒";
            EditorGUILayout.LabelField(timeText, EditorStyles.miniLabel);
        }

        GUILayout.FlexibleSpace();

        // 取消按钮
        if (GUILayout.Button("取消", GUILayout.Width(60)))
        {
            CancelProgress();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    public static void CompleteProgress(string finalMessage = "✅ 操作完成！")
    {
        progress = 1f;
        status = finalMessage;

        // 延迟清除
        EditorApplication.delayCall += () => {
            isActive = false;

            if (!string.IsNullOrEmpty(finalMessage) && !finalMessage.Contains("✅"))
            {
                EditorUtility.DisplayDialog("操作完成", finalMessage, "确定");
            }
        };
    }

    public static void CancelProgress()
    {
        isActive = false;
        progress = 0f;
        status = "已取消";

        EditorUtility.DisplayDialog("取消", "操作已取消", "确定");
    }

    public static bool IsActive => isActive;
}