using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using Task3.Network;
using Newtonsoft.Json;

public class WebSocketManager : MonoSingleton<WebSocketManager>
{
    [Header("连接设置")]
    public string serverUrl = "ws://120.53.31.252:8080/ws/test-scene-123";
    public string applicationId = "unity-client-001";
    
    private WebSocket webSocket;
    private ConnectionStatus _status = ConnectionStatus.Disconnected;
    private Queue<string> messageQueue = new Queue<string>();

    void Start()
    {
        Debug.Log("WebSocketManager 启动 - Start方法被调用");
        Connect();
    }
    
    public ConnectionStatus Status
    {
        get { return _status; }
        private set
        {
            _status = value;
            Debug.Log($"状态: {_status}");
            OnStatusChanged?.Invoke(_status);
        }
    }
    
    public bool IsConnected
    {
        get { return _status == ConnectionStatus.Connected; }
    }
    
    public event Action<ConnectionStatus> OnStatusChanged;
    public event Action<string> OnMessageReceived;
    public event Action<string> OnError;
    
    void Update()
    {
        // 处理WebSocket消息（Unity主线程）
        while (messageQueue.Count > 0)
        {
            string msg = messageQueue.Dequeue();
            OnMessageReceived?.Invoke(msg);
        }
        
        // 驱动WebSocket
        #if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket != null)
        {
            webSocket.DispatchMessageQueue();
        }
        #endif
    }
    
    public async void Connect()
    {
        Debug.Log($"【调试】Connect 方法被调用");
        Debug.Log($"【调试】当前状态: {Status}");

        if (Status == ConnectionStatus.Connected || Status == ConnectionStatus.Connecting)
        {
            Debug.LogWarning("已经在连接中或已连接");
            return;
        }
        
        Debug.Log($"正在连接: {serverUrl}");
        Status = ConnectionStatus.Connecting;
        
        try
        {
            webSocket = new WebSocket(serverUrl);
            
            // 绑定事件
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnWebSocketMessage;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnClose += OnWebSocketClose;
            
            // 连接
            await webSocket.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError($"连接失败: {e.Message}");
            Status = ConnectionStatus.Error;
            OnError?.Invoke(e.Message);
        }
    }
    
    private void OnWebSocketOpen()
    {
        Debug.Log("✅ WebSocket连接打开");
        Status = ConnectionStatus.Connected;
        
        // 连接成功后立即发送注册消息
        SendRegisterMessage();
    }
    
    private void OnWebSocketMessage(byte[] data)
    {
        // 处理收到的消息
        string message = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"📨 收到原始消息: {message}");
        
        // 放入主线程队列
        lock (messageQueue)
        {
            messageQueue.Enqueue(message);
        }
    }
    
    private void OnWebSocketError(string errorMsg)
    {
        Debug.LogError($"❌ WebSocket错误: {errorMsg}");
        OnError?.Invoke(errorMsg);
    }
    
    private void OnWebSocketClose(WebSocketCloseCode closeCode)
    {
        Debug.Log($"🔌 WebSocket关闭: {closeCode}");
        Status = ConnectionStatus.Disconnected;
    }
    
    private void SendRegisterMessage()
    {
        var registerMsg = new
        {
            type = "register",
            applicationId = applicationId,
            timestamp = DateTime.UtcNow.ToString("o")
        };
        
        string json = JsonConvert.SerializeObject(registerMsg);
        Debug.Log($"📤 发送注册消息: {json}");
        
        SendWebSocketMessage(json);
    }
    
    public async void SendWebSocketMessage(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            Debug.Log($"📤 发送: {message}");
            await webSocket.SendText(message);
        }
        else
        {
            Debug.LogWarning("WebSocket未连接，消息未发送");
        }
    }
    
    public void SendJson(object message)
    {
        string json = JsonConvert.SerializeObject(message);
        SendWebSocketMessage(json);
    }
    
    public void Disconnect()  
    {
        if (webSocket != null)
        {
            // 不等待，直接关闭
            webSocket.Close();
            webSocket = null;
        }
        Status = ConnectionStatus.Disconnected;
        Debug.Log("已断开连接");
    }
    
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();  // 先调用父类的OnApplicationQuit
        
        if (webSocket != null)
        {
            webSocket.Close();
            webSocket = null;
        }
    }
}