namespace NCommon.State
{
    public interface ISessionState
    {
        T Get<T>(object key);
        void Put<T>(object key, T instance);
        void Remove<T>(object key);
    }
}