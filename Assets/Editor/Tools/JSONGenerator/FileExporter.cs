using UnityEngine;
using System.IO;
using UnityEditor;

public static class FileExporter
{
    private const string GENERATED_DIR = "Assets/Generated/";
    private const string ROOM_DATA_FILE = "room_data.json";

    public static bool ExportJSON(string json, string fileName = ROOM_DATA_FILE)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("JSON content is empty, cannot export");
            return false;
        }

        if (!Directory.Exists(GENERATED_DIR))
        {
            Directory.CreateDirectory(GENERATED_DIR);
            Debug.Log($"Created directory: {GENERATED_DIR}");
        }

        string filePath = Path.Combine(GENERATED_DIR, fileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Successfully exported JSON to: {filePath}");
            AssetDatabase.Refresh();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to export JSON: {ex.Message}");
            return false;
        }
    }

    public static bool ExportSceneData()
    {
        string json = JSONBuilder.BuildJSONFromScene();
        return ExportJSON(json);
    }

    public static string GetGeneratedDirectory()
    {
        return GENERATED_DIR;
    }

    public static string GetRoomDataFilePath()
    {
        return Path.Combine(GENERATED_DIR, ROOM_DATA_FILE);
    }
}