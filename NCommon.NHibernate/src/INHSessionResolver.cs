using System;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    public interface INHSessionResolver
    {
        Guid GetSessionKeyFor<T>();
        ISession OpenSessionFor<T>();
        void RegisterSessionFactoryProvider(Func<ISessionFactory> factoryProvider);
        ISessionFactory GetFactoryFor<T>();
        int SessionFactoriesRegistered { get; }
    }
}