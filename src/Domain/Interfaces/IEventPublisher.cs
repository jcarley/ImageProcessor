using Domain.Events;

namespace Domain.Interfaces;

public interface IEventPublisher
{
    AsyncLocal<ITransaction> Transaction { get; set; }

    public Task Publish(string streamName, EventBase evt);
}