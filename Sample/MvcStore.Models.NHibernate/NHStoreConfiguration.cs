using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MvcStore.Common;
using NCommon.Configuration;
using NCommon.Data.NHibernate;
using NCommon.Util;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MvcStoreModels.NHibernate
{
    public class NHStoreConfiguration : IStoreConfiguration
    {
        public IDataConfiguration Create()
        {
            var nhFactory = BuildNHConfiguration().BuildSessionFactory();
            return new NHConfiguration()
                .WithSessionFactory(() => nhFactory)
                .WithDistinctResults();
        }

        public void SchemaCreate()
        {
            var config = BuildNHConfiguration();
            var export = new SchemaExport(config);
            export.Drop(false, true);
            export.Create(false, true);
        }

        public void SchemaUpdate()
        {
            var config = BuildNHConfiguration();
            var updater = new SchemaUpdate(config);
            updater.Execute(false, true);
        }

        Configuration BuildNHConfiguration()
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                              .ConnectionString(x => x.Is(ConnectionString.Get()))
                              .ProxyFactoryFactory<ProxyFactoryFactory>()
                              .AdoNetBatchSize(10))
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<NHStoreConfiguration>())
                .BuildConfiguration();
        }
    }
}