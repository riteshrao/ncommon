using System.Data.Objects;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NCommon.ContainerAdapter.Unity;
using NCommon.Data;
using NCommon.Data.EntityFramework;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.ContainerAdapters.Tests.Unity
{
    [TestFixture]
    public class when_configuring_data_using_EFConfiguration
    {
        IUnityContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new UnityContainer();
            Configure
                .Using(new UnityContainerAdapter(_container))
                .ConfigureData<EFConfiguration>();
            ServiceLocator.SetLocatorProvider(() => MockRepository.GenerateStub<IServiceLocator>());
        }

        [Test]
        public void verify_EFUnitOfWorkFactory_is_registered_as_singleton()
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
            Assert.That(repo, Is.TypeOf(typeof (EFRepository<string>)));
        }

        [Test]
        public void verify_instances_of_IUnitOfWorkFactory_is_EFUnitOfWorkFactory()
        {
            var factory = _container.Resolve<IUnitOfWorkFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.TypeOf<EFUnitOfWorkFactory>());
        }
    }
}