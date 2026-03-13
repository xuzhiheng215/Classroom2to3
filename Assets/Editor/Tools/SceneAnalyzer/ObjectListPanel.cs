using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ObjectListPanel
{
    private static Vector2 scrollPosition;
    private static string searchText = "";
    private static int selectedTypeIndex = 0;
    private static string[] objectTypes = new string[] { "全部", "桌子", "椅子", "墙壁", "灯光", "其他" };

    private static List<GameObject> sceneObjects = new List<GameObject>();
    private static Dictionary<string, int> typeCounts = new Dictionary<string, int>();

    public static void DrawPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // 标题和统计
        DrawHeader();

        EditorGUILayout.Space(10);

        // 过滤控制
        DrawFilterControls();

        EditorGUILayout.Space(10);

        // 物体列表
        DrawObjectList();

        EditorGUILayout.EndVertical();
    }

    private static void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("📋 场景物体列表", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField($"总计: {sceneObjects.Count} 个物体",
            EditorStyles.miniBoldLabel);

        EditorGUILayout.EndHorizontal();
    }

    private static void DrawFilterControls()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("🔍 筛选条件", EditorStyles.miniBoldLabel);

        EditorGUILayout.BeginHorizontal();

        // 类型筛选
        EditorGUILayout.LabelField("类型:", GUILayout.Width(40));
        selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, objectTypes,
            GUILayout.Width(100));

        GUILayout.FlexibleSpace();

        // 搜索框
        EditorGUILayout.LabelField("搜索:", GUILayout.Width(40));
        searchText = EditorGUILayout.TextField(searchText,
            GUILayout.Width(150));

        EditorGUILayout.EndHorizontal();

        // 类型统计
        if (typeCounts.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var kvp in typeCounts)
            {
                string icon = GetTypeIcon(kvp.Key);
                EditorGUILayout.LabelField($"{icon} {kvp.Key}: {kvp.Value}",
                    GUILayout.Width(80));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private static void DrawObjectList()
    {
        if (sceneObjects.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "场景物体列表为空\n请点击'扫描场景'按钮加载场景物体",
                MessageType.Info);
            return;
        }

        var filteredObjects = GetFilteredObjects();

        if (filteredObjects.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "没有找到符合条件的物体\n请调整筛选条件",
                MessageType.Warning);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
            GUILayout.Height(250));

        // 表头
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        EditorGUILayout.LabelField("物体", GUILayout.Width(200));
        EditorGUILayout.LabelField("类型", GUILayout.Width(80));
        EditorGUILayout.LabelField("位置 (X,Y,Z)", GUILayout.Width(120));
        EditorGUILayout.LabelField("操作", GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        // 列表内容
        for (int i = 0; i < filteredObjects.Count; i++)
        {
            var obj = filteredObjects[i];

            EditorGUILayout.BeginHorizontal();

            // 名称
            string icon = GetTypeIcon(GetObjectType(obj.name));
            EditorGUILayout.LabelField($"{icon} {obj.name}",
                GUILayout.Width(200));

            // 类型
            string type = GetObjectType(obj.name);
            EditorGUILayout.LabelField(type, GUILayout.Width(80));

            // 位置
            Vector3 pos = obj.transform.position;
            EditorGUILayout.LabelField($"{pos.x:F0},{pos.y:F0},{pos.z:F0}",
                GUILayout.Width(120));

            // 定位按钮
            if (GUILayout.Button("📍", GUILayout.Width(30)))
            {
                Selection.activeGameObject = obj;
                EditorGUIUtility.PingObject(obj);
                if (SceneView.lastActiveSceneView != null)
                    SceneView.lastActiveSceneView.FrameSelected();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // 显示数量
        EditorGUILayout.LabelField(
            $"显示 {filteredObjects.Count} / {sceneObjects.Count} 个物体",
            EditorStyles.centeredGreyMiniLabel);
    }

    private static List<GameObject> GetFilteredObjects()
    {
        List<GameObject> result = new List<GameObject>();

        string selectedType = selectedTypeIndex > 0 ?
            objectTypes[selectedTypeIndex] : null;

        foreach (var obj in sceneObjects)
        {
            // 类型过滤
            if (selectedType != null)
            {
                string objType = GetObjectType(obj.name);
                if (objType != selectedType)
                    continue;
            }

            // 名称搜索
            if (!string.IsNullOrEmpty(searchText) &&
                !obj.name.ToLower().Contains(searchText.ToLower()))
                continue;

            result.Add(obj);
        }

        return result;
    }

    private static string GetObjectType(string objectName)
    {
        if (objectName.Contains("Desk") || objectName.Contains("desk") ||
            objectName.Contains("Table") || objectName.Contains("table"))
            return "桌子";
        if (objectName.Contains("Chair") || objectName.Contains("chair") ||
            objectName.Contains("Seat") || objectName.Contains("seat"))
            return "椅子";
        if (objectName.Contains("Wall") || objectName.Contains("wall"))
            return "墙壁";
        if (objectName.Contains("Light") || objectName.Contains("light") ||
            objectName.Contains("Lamp") || objectName.Contains("lamp"))
            return "灯光";
        return "其他";
    }

    private static string GetTypeIcon(string type)
    {
        switch (type)
        {
            case "桌子": return "🪑";
            case "椅子": return "💺";
            case "墙壁": return "🧱";
            case "灯光": return "💡";
            default: return "📦";
        }
    }

    public static void UpdateObjectList()
    {
        sceneObjects.Clear();
        typeCounts.Clear();

        var allObjects = Object.FindObjectsOfType<GameObject>();

        foreach (var obj in allObjects)
        {
            sceneObjects.Add(obj);

            string type = GetObjectType(obj.name);
            if (typeCounts.ContainsKey(type))
                typeCounts[type]++;
            else
                typeCounts[type] = 1;
        }
    }

    public static int GetObjectCount()
    {
        return sceneObjects.Count;
    }
}