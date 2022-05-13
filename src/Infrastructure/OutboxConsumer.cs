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

public class EventPublisher : IEventPublisher
{
    private readonly Dictionary<string, IEventStream?> _eventRegistry;

    public EventPublisher(ContributionEventStream contributionEventStream)
    {
        _eventRegistry = new Dictionary<string, IEventStream?>();
        _eventRegistry.Add(contributionEventStream.StreamName, contributionEventStream);
    }

    public async Task<bool> Publish(string streamName, EventBase evt)
    {
        if (_eventRegistry.TryGetValue(streamName, out IEventStream? stream))
        {
            if (stream != null)
            {
                return await stream.Publish(evt);
            }
        }

        return false;
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

        _changeTrackingEventStream.OnChange(async change =>
        {
            var outboxEvent = change.Event;
            bool result = await _publisher.Publish(outboxEvent.OutputStream, outboxEvent);

            if (result)
            {
                await _unitOfWork.EventOutboxRepository.Delete(outboxEvent);
            }
        });
    }
}