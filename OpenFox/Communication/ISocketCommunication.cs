using System;
using OpenFox.Parsing.Packets;

namespace OpenFox.Communication
{
    internal interface ISocketCommunication
    {
        void Connect(ISocketCallback handler);
        void Write(byte[] buffer, int tag, PacketType type);
    }

    internal interface ISocketCallback
    {
        void Socket_OnConnected(bool obj);
        void Socket_OnMessageSent(bool success, int tag, PacketType packetType);
        void Socket_OnMessageReceived(byte[] bytes);
        void Socket_OnDisconnected(Exception ex);
    }
}
