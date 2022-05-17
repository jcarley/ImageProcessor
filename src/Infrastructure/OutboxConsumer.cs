using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;

namespace Infrastructure;

public class ContributionEventStream : IEventStream
{
    public string StreamName => "Contributions";

    public Task<bool> Publish(EventBase evt)
    {
        return Task.FromResult(true);
    }
}

public class OutboxConsumer
{
    private readonly IChangeTrackingEventStream _changeTrackingEventStream;
    private readonly IEventPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;

    public OutboxConsumer(IChangeTrackingEventStream changeTrackingEventStream, IEventPublisher publisher,
        IUnitOfWork unitOfWork)
    {
        _changeTrackingEventStream = changeTrackingEventStream;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
    }

    public void Start()
    {
        _changeTrackingEventStream.StartWatch();

        _changeTrackingEventStream.OnChange(async outBoxEvent =>
        {
            EventBase? evt = outBoxEvent.SerializedValue;
        });
    }
}

public interface IChangeTrackingEventStream
{
    void StartWatch();
    void OnChange(Func<OutBoxEvent, Task> action);
}