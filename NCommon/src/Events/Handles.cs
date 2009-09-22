namespace NCommon.Events
{
    /// <summary>
    /// Interface used by handlers of domain events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Handles<T> where T : IDomainEvent
    {
        void Handle(T @event);
    }
}