using System.Data.Linq;
using Castle.Windsor;
using CommonServiceLocator;
using NCommon.ContainerAdapter.CastleWindsor;
using NCommon.DataServices.Transactions;
using NCommon.LinqToSql;
using NCommon.ObjectAccess;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.ContainerAdapters.Tests.CastleWindsor
{
    [TestFixture]
    public class when_configuring_data_using_LinqToSqlConfiguration
    {
        IWindsorContainer _container;

        [TestFixtureSetUp()]
        public void FixtureSetup()
        {
            _container = new WindsorContainer();
            NCommon.ConfigureNCommon
                .Using(new WindsorContainerAdapter(_container))
                .ConfigureData<LinqToSqlConfiguration>();
            ServiceLocator.SetLocatorProvider(() => MockRepository.GenerateStub<IServiceLocator>());
        }

        [Test]
        public void verify_IUnitOfWorkFactory_is_LinqToSqlUnitOfWorkFactory()
        {
            var factory = _container.Resolve<IUnitOfWorkFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.TypeOf<LinqToSqlUnitOfWorkFactory>());
        }

        [Test]
        public void verify_NHUnitOfWorkFactory_is_registered_as_singleton()
        {
            var factory1 = _container.Resolve<IUnitOfWorkFactory>();
            var factory2 = _container.Resolve<IUnitOfWorkFactory>();
            Assert.That(factory1, Is.Not.Null);
            Assert.That(factory2, Is.Not.Null);
            Assert.That(factory1, Is.SameAs(factory2));
        }

        [Test]
        public void verify_can_get_instances_of_IRepository()
        {
            var repo = _container.Resolve<IRepository<string>>();
            Assert.That(repo, Is.Not.Null);
            Assert.That(repo, Is.TypeOf(typeof (LinqToSqlRepository<string>)));
        }
    }
}