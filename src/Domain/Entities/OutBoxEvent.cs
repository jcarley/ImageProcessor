using Domain.Events;
using Domain.Interfaces;

namespace Domain.Entities;

public class OutBoxEvent : IIdentity
{
    public string? OutputStream { get; set; }

    public int Retries { get; set; }

    public DateTime CreatedAt { get; set; }

    public EventBase? SerializedValue { get; set; }

    public Guid Id { get; set; }
}