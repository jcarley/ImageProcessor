using Domain.Events;

namespace Domain.Interfaces;

public interface IEventStream
{
    public string StreamName { get; }

    public Task<bool> Publish(EventBase evt);
}