namespace NCommon.State
{
    public interface IApplicationState
    {
        T Get<T>(object key);
        void Put<T>(object key, T instance);
        void Remove<T>(object key);
    }
}