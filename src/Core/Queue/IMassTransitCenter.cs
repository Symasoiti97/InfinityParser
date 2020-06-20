using System.Threading.Tasks;

namespace Queue
{
    public interface IMassTransitCenter
    {
        Task Publish<T>(T message) where T : class;
    }
}