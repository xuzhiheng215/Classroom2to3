using UnityEngine;
using Newtonsoft.Json.Linq;
using Task3.Network;
using System.Collections.Generic;

public class MessageHandler : MonoSingleton<MessageHandler>
{
    // 用于缓存被控制的物体
    private Dictionary<string, GameObject> sceneObjects = new Dictionary<string, GameObject>();
    
    void Start()
    {
        // 订阅接收消息
        if (WebSocketManager.Instance != null)
        {
            WebSocketManager.Instance.OnMessageReceived += HandleMessage;
            Debug.Log("✅ MessageHandler 已启动，等待控制指令...");
        }
        else
        {
            Debug.LogError("❌ WebSocketManager.Instance 为空！请确保场景中有 WebSocketManager");
        }
    }
    
    /// <summary>
    /// 添加或更新物体引用（可由外部调用，比如场景加载时）
    /// </summary>
    public void RegisterObject(string objectId, GameObject obj)
    {
        if (!sceneObjects.ContainsKey(objectId))
        {
            sceneObjects.Add(objectId, obj);
            Debug.Log($"📝 注册物体: {objectId} -> {obj.name}");
        }
    }
    
    /// <summary>
    /// 批量注册物体
    /// </summary>
    public void RegisterObjects(Dictionary<string, GameObject> objects)
    {
        foreach (var kvp in objects)
        {
            RegisterObject(kvp.Key, kvp.Value);
        }
    }
    
    /// <summary>
    /// 查找物体的方法（带详细日志）
    /// </summary>
    private GameObject FindGameObject(string objectId)
    {
        Debug.Log($"🔍 开始查找物体: '{objectId}'");
        
        // 1. 先从字典里找
        if (sceneObjects.TryGetValue(objectId, out GameObject obj))
        {
            Debug.Log($"✅ [字典] 找到物体: {obj.name}");
            return obj;
        }
        
        Debug.Log($"⚠️ [字典] 未找到 '{objectId}'");
        
        // 2. 尝试用 GameObject.Find 精确查找
        obj = GameObject.Find(objectId);
        if (obj != null)
        {
            Debug.Log($"✅ [GameObject.Find] 找到物体: {obj.name}");
            sceneObjects[objectId] = obj;
            return obj;
        }
        
        Debug.Log($"⚠️ [GameObject.Find] 未找到 '{objectId}'");
        
        // 3. 尝试模糊匹配（处理带括号的情况）
        string[] possibleNames = new string[]
        {
            objectId + "(1)",
            objectId + " (1)",
            objectId + "(2)",
            objectId + " (2)",
            objectId + " 1",
            objectId + "_1"
        };
        
        foreach (string name in possibleNames)
        {
            obj = GameObject.Find(name);
            if (obj != null)
            {
                Debug.Log($"✅ [模糊匹配] 找到物体: {obj.name} (匹配 '{name}')");
                sceneObjects[objectId] = obj;
                return obj;
            }
        }
        
        // 4. 尝试递归查找名称包含目标字符串的物体
        GameObject[] allRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in allRoots)
        {
            obj = FindObjectByNameContains(root.transform, objectId);
            if (obj != null)
            {
                Debug.Log($"✅ [递归查找] 找到物体: {obj.name} (包含 '{objectId}')");
                sceneObjects[objectId] = obj;
                return obj;
            }
        }
        
