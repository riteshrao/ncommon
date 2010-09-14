using NHibernate;

namespace MvcStore.Services
{
    public interface IStoreInitializer
    {
        void Initialize(ISessionFactory sessionFactory);
    }
}