using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OpenFox.Communication;
using OpenFox.Logging;
using OpenFox.DataAccess;
using OpenFox.Parsing.PacketHandlers;
using OpenFox.Parsing.Packets;
using OpenFox.Parsing.Utility;

namespace OpenFox
{

    public enum ServiceState
    {
        Initializing = 0,
        Connecting,
        SendingConnectionMessage,
        SendingMessage,
        Idle,
        Disconnected
    }

    public sealed class OpenFoxClientService : ISocketCallback, IPacketCallback
    {
        public OpenFoxClientService(string ip
            , int port
            , IMessageQueue queue
            , ILogger logger
            , int reconnectSeconds
            , bool isBigEndian)
        {
            _messageQueue = queue;
            _logger = logger;
            _minReconnectSeconds = reconnectSeconds;
            _isBigEndian = isBigEndian;

            _timer.Elapsed += Timer_Elapsed;

            _writer = new PacketWriter(true, ushort.MaxValue, _isBigEndian);
            _reader = new PacketReader(_isBigEndian);

            byte[] startIndicator = MessageConstants.MessageStartIndicatorBytes(_isBigEndian);
            byte[] endIndicator = MessageConstants.MessageEndIndicatorBytes(_isBigEndian);

            _communication = new SocketCommunication(ip, port, startIndicator, endIndicator);
        }

        // for parsing packets
        private PacketRegistry _packetRegistry = new PacketRegistry();
        private PacketHandlerRegistry _handlerRegistry = new PacketHandlerRegistry();
        private readonly PacketWriter _writer;
        private readonly PacketReader _reader;
        private DataMessageManager _dmManager = new DataMessageManager(); //combines frames

        // passed in parameter in start
        private ILogger _logger; // logging utility
        private IMessageQueue _messageQueue; // reads messages to send, and writes responses
        private ISocketCommunication _communication; // socket communication

        // state variables
        private ServiceState _state = ServiceState.Initializing; // state of the service
        private DateTime _lastMessageTime; //keep track of last message, so we know when to heartbeat
        private byte[] _codingTechnique; // used for decoding images.
        private int _defaultTimeOut = 30; // longest time waiting for response
        private int _maxIdleTime = 30; // longest time without any messages
        private int _minSendDelaySeconds = 0; // use to configure minimum delay between sends, only used for testing.
        private int _minReconnectSeconds = 5; // minimum time to reconnect in seconds
        private bool _isBigEndian = true; // controls how it reads and writes endians
        private System.Timers.Timer _timer = new System.Timers.Timer(2000); //heartbeat check timer

        public void Start()
        {
            if (_state != ServiceState.Initializing)
            {
                return;
            }

            StartConnection();
        }

        #region Socket Events

        public void Socket_OnConnected(bool obj)
        {
            _logger.Info("Socket_OnConnected " + obj);

            if (obj == false)
            {
                _state = ServiceState.Disconnected;
                Thread.Sleep(_minReconnectSeconds * 1000);
                StartConnection();
                return;
            }

            _state = ServiceState.SendingConnectionMessage;
            SendConnectionMessage();
        }

        public void Socket_OnMessageSent(bool success, int tag, PacketType packetType)
        {
            _logger.Info($"Socket_OnMessageSent {(char)packetType} - {tag}");
            _lastMessageTime = DateTime.Now;
            _state = ServiceState.Idle;
        }

        public void Socket_OnMessageReceived(byte[] bytes)
        {
            _reader.Clear();
            _reader.SetSource(bytes);
            PacketType pt = (PacketType)_reader.PeekByte(MessageConstants.MessageTypePosition);
            IPacket packet = ResolvePacket(pt, _reader);

            _logger.Info($"Socket_OnMessageReceived {(char)packet.Type} - {packet.ExchangeId}");

            IPacketHandler handler = ResolveHandler(pt);
            handler.Handle(packet, this);
        }

        public void Socket_OnDisconnected(Exception ex)
        {
            _logger.Info($"Socket_OnDisconnected {ex.Message}");
            _timer.Stop();

            _state = ServiceState.Disconnected;

            // reconnect now or after delay
            if (_minReconnectSeconds <= 0)
            {
                StartConnection();
            }
            else
            {
                Task.Delay(TimeSpan.FromSeconds(_minReconnectSeconds)).ContinueWith(_ =>
                {
                    StartConnection();
                });
            }
        }

        #endregion

        #region IPacketCallback

        public void Process(DataMessagePacket packet)
        {
            if (_dmManager.Process(packet, out OFMLResponse ofml))
            {
                _messageQueue.SaveResponse(ofml);
            }
        }

        public void SendAck(ushort exchnageId)
        {
            PositiveAckPacket p = new PositiveAckPacket();
            p.ExchangeId = exchnageId;
            SendPacket(p);
        }

        public void UpdateAck(ushort exchangeId)
        {
            _messageQueue.MarkMessageSent(exchangeId);
        }

        public void SetConnectionResponseParams(ConnectionPacket pt)
        {
            _defaultTimeOut = pt.defaultTimeOut;
            _maxIdleTime = pt.maxIdleTime;
            _codingTechnique = pt.objectCoding;

            _state = ServiceState.Idle;

            StartProcessing();
        }

        public void SaveResponse(OFMLResponse message)
        {
            _messageQueue.SaveResponse(message);
        }

        #endregion

        #region Private Methods

        private void StartConnection()
        {
            _logger.Info("StartConnection");
            if (_state != ServiceState.Initializing && _state != ServiceState.Disconnected)
            {
                return;
            }
            _state = ServiceState.Connecting;
            _communication.Connect(this);
        }

        private void StartProcessing()
        {
            _logger.Info("StartProcessing");
            _state = ServiceState.Idle;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            // only when idle
            if (_state != ServiceState.Idle) return;

            TimeSpan elapsedTime = DateTime.Now - _lastMessageTime;
            double threshhold = _defaultTimeOut * 1;

            bool shouldHeartbeat = elapsedTime.TotalSeconds >= threshhold;

            if (shouldHeartbeat)
            {
                SendHeartbeatMessage();
                return;
            }

            bool shouldSend = elapsedTime.TotalSeconds >= _minSendDelaySeconds;

            if (shouldSend)
            {
                OFMLRequest model = _messageQueue.GetNextMessage();
                if (model != null)
                {
                    SendOFML(model);
                }
            }
        }

        private void SendOFML(OFMLRequest message)
        {
            ushort messageId = message.id;
            DataMessagePacket p = new DataMessagePacket();
            p.OFML = message.text;
            p.ExchangeId = messageId;
            SendPacket(p);
        }

        private void SendConnectionMessage()
        {
            ConnectionPacket p = new ConnectionPacket();
            p.ExchangeId = 1;
            SendPacket(p);
        }

        private void SendHeartbeatMessage()
        {
            HeartbeatPacket p = new HeartbeatPacket();
            p.ExchangeId = 2;
            SendPacket(p);
        }

        private void SendPacket(IPacket packet)
        {
            _state = ServiceState.SendingMessage;
            _writer.Reset();
            packet.Serialize(_writer);
            byte[] bytes = _writer.CopyData();
            _communication.Write(bytes, packet.ExchangeId, packet.Type);
        }

        private IPacket ResolvePacket(PacketType pt, PacketReader reader)
        {
            var type = _packetRegistry.Types[pt];
            var packet = (IPacket)Activator.CreateInstance(type);
            packet.Deserialize(reader);
            return packet;
        }

        private IPacketHandler ResolveHandler(PacketType pt)
        {
            var type = _handlerRegistry.Types[pt];
            return (IPacketHandler)Activator.CreateInstance(type);
        }

        #endregion

    }
}
