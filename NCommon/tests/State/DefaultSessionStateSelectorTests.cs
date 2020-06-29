using System.ServiceModel;
using System.Threading;
using System.Web;
using NCommon;
using NCommon.StateStorage;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class DefaultSessionStateSelectorTests
    {
        [Test]
        public void Get_returns_HttpSessionState_instance_when_context_is_http()
        {
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.IsWcfApplication).Return(false);
            context.Stub(x => x.IsWebApplication).Return(true);
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateStub<HttpSessionStateBase>());
            context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());

            var state = new DefaultSessionStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<HttpSessionState>());
        }

        [Test]
        public void Get_returns_HttpSessionState_instance_when_context_is_wcf_and_aspnet_compat_is_enabled()
        {
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.IsWcfApplication).Return(false);
            context.Stub(x => x.IsWebApplication).Return(true);
            context.Stub(x => x.IsAspNetCompatEnabled).Return(true);

            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateStub<HttpSessionStateBase>());
            context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());

            var state = new DefaultSessionStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<HttpSessionState>());
        }

        [Test]
        public void Get_returns_WcfSessionState_instance_when_context_is_wcf_and_aspnet_compat_is_disabled()
        {
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.IsWcfApplication).Return(true);
            context.Stub(x => x.IsWebApplication).Return(false);
            context.Stub(x => x.IsAspNetCompatEnabled).Return(false);

            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.InstanceContext).Return(MockRepository.GenerateStub<IInstanceContext>());
            context.OperationContext.InstanceContext.SynchronizationContext = new SynchronizationContext();
            context.OperationContext.InstanceContext.Stub(x => x.Extensions)
                .Return(MockRepository.GenerateStub<IExtensionCollection<InstanceContext>>());

            var state = new DefaultSessionStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<WcfSessionState>());
        }
    }
}