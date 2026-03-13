using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TestComponents
{
    static TestComponents()
    {
        // This static constructor runs when Unity loads
        Debug.Log("Testing Scene Scanner and Data Extractor Components...");
        
        // Test basic functionality
        TestBasicFunctionality();
    }

    [MenuItem("Tools/Test Data Extractor Components")]
    public static void TestComponentsMenuItem()
    {
        TestBasicFunctionality();
        TestJSONGeneration();
        TestFileExport();
    }

    private static void TestBasicFunctionality()
    {
        Debug.Log("=== Testing Basic Functionality ===");
        
        // Test SceneScanner
        int objectCount = SceneScanner.GetSceneObjectCount();
        Debug.Log($"Scene object count: {objectCount}");
        
        GameObject[] allObjects = SceneScanner.GetAllSceneObjects();
        Debug.Log($"GetAllSceneObjects returned {allObjects.Length} objects");
        
        // Test ObjectClassifier
        if (allObjects.Length > 0)
        {
            GameObject testObj = allObjects[0];
            ObjectClassifier.ObjectType type = ObjectClassifier.ClassifyObject(testObj);
            Debug.Log($"Object {testObj.name} classified as: {type}");
        }
        
        Debug.Log("=== Basic Functionality Test Complete ===");
    }

    private static void TestJSONGeneration()
    {
        Debug.Log("=== Testing JSON Generation ===");
        
        // Test JSONBuilder
        string statisticsJson = JSONBuilder.BuildStatisticsJSON();
        Debug.Log($"Generated statistics JSON: {statisticsJson.Length} characters");
        
        // Test ProtocolValidator
        ProtocolValidator.ValidationResult validation = ProtocolValidator.ValidateJSON(statisticsJson);
        Debug.Log($"Statistics JSON validation: {(validation.isValid ? "PASSED" : "FAILED" )}");
        
        if (!validation.isValid)
        {
            Debug.LogError(ProtocolValidator.GenerateValidationReport(validation));
        }
        
        Debug.Log("=== JSON Generation Test Complete ===");
    }

    private static void TestFileExport()
    {
        Debug.Log("=== Testing File Export ===");
        
        // Test JSONFormatter
        string compactJson = JSONBuilder.BuildStatisticsJSON();
        string formattedJson = JSONFormatter.FormatJSON(compactJson, 2);
        Debug.Log($"Compact JSON: {compactJson.Length} chars, Formatted JSON: {formattedJson.Length} chars");
        
        // Test FileExporter
        bool exportSuccess = FileExporter.ExportJSON(formattedJson, "test_statistics.json");
        Debug.Log($"File export: {(exportSuccess ? "SUCCESS" : "FAILED" )}");
        
        Debug.Log("=== File Export Test Complete ===");
    }
}