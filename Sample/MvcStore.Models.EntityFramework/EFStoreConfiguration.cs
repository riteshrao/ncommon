using System;
using MvcStore.Common;
using NCommon.Configuration;
using NCommon.Data.EntityFramework;
using NCommon.Util;

namespace MvcStoreModels.EntityFramework
{
    public class EFStoreConfiguration : IStoreConfiguration
    {
        public IDataConfiguration Create()
        {
            var context = new MvcStoreDataContext(ConnectionString.Get());
            return new EFConfiguration()
                .WithObjectContext(() => context);
        }
    }
}