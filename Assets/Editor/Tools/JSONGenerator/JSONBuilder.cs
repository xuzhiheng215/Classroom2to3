using UnityEngine;
using System.Collections.Generic;
using System.Text;

public static class JSONBuilder
{
    public static string BuildJSONFromScene()
    {
        // Get all scene objects
        GameObject[] sceneObjects = SceneScanner.GetAllSceneObjects();
        
        // Create protocol data
        ProtocolMapper.VRProtocolData protocolData = ProtocolMapper.CreateProtocolData(sceneObjects, UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name);
        
        // Convert to JSON
        return BuildJSONFromProtocolData(protocolData);
    }

    public static string BuildJSONFromProtocolData(ProtocolMapper.VRProtocolData protocolData)
    {
        if (protocolData == null)
        {
            Debug.LogError("Protocol data is null, cannot build JSON");
            return "{}";
        }

        // Use StringBuilder for efficient JSON construction
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{");

        // Version
        jsonBuilder.AppendFormat("\"version\":\"{0}\",", protocolData.version);
        
        // Timestamp
        jsonBuilder.AppendFormat("\"timestamp\":\"{0}\",", protocolData.timestamp);

        // Scene Info
        jsonBuilder.Append("\"sceneInfo\":{");
        jsonBuilder.AppendFormat("\"sceneName\":\"{0}\",", protocolData.sceneInfo.sceneName);
        
        // Bounds Min
        jsonBuilder.Append("\"boundsMin\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}},", 
            protocolData.sceneInfo.boundsMin.x, 
            protocolData.sceneInfo.boundsMin.y, 
            protocolData.sceneInfo.boundsMin.z);
        
        // Bounds Max
        jsonBuilder.Append("\"boundsMax\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}}},", 
            protocolData.sceneInfo.boundsMax.x, 
            protocolData.sceneInfo.boundsMax.y, 
            protocolData.sceneInfo.boundsMax.z);

        // Objects Array
        jsonBuilder.Append("\"objects\":[");
        for (int i = 0; i < protocolData.objects.Count; i++)
        {
            ProtocolMapper.ObjectInfo obj = protocolData.objects[i];
            AppendObjectInfo(jsonBuilder, obj);
            if (i < protocolData.objects.Count - 1)
                jsonBuilder.Append(",");
        }
        jsonBuilder.Append("]");

        // Statistics
        jsonBuilder.Append(",\"statistics\":{");
        jsonBuilder.AppendFormat("\"totalObjectCount\":{0},", protocolData.statistics.totalObjectCount);
        
        // Object Type Count
        jsonBuilder.Append("\"objectTypeCount\":{");
        int typeCount = 0;
        foreach (var kvp in protocolData.statistics.objectTypeCount)
        {
            jsonBuilder.AppendFormat("\"{0}\":{1}", kvp.Key, kvp.Value);
            typeCount++;
            if (typeCount < protocolData.statistics.objectTypeCount.Count)
                jsonBuilder.Append(",");
        }
        jsonBuilder.Append("},");
        
        // Mesh Statistics
        jsonBuilder.AppendFormat("\"totalMeshVertices\":{0},", protocolData.statistics.totalMeshVertices);
        jsonBuilder.AppendFormat("\"totalMeshTriangles\":{0},", protocolData.statistics.totalMeshTriangles);
        jsonBuilder.AppendFormat("\"sceneVolume\":{0}}}", protocolData.statistics.sceneVolume);

        jsonBuilder.Append("}");
        return jsonBuilder.ToString();
    }

    private static void AppendObjectInfo(StringBuilder jsonBuilder, ProtocolMapper.ObjectInfo obj)
    {
        jsonBuilder.Append("{");
        
        // Basic info
        jsonBuilder.AppendFormat("\"id\":\"{0}\",", obj.id);
        jsonBuilder.AppendFormat("\"name\":\"{0}\",", obj.name);
        jsonBuilder.AppendFormat("\"type\":\"{0}\",", obj.type);
        
        // Position
        jsonBuilder.Append("\"position\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}},", 
            obj.position.x, obj.position.y, obj.position.z);
        
        // Rotation
        jsonBuilder.Append("\"rotation\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}},", 
            obj.rotation.x, obj.rotation.y, obj.rotation.z);
        
        // Scale
        jsonBuilder.Append("\"scale\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}},", 
            obj.scale.x, obj.scale.y, obj.scale.z);
        
        // Hierarchy
        jsonBuilder.AppendFormat("\"hierarchy\":\"{0}\",", obj.hierarchy);
        
        // Components
        jsonBuilder.Append("\"components\":{");
        
        // Mesh Info
        jsonBuilder.Append("\"mesh\":{");
        jsonBuilder.AppendFormat("\"vertexCount\":{0},\"triangleCount\":{1},", 
            obj.components.mesh.vertexCount, obj.components.mesh.triangleCount);
        
        // Mesh Bounds Min
        jsonBuilder.Append("\"boundsMin\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}},", 
            obj.components.mesh.boundsMin.x, 
            obj.components.mesh.boundsMin.y, 
            obj.components.mesh.boundsMin.z);
        
        // Mesh Bounds Max
        jsonBuilder.Append("\"boundsMax\":{");
        jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}}},", 
            obj.components.mesh.boundsMax.x, 
            obj.components.mesh.boundsMax.y, 
            obj.components.mesh.boundsMax.z);
        
        // Material Info
        jsonBuilder.Append("\"material\":{");
        jsonBuilder.AppendFormat("\"name\":\"{0}\",", obj.components.material.name);
        
        // Material Color
        jsonBuilder.Append("\"color\":{");
        jsonBuilder.AppendFormat("\"r\":{0},\"g\":{1},\"b\":{2},\"a\":{3}},", 
            obj.components.material.color.r, 
            obj.components.material.color.g, 
            obj.components.material.color.b, 
            obj.components.material.color.a);
        
        jsonBuilder.AppendFormat("\"roughness\":{0},\"metallic\":{1}},", 
            obj.components.material.roughness, obj.components.material.metallic);
        
        // Component Flags
        jsonBuilder.AppendFormat("\"hasCollider\":{0},\"hasRigidbody\":{1},\"hasAnimator\":{2},", 
            obj.components.hasCollider.ToString().ToLower(), 
            obj.components.hasRigidbody.ToString().ToLower(), 
            obj.components.hasAnimator.ToString().ToLower());
        
        jsonBuilder.AppendFormat("\"hasAudioSource\":{0},\"hasLight\":{1},", 
            obj.components.hasAudioSource.ToString().ToLower(), 
            obj.components.hasLight.ToString().ToLower());
        
        // All Components List
        jsonBuilder.Append("\"allComponents\":[");
        for (int i = 0; i < obj.components.allComponents.Count; i++)
        {
            jsonBuilder.AppendFormat("\"{0}\"", obj.components.allComponents[i]);
            if (i < obj.components.allComponents.Count - 1)
                jsonBuilder.Append(",");
        }
        jsonBuilder.Append("]}");
        
        jsonBuilder.Append("}");
    }

    public static string BuildSimpleJSON(GameObject[] objects)
    {
        // Build a simplified JSON with just object count and basic info
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{");
        jsonBuilder.AppendFormat("\"objectCount\":{0},", objects.Length);
        jsonBuilder.Append("\"objects\":[");
        
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = objects[i];
            jsonBuilder.Append("{");
            jsonBuilder.AppendFormat("\"name\":\"{0}\",", obj.name);
            jsonBuilder.AppendFormat("\"type\":\"{0}\",", ObjectClassifier.ClassifyObject(obj));
            jsonBuilder.Append("\"position\":{");
            Vector3 pos = obj.transform.position;
            jsonBuilder.AppendFormat("\"x\":{0},\"y\":{1},\"z\":{2}}", pos.x, pos.y, pos.z);
            jsonBuilder.Append("}");
            
            if (i < objects.Length - 1)
                jsonBuilder.Append(",");
        }
        
        jsonBuilder.Append("]}");
        return jsonBuilder.ToString();
    }

    public static string BuildStatisticsJSON()
    {
        GameObject[] sceneObjects = SceneScanner.GetAllSceneObjects();
        Dictionary<string, int> typeCount = new Dictionary<string, int>();
        
        foreach (GameObject obj in sceneObjects)
        {
            string type = ObjectClassifier.ClassifyObject(obj).ToString();
            if (typeCount.ContainsKey(type))
                typeCount[type]++;
            else
                typeCount[type] = 1;
        }
        
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{");
        jsonBuilder.AppendFormat("\"totalObjects\":{0},", sceneObjects.Length);
        jsonBuilder.Append("\"typeCount\":{");
        
        int i = 0;
        foreach (var kvp in typeCount)
        {
            jsonBuilder.AppendFormat("\"{0}\":{1}", kvp.Key, kvp.Value);
            if (i < typeCount.Count - 1)
                jsonBuilder.Append(",");
            i++;
        }
        
        jsonBuilder.Append("}}");
        return jsonBuilder.ToString();
    }
}