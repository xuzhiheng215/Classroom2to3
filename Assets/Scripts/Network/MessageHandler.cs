using UnityEngine;
using Newtonsoft.Json.Linq;
using Task3.Network;
using System.Collections.Generic;

public class MessageHandler : MonoSingleton<MessageHandler>
{
    // 保留：用于缓存被控制的物体
    private Dictionary<string, GameObject> sceneObjects = new Dictionary<string, GameObject>();
    
    void Start()
    {
        // 只订阅接收消息
        if (WebSocketManager.Instance != null)
        {
            WebSocketManager.Instance.OnMessageReceived += HandleMessage;
            Debug.Log("MessageHandler 已启动，等待控制指令...");
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
            Debug.Log($"注册物体: {objectId} -> {obj.name}");
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
    
    // 只保留接收消息的处理
    void HandleMessage(string json)
    {
        Debug.Log($"收到控制指令: {json}");
        
        try
        {
            JObject msg = JObject.Parse(json);
            string type = msg["type"]?.ToString();
            
            switch (type)
            {
                // 接收任务二的控制指令
                case "command":
                    string command = msg["command"]?.ToString();
                    string objectId = msg["objectId"]?.ToString();
                    
                    Debug.Log($"执行指令: {command} 作用于 {objectId}");
                    ExecuteCommand(command, objectId, msg);
                    break;
                    
                // 如果还有其他控制类型，保留
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
                    Debug.Log($"未知指令类型: {type}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"指令处理失败: {e.Message}");
        }
    }
    
    // 处理可见性变化
    private void HandleVisibilityChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        bool isVisible = msg["isVisible"]?.Value<bool>() ?? false;
        
        if (sceneObjects.TryGetValue(objectId, out GameObject obj))
        {
            obj.SetActive(isVisible);
            Debug.Log($"物体 {objectId} 可见性: {isVisible}");
        }
    }
    
    // 处理位置变化
    private void HandlePositionChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        float x = msg["x"]?.Value<float>() ?? 0;
        float y = msg["y"]?.Value<float>() ?? 0;
        float z = msg["z"]?.Value<float>() ?? 0;
        
        if (sceneObjects.TryGetValue(objectId, out GameObject obj))
        {
            obj.transform.position = new Vector3(x, y, z);
            Debug.Log($"物体 {objectId} 移动到: ({x}, {y}, {z})");
        }
    }
    
    // 处理旋转变化
    private void HandleRotationChanged(JObject msg)
    {
        string objectId = msg["objectId"]?.ToString();
        float x = msg["x"]?.Value<float>() ?? 0;
        float y = msg["y"]?.Value<float>() ?? 0;
        float z = msg["z"]?.Value<float>() ?? 0;
        float w = msg["w"]?.Value<float>() ?? 1;
        
        if (sceneObjects.TryGetValue(objectId, out GameObject obj))
        {
            obj.transform.rotation = new Quaternion(x, y, z, w);
            Debug.Log($"物体 {objectId} 旋转");
        }
    }
    
    // 执行具体指令
    private void ExecuteCommand(string command, string objectId, JObject msg)
    {
        if (!sceneObjects.TryGetValue(objectId, out GameObject obj))
        {
            Debug.LogWarning($"未找到物体: {objectId}");
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