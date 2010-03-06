using System;
using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using NCommon.State;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
	public class NHTestBase
	{
        protected IState State { get; private set; }
		protected ISessionFactory Factory { get; private set; }
        protected IServiceLocator Locator { get; private set; }

		/// <summary>
		/// Sets up the NHibernate SessionFactory and NHUnitOfWorkFactory.
		/// </summary>
		[TestFixtureSetUp]
		public virtual void SetUp()
		{
			Factory = Fluently.Configure()
				.Database(MsSqlConfiguration
				          	.MsSql2008
				          	.ConnectionString(x => x.FromConnectionStringWithKey("testdb")))
				.Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<NHRepositoryTests>())
				.ExposeConfiguration(config =>
				{
					var exportMode = ConfigurationManager.AppSettings["schemaExportMode"];
					switch (exportMode)
					{
						case ("Create"):
							new SchemaExport(config).Create(false, true);
							break;
						case ("Update"):
							new SchemaUpdate(config).Execute(false, true);
							break;
						case ("DropCreate"):
							new SchemaExport(config).Drop(false, true);
							new SchemaExport(config).Create(false, true);
							break;
					}
				})
				.BuildSessionFactory();

            //NHUnitOfWorkFactory.SetSessionProvider(Factory.OpenSession);

			Locator = MockRepository.GenerateStub<IServiceLocator>();
			Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(new NHUnitOfWorkFactory());
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));

			ServiceLocator.SetLocatorProvider(() => Locator);
			HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
		}

        [SetUp]
        public virtual void TestSetup ()
        {
            State = new FakeState();
        }

		[TestFixtureTearDown]
		public virtual void TearDown()
		{
            //NHUnitOfWorkFactory.SetSessionProvider(null);
			HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Stop();
		}
	}
}