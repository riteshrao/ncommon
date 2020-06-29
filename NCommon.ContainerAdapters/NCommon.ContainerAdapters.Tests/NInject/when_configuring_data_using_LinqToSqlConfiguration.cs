using System.Data.Linq;

using NCommon.ContainerAdapter.Ninject;
using NCommon.DataServices.Transactions;
using NCommon.Data.LinqToSql;
using Ninject;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.ContainerAdapters.Tests.Ninject
{
    [TestFixture]
    public class when_configuring_data_using_LinqToSqlConfiguration
    {
        IKernel _kernel;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _kernel = new StandardKernel();
            ConfigureNCommon
                .Using(new NinjectContainerAdapter(_kernel))
                .ConfigureData<LinqToSqlConfiguration>();
            ServiceLocator.SetLocatorProvider(() => MockRepository.GenerateStub<IServiceLocator>());
        }

        [Test]
        public void verify_IUnitOfWorkFactory_is_LinqToSqlUnitOfWorkFactory()
        {
            var factory = _kernel.Get<IUnitOfWorkFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.TypeOf<LinqToSqlUnitOfWorkFactory>());
        }

        [Test]
        public void verify_NHUnitOfWorkFactory_is_registered_as_singleton()
        {
            var factory1 = _kernel.Get<IUnitOfWorkFactory>();
            var factory2 = _kernel.Get<IUnitOfWorkFactory>();
            Assert.That(factory1, Is.Not.Null);
            Assert.That(factory2, Is.Not.Null);
            Assert.That(factory1, Is.SameAs(factory2));
        }

        [Test]
        public void verify_can_get_instances_of_IRepository()
        {
            var repo = _kernel.Get<IRepository<string>>();
            Assert.That(repo, Is.Not.Null);
            Assert.That(repo, Is.TypeOf(typeof (LinqToSqlRepository<string>)));
        }
    }
}