using UnityEngine;

public static class ComponentExtractor
{
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public struct MeshData
    {
        public int vertexCount;
        public int triangleCount;
        public Vector3 boundsMin;
        public Vector3 boundsMax;
    }

    public struct MaterialData
    {
        public string materialName;
        public Color color;
        public float roughness;
        public float metallic;
    }

    public static TransformData ExtractTransformData(GameObject obj)
    {
        TransformData data = new TransformData();
        if (obj != null)
        {
            Transform transform = obj.transform;
            data.position = transform.position;
            data.rotation = transform.rotation;
            data.scale = transform.localScale;
        }
        return data;
    }

    public static MeshData ExtractMeshData(GameObject obj)
    {
        MeshData data = new MeshData();
        
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            data.vertexCount = mesh.vertexCount;
            data.triangleCount = mesh.triangles.Length / 3;
            data.boundsMin = mesh.bounds.min;
            data.boundsMax = mesh.bounds.max;
        }
        
        return data;
    }

    public static MaterialData ExtractMaterialData(GameObject obj)
    {
        MaterialData data = new MaterialData();
        data.color = Color.white;
        data.roughness = 0.5f;
        data.metallic = 0f;
        
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.sharedMaterials.Length > 0)
        {
            Material material = renderer.sharedMaterials[0];
            data.materialName = material.name;
            
            if (material.HasProperty("_Color"))
                data.color = material.color;
            
            if (material.HasProperty("_Roughness"))
                data.roughness = material.GetFloat("_Roughness");
            
            if (material.HasProperty("_Metallic"))
                data.metallic = material.GetFloat("_Metallic");
        }
        
        return data;
    }

    public static bool HasCollider(GameObject obj)
    {
        return obj != null && obj.GetComponent<Collider>() != null;
    }

    public static bool HasRigidbody(GameObject obj)
    {
        return obj != null && obj.GetComponent<Rigidbody>() != null;
    }

    public static bool HasAnimator(GameObject obj)
    {
        return obj != null && obj.GetComponent<Animator>() != null;
    }

    public static bool HasAudioSource(GameObject obj)
    {
        return obj != null && obj.GetComponent<AudioSource>() != null;
    }

    public static bool HasLightComponent(GameObject obj)
    {
        return obj != null && obj.GetComponent<Light>() != null;
    }

    public static System.Collections.Generic.List<string> GetComponentNames(GameObject obj)
    {
        System.Collections.Generic.List<string> componentNames = new System.Collections.Generic.List<string>();
        
        if (obj != null)
        {
            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp != null)
                    componentNames.Add(comp.GetType().Name);
            }
        }
        
        return componentNames;
    }

    public static System.Collections.Generic.Dictionary<string, object> ExtractAllComponentData(GameObject obj)
    {
        System.Collections.Generic.Dictionary<string, object> data = new System.Collections.Generic.Dictionary<string, object>();
        
        if (obj == null) return data;
        
        TransformData transformData = ExtractTransformData(obj);
        data.Add("transform", transformData);
        
        MeshData meshData = ExtractMeshData(obj);
        data.Add("mesh", meshData);
        
        MaterialData materialData = ExtractMaterialData(obj);
        data.Add("material", materialData);
        
        data.Add("hasCollider", HasCollider(obj));
        data.Add("hasRigidbody", HasRigidbody(obj));
        data.Add("hasAnimator", HasAnimator(obj));
        data.Add("hasAudioSource", HasAudioSource(obj));
        data.Add("hasLight", HasLightComponent(obj));
        
        System.Collections.Generic.List<string> componentNames = GetComponentNames(obj);
        data.Add("components", componentNames);
        
        return data;
    }
}