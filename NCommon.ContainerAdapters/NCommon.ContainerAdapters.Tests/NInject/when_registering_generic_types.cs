using System;
using NCommon.ContainerAdapter.NInject;
using Ninject;
using NUnit.Framework;

namespace NCommon.ContainerAdapters.Tests.NInject
{
    public interface IGenericInterface<T>{}

    public class GenericImpl<T> : IGenericInterface<T> {}

    [TestFixture]
    public class when_registering_generic_types
    {
        [Test]
        public void does_not_throw_stack_overflow()
        {
            var kernel = new StandardKernel();
            var adapter = new NInjectContainerAdapter(kernel);
            Assert.DoesNotThrow(() => adapter.RegisterGeneric(typeof(IGenericInterface<>), typeof(GenericImpl<>)));
        }

        [Test]
        public void registers_generic_types_correctly()
        {
            var kernel = new StandardKernel();
            var adapter = new NInjectContainerAdapter(kernel);
            adapter.RegisterGeneric(typeof (IGenericInterface<>), typeof (GenericImpl<>));

            var instance = kernel.Get<IGenericInterface<string>>();
            Assert.NotNull(instance);
            Assert.That(instance, Is.TypeOf(typeof(GenericImpl<string>)));
        }
    }
}