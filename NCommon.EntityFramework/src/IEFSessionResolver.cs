using System;
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    public interface IEFSessionResolver
    {
        Guid GetSessionKeyFor<T>();
        IEFSession OpenSessionFor<T>();
        void RegisterObjectContextProvider(Func<ObjectContext> contextProvider);
        ObjectContext GetObjectContextFor<T>();
        int ObjectContextsRegistered { get; }
    }
}