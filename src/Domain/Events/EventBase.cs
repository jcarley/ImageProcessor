namespace Domain.Events;

public abstract class EventBase
{
    public Guid Id { get; set; }

    public Guid CorrelationId { get; set; }

    public Guid MessageId { get; set; }
}