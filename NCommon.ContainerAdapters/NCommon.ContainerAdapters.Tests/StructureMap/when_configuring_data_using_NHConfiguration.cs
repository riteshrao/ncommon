
using NCommon.ContainerAdapter.StructureMap;
using NCommon.DataServices.Transactions;
using NCommon.Data.NHibernate;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace NCommon.ContainerAdapters.Tests.StructureMap
{
    [TestFixture]
    public class when_configuring_data_using_NHConfiguration
    {
        IContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new Container();
            ConfigureNCommon
                .Using(new StructureMapContainerAdapter(_container))
                .ConfigureData<NHConfiguration>();
            ServiceLocator.SetLocatorProvider(() => MockRepository.GenerateStub<IServiceLocator>());
        }

        [Test]
        public void verify_IUnitOfWorkFactory_is_NHUnitOfWorkFactory()
        {
            var factory = _container.GetInstance<IUnitOfWorkFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.TypeOf<NHUnitOfWorkFactory>());
        }

        [Test]
        public void verify_NHUnitOfWorkFactory_is_registered_as_singleton()
        {
            var factory1 = _container.GetInstance<IUnitOfWorkFactory>();
            var factory2 = _container.GetInstance<IUnitOfWorkFactory>();
            Assert.That(factory1, Is.Not.Null);
            Assert.That(factory2, Is.Not.Null);
            Assert.That(factory1, Is.SameAs(factory2));
        }

        [Test]
        public void verify_can_get_instances_of_IRepository()
        {
            var repo = _container.GetInstance<IRepository<string>>();
            Assert.That(repo, Is.Not.Null);
            Assert.That(repo, Is.TypeOf(typeof (NHRepository<string>)));
        }
    }
}