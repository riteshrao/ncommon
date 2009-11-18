using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using NCommon.Storage;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
	public class NHTestBase
	{
		protected ISessionFactory Factory { get; private set; }

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

			Store.Local.Set("NHRepositoryTests.SessionFactory", Factory);
			NHUnitOfWorkFactory.SetSessionProvider(
				() => Store.Local.Get<ISessionFactory>("NHRepositoryTests.SessionFactory").OpenSession());

			var locator = MockRepository.GenerateStub<IServiceLocator>();
			locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>())
				.Return(new NHUnitOfWorkFactory()).Repeat.Any();

			ServiceLocator.SetLocatorProvider(() => locator);
			HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
		}

		[TestFixtureTearDown]
		public virtual void TearDown()
		{
			NHUnitOfWorkFactory.SetSessionProvider(null);
			Store.Local.Clear();
			HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Stop();
		}
	}
}