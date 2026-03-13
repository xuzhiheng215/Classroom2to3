using UnityEngine;
using System.Collections.Generic;

public static class ProtocolMapper
{
    // Protocol data structures
    public class VRProtocolData
    {
        public string version = "1.0";
        public string timestamp;
        public SceneInfo sceneInfo;
        public List<ObjectInfo> objects = new List<ObjectInfo>();
        public Statistics statistics;
    }

    public class SceneInfo
    {
        public string sceneName;
        public Vector3Data boundsMin;
        public Vector3Data boundsMax;
    }

    public class ObjectInfo
    {
        public string id;
        public string name;
        public string type;
        public Vector3Data position;
        public Vector3Data rotation;
        public Vector3Data scale;
        public string hierarchy;
        public ComponentData components;
    }

    public class ComponentData
    {
        public MeshInfo mesh;
        public MaterialInfo material;
        public bool hasCollider;
        public bool hasRigidbody;
        public bool hasAnimator;
        public bool hasAudioSource;
        public bool hasLight;
        public List<string> allComponents;
    }

    public class MeshInfo
    {
        public int vertexCount;
        public int triangleCount;
        public Vector3Data boundsMin;
        public Vector3Data boundsMax;
    }

    public class MaterialInfo
    {
        public string name;
        public ColorData color;
        public float roughness;
        public float metallic;
    }

    public class Vector3Data
    {
        public float x;
        public float y;
        public float z;

        public Vector3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Data(Vector3 vector3)
        {
            this.x = vector3.x;
            this.y = vector3.y;
            this.z = vector3.z;
        }
    }

    public class ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorData(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public ColorData(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }
    }

    public class Statistics
    {
        public int totalObjectCount;
        public Dictionary<string, int> objectTypeCount = new Dictionary<string, int>();
        public float totalMeshVertices;
        public float totalMeshTriangles;
        public float sceneVolume;
    }

    public static VRProtocolData CreateProtocolData(GameObject[] sceneObjects, string sceneName = "CurrentScene")
    {
        VRProtocolData protocolData = new VRProtocolData();
        protocolData.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        protocolData.sceneInfo = new SceneInfo { sceneName = sceneName };
        protocolData.statistics = new Statistics();

        // Calculate scene bounds and collect object data
        Bounds sceneBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool firstObject = true;

        foreach (GameObject obj in sceneObjects)
        {
            ObjectInfo objectInfo = MapObjectToProtocol(obj);
            protocolData.objects.Add(objectInfo);

            // Update scene bounds
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (firstObject)
                {
                    sceneBounds = renderer.bounds;
                    firstObject = false;
                }
                else
                {
                    sceneBounds.Encapsulate(renderer.bounds);
                }
            }

            // Update statistics
            UpdateStatistics(protocolData.statistics, obj, objectInfo.type);
        }

        // Set scene bounds
        protocolData.sceneInfo.boundsMin = new Vector3Data(sceneBounds.min);
        protocolData.sceneInfo.boundsMax = new Vector3Data(sceneBounds.max);
        protocolData.statistics.sceneVolume = sceneBounds.size.x * sceneBounds.size.y * sceneBounds.size.z;
        protocolData.statistics.totalObjectCount = sceneObjects.Length;

        return protocolData;
    }

    private static ObjectInfo MapObjectToProtocol(GameObject obj)
    {
        ObjectInfo objectInfo = new ObjectInfo();
        
        // Basic object information
        objectInfo.id = obj.GetInstanceID().ToString();
        objectInfo.name = obj.name;
        objectInfo.type = ObjectClassifier.ClassifyObject(obj).ToString();
        objectInfo.hierarchy = SceneScanner.GetObjectHierarchy(obj);

        // Transform data
        ComponentExtractor.TransformData transformData = ComponentExtractor.ExtractTransformData(obj);
        objectInfo.position = new Vector3Data(transformData.position);
        objectInfo.rotation = new Vector3Data(transformData.rotation.eulerAngles);
        objectInfo.scale = new Vector3Data(transformData.scale);

        // Component data
        objectInfo.components = MapComponentsToProtocol(obj);

        return objectInfo;
    }

    private static ComponentData MapComponentsToProtocol(GameObject obj)
    {
        ComponentData componentData = new ComponentData();

        // Mesh data
        ComponentExtractor.MeshData meshData = ComponentExtractor.ExtractMeshData(obj);
        componentData.mesh = new MeshInfo
        {
            vertexCount = meshData.vertexCount,
            triangleCount = meshData.triangleCount,
            boundsMin = new Vector3Data(meshData.boundsMin),
            boundsMax = new Vector3Data(meshData.boundsMax)
        };

        // Material data
        ComponentExtractor.MaterialData materialData = ComponentExtractor.ExtractMaterialData(obj);
        componentData.material = new MaterialInfo
        {
            name = materialData.materialName,
            color = new ColorData(materialData.color),
            roughness = materialData.roughness,
            metallic = materialData.metallic
        };

        // Component flags
        componentData.hasCollider = ComponentExtractor.HasCollider(obj);
        componentData.hasRigidbody = ComponentExtractor.HasRigidbody(obj);
        componentData.hasAnimator = ComponentExtractor.HasAnimator(obj);
        componentData.hasAudioSource = ComponentExtractor.HasAudioSource(obj);
        componentData.hasLight = ComponentExtractor.HasLightComponent(obj);

        // All component names
        componentData.allComponents = ComponentExtractor.GetComponentNames(obj);

        return componentData;
    }

    private static void UpdateStatistics(Statistics stats, GameObject obj, string objectType)
    {
        // Update type count
        if (stats.objectTypeCount.ContainsKey(objectType))
            stats.objectTypeCount[objectType]++;
        else
            stats.objectTypeCount[objectType] = 1;

        // Update mesh statistics
        ComponentExtractor.MeshData meshData = ComponentExtractor.ExtractMeshData(obj);
        stats.totalMeshVertices += meshData.vertexCount;
        stats.totalMeshTriangles += meshData.triangleCount;
    }

    public static string GetProtocolVersion()
    {
        return "1.0";
    }

    public static Dictionary<string, int> GetDefaultTypeMapping()
    {
        return new Dictionary<string, int>
        {
            { ObjectClassifier.ObjectType.Desk.ToString(), 0 },
            { ObjectClassifier.ObjectType.Chair.ToString(), 0 },
            { ObjectClassifier.ObjectType.Blackboard.ToString(), 0 },
            { ObjectClassifier.ObjectType.Bookcase.ToString(), 0 },
            { ObjectClassifier.ObjectType.Clock.ToString(), 0 },
            { ObjectClassifier.ObjectType.Globe.ToString(), 0 },
            { ObjectClassifier.ObjectType.Light.ToString(), 0 },
            { ObjectClassifier.ObjectType.Door.ToString(), 0 },
            { ObjectClassifier.ObjectType.Window.ToString(), 0 },
            { ObjectClassifier.ObjectType.Shelf.ToString(), 0 },
            { ObjectClassifier.ObjectType.TeacherDesk.ToString(), 0 },
            { ObjectClassifier.ObjectType.AirConditioner.ToString(), 0 },
            { ObjectClassifier.ObjectType.Noticeboard.ToString(), 0 },
            { ObjectClassifier.ObjectType.Unknown.ToString(), 0 }
        };
    }
}