        Debug.LogError($"❌ 所有方法都未找到物体: '{objectId}'");
        return null;
    }
    
    /// <summary>
    /// 递归查找名称包含指定字符串的物体
    /// </summary>
    private GameObject FindObjectByNameContains(Transform parent, string searchName)
    {
        if (parent.name.Contains(searchName))
        {
            return parent.gameObject;
        }
        
        foreach (Transform child in parent)
        {
            GameObject result = FindObjectByNameContains(child, searchName);
            if (result != null)
            {
                return result;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 接收消息的主处理方法
    /// </summary>
    void HandleMessage(string json)
    {
        Debug.Log($"📨 [HandleMessage] 收到原始消息: {json}");
        
        try
        {
            JObject msg = JObject.Parse(json);
            string type = msg["type"]?.ToString();
            
            Debug.Log($"📌 消息类型: {type}");
            
            switch (type)
            {
                case "command":
                    string command = msg["command"]?.ToString();
                    string objectId = msg["objectId"]?.ToString();
                    
                    Debug.Log($"🎮 执行指令: {command} 作用于 {objectId}");
                    ExecuteCommand(command, objectId, msg);
                    break;
                    
                case "object_visibility_changed":
                    HandleVisibilityChanged(msg);
                    break;
                    
                case "object_position_changed":
                    HandlePositionChanged(msg);
                    break;
                    
                case "object_rotation_changed":
                    HandleRotationChanged(msg);
                    break;
                    
                default:
                    Debug.Log($"❓ 未知指令类型: {type}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 指令处理失败: {e.Message}\n堆栈: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// 处理可见性变化
    /// </summary>
    private void HandleVisibilityChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        bool isVisible = msg["isVisible"]?.Value<bool>() ?? false;
        
        Debug.Log($"👁️ 处理可见性变化: objectId='{objectId}', isVisible={isVisible}");
        
        GameObject obj = FindGameObject(objectId);
        
        if (obj != null)
        {
            Debug.Log($"🔧 准备设置物体 '{obj.name}' 的可见性为: {isVisible}");
            obj.SetActive(isVisible);
            Debug.Log($"✅ 物体 '{objectId}' 可见性已设置为: {isVisible}");
        }
        else
        {
            Debug.LogError($"❌ 找不到物体: '{objectId}'，无法设置可见性");
            
            // 额外调试：打印场景中所有物体的名称
            Debug.Log("📋 场景中所有物体的名称：");
            GameObject[] allRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in allRoots)
            {
                PrintAllChildrenNames(root.transform, "  ");
            }
        }
    }
    
    /// <summary>
    /// 打印所有子物体名称（用于调试）
    /// </summary>
    private void PrintAllChildrenNames(Transform parent, string indent)
    {
        Debug.Log($"{indent}- {parent.name}");
        foreach (Transform child in parent)
        {
            PrintAllChildrenNames(child, indent + "  ");
        }
    }
    
    /// <summary>
    /// 处理位置变化
    /// </summary>
    private void HandlePositionChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        float x = msg["x"]?.Value<float>() ?? 0;
        float y = msg["y"]?.Value<float>() ?? 0;
        float z = msg["z"]?.Value<float>() ?? 0;
        
        Debug.Log($"📍 处理位置变化: objectId='{objectId}', 目标位置=({x}, {y}, {z})");
        
        GameObject obj = FindGameObject(objectId);
        
        if (obj != null)
        {
            obj.transform.position = new Vector3(x, y, z);
            Debug.Log($"✅ 物体 '{objectId}' 移动到: ({x}, {y}, {z})");
        }
        else
        {
            Debug.LogError($"❌ 找不到物体: '{objectId}'，无法设置位置");
        }
    }
    
    /// <summary>
    /// 处理旋转变化
    /// </summary>
    private void HandleRotationChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        float x = msg["x"]?.Value<float>() ?? 0;
        float y = msg["y"]?.Value<float>() ?? 0;
        float z = msg["z"]?.Value<float>() ?? 0;
        float w = msg["w"]?.Value<float>() ?? 1;
        
        Debug.Log($"🔄 处理旋转变化: objectId='{objectId}'");
        
        GameObject obj = FindGameObject(objectId);
        
        if (obj != null)
        {
            obj.transform.rotation = new Quaternion(x, y, z, w);
            Debug.Log($"✅ 物体 '{objectId}' 已旋转");
        }
        else
        {
            Debug.LogError($"❌ 找不到物体: '{objectId}'，无法设置旋转");
        }
    }
    
    /// <summary>
    /// 执行具体指令
    /// </summary>
    private void ExecuteCommand(string command, string objectId, JObject msg)
    {
        Debug.Log($"⚙️ 执行命令: command='{command}', objectId='{objectId}'");
        
        GameObject obj = FindGameObject(objectId);
        
        if (obj == null)
        {
            Debug.LogError($"❌ 未找到物体: {objectId}，无法执行命令");
            return;
        }
        
        switch (command)
        {
            case "move":
                float x = msg["x"]?.Value<float>() ?? 0;
                float y = msg["y"]?.Value<float>() ?? 0;
                float z = msg["z"]?.Value<float>() ?? 0;
                obj.transform.position = new Vector3(x, y, z);
                Debug.Log($"移动物体: {objectId} 到 ({x}, {y}, {z})");
                break;
                
            case "rotate":
                float rx = msg["x"]?.Value<float>() ?? 0;
                float ry = msg["y"]?.Value<float>() ?? 0;
                float rz = msg["z"]?.Value<float>() ?? 0;
                obj.transform.rotation = Quaternion.Euler(rx, ry, rz);
                Debug.Log($"旋转物体: {objectId} 到 ({rx}, {ry}, {rz})");
                break;
                
            case "highlight":
                bool enable = msg["enable"]?.Value<bool>() ?? true;
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = enable ? Color.yellow : Color.white;
                    Debug.Log($"高亮物体: {objectId} = {enable}");
                }
                else
                {
                    Debug.LogWarning($"物体 {objectId} 没有 Renderer 组件，无法高亮");
                }
                break;
                
            default:
                Debug.Log($"未知命令: {command}");
                break;
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (WebSocketManager.Instance != null)
        {
            WebSocketManager.Instance.OnMessageReceived -= HandleMessage;
        }
    }
}