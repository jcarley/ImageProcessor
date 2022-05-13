namespace Domain.Events;

public class ContributionAdded : EventBase
{
    public Guid ContributionId { get; set; }
}