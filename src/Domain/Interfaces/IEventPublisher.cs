using Domain.Events;

namespace Domain.Interfaces;

public interface IEventPublisher
{
    public Task<bool> Publish(string streamName, EventBase evt);
}