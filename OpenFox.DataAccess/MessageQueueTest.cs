using System.Collections.Generic;
using OpenFox.Logging;

namespace OpenFox.DataAccess
{
    public class MessageQueueTest : IMessageQueue
    {
        private readonly ILogger _logger;

        private Queue<(ushort, string)> _queue = new Queue<(ushort, string)>();
        private Dictionary<ushort, OFMLRequest> _messages = new Dictionary<ushort, OFMLRequest>();

        public MessageQueueTest(ILogger logger, string message)
        {
            _logger = logger;
            if (!string.IsNullOrWhiteSpace(message))
                _queue.Enqueue((1015, message));
        }

        public OFMLRequest GetNextMessage()
        {
            if (_queue.Count == 0)
            {
                _logger.Info("Que is empty");
                return null;
            }

            var next = _queue.Dequeue();
            ushort exchangeId = next.Item1;
            string text = next.Item2;
            OFMLRequest request = new OFMLRequest() { id = exchangeId, text = text };
            return request;
        }

        public void MarkMessageSent(ushort exchangeId)
        {
            _logger.Info($"Marking sent. {exchangeId}");
            _messages.Remove(exchangeId);
        }

        public bool SaveResponse(OFMLResponse response)
        {
            _logger.Verbose(response.message);
            return true;
        }
    }
}
