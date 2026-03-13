using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

public static class ProtocolValidator
{
    public struct ValidationResult
    {
        public bool isValid;
        public List<string> errors;
        public List<string> warnings;
    }

    public static ValidationResult ValidateJSON(string json)
    {
        ValidationResult result = new ValidationResult();
        result.errors = new List<string>();
        result.warnings = new List<string>();
        result.isValid = true;

        if (string.IsNullOrEmpty(json))
        {
            result.errors.Add("JSON string is empty or null");
            result.isValid = false;
            return result;
        }

        // Check if JSON is properly formatted
        if (!IsValidJSON(json))
        {
            result.errors.Add("JSON format is invalid");
            result.isValid = false;
            return result;
        }

        // Validate required fields
        ValidateRequiredFields(json, ref result);

        return result;
    }

    private static bool IsValidJSON(string json)
    {
        try
        {
            json = json.Trim();
            if (!json.StartsWith("{") || !json.EndsWith("}"))
                return false;

            int braceCount = 0;
            foreach (char c in json)
            {
                if (c == '{') braceCount++;
                if (c == '}') braceCount--;
                if (braceCount < 0) return false;
            }
            return braceCount == 0;
        }
        catch
        {
            return false;
        }
    }

    private static void ValidateRequiredFields(string json, ref ValidationResult result)
    {
        List<string> requiredFields = new List<string>
        {
            "version",
            "timestamp",
            "sceneInfo",
            "objects",
            "statistics"
        };

        foreach (string field in requiredFields)
        {
            if (!json.Contains($"\"{field}\":"))
            {
                result.errors.Add($"Missing required field: {field}");
                result.isValid = false;
            }
        }
    }

    public static string GenerateValidationReport(ValidationResult result)
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== JSON Validation Report ===");
        report.AppendLine($"Validation Result: {(result.isValid ? "PASSED" : "FAILED" )}");
        return report.ToString();
    }
}