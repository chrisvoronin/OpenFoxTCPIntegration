using System;
using System.Collections.Generic;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.Utility
{
    public class PacketRegistry
    {
        private Dictionary<PacketType, Type> _types = new Dictionary<PacketType, Type>();

        public Dictionary<PacketType, Type> Types 
        { 
            get 
            {
                if (_types.Count == 0)
                {
                    LoadTypes();
                }
                return _types; 
            } 
        }

        private void LoadTypes()
        {
            _types.Add(PacketType.HeartBeat, typeof(HeartbeatPacket));
            _types.Add(PacketType.PositiveAck, typeof(PositiveAckPacket));
            _types.Add(PacketType.NegativeAck, typeof(NegativeAckPacket));
            _types.Add(PacketType.DataMessage, typeof(DataMessagePacket));
            _types.Add(PacketType.Connection, typeof(ConnectionPacket));
        }
    }
}
