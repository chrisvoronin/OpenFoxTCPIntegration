using System;
using System.Collections.Generic;
using OpenFox.Parsing.PacketHandlers;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.Utility
{
    public class PacketHandlerRegistry
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
            _types.Add(PacketType.HeartBeat, typeof(HeartBeatHandler));
            _types.Add(PacketType.PositiveAck, typeof(PositiveAckHandler));
            _types.Add(PacketType.NegativeAck, typeof(NegativeAckHandler));
            _types.Add(PacketType.DataMessage, typeof(DataMessageHandler));
            _types.Add(PacketType.Connection, typeof(ConnectionHandler));
        }
    }
}
