using System.Threading.Tasks;

namespace OpenFox.DataAccess
{
    public interface IMessageQueue
    {
        OFMLRequest GetNextMessage();

        void MarkMessageSent(ushort exchangeId);

        bool SaveResponse(OFMLResponse message);
    }

    // in case we ever need to do this async
    public interface IAsyncMessageQueue : IMessageQueue
    {
        Task<OFMLRequest> GetNextMessageAsync();

        Task MarkMessageSentAsync(ushort exchangeId);

        Task<bool> SaveResponseAsync(OFMLResponse message);
    }
}