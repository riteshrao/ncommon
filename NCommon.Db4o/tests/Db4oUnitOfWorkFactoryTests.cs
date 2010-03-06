using System;
using Db4objects.Db4o;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.Db4o.Tests
{
    [TestFixture]
    public class Db4oUnitOfWorkFactoryTests
    {
        [Test]
        public void Create_Throws_InvalidOperationException_When_No_ContainerProvider_Has_Been_Set()
        {
            var factory = new Db4oUnitOfWorkFactory();
            Assert.Throws<InvalidOperationException>(() => factory.Create());
        }

        [Test]
        public void Create_Returns_LinqToSqlUnitOfWork_Instance_When_DataContextProvider_Has_Been_Set()
        {
            Db4oUnitOfWorkFactory.SetContainerProvider(() => MockRepository.GenerateStub<IObjectContainer>());

            var factory = new Db4oUnitOfWorkFactory();
            var uowInstance = factory.Create();

            Assert.That(uowInstance, Is.Not.Null);
            Assert.That(uowInstance, Is.TypeOf(typeof(Db4oUnitOfWork)));
            Db4oUnitOfWorkFactory.SetContainerProvider(null);
        }
    }
}