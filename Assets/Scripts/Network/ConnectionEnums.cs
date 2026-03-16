using UnityEngine;

namespace Task3.Network
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Error,
        Reconnecting
    }
}