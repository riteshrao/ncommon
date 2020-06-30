using System.Collections.Generic;
using System.ServiceModel;
using System.Web;
using NCommon;
using NCommon.StateStorage;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class DefaultLocalStateSelectorTests
    {
        [Test]
        public void Get_returns_ThreadLocalState_instance_when_context_is_not_http_or_wcf()
        {
            var context = MockRepository.GenerateStub<IContext>();
            var state = new DefaultLocalStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<ThreadLocalState>());
        }

        [Test]
        public void Get_returns_HttpLocalState_instance_when_context_is_http()
        {
            var context = MockRepository.GenerateMock<IContext>();
            context.Expect(x => x.IsWebApplication).Return(true);
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Items).Return(new Dictionary<string, object>());

            var state = new DefaultLocalStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<HttpLocalState>());
            context.VerifyAllExpectations();
        }

        [Test]
        public void Get_returns_WcfLocalState_instance_when_context_is_wcf()
        {
            var context = MockRepository.GenerateMock<IContext>();
            context.Stub(x => x.IsWebApplication).Return(false);
            context.Stub(x => x.IsWcfApplication).Return(true);
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Extensions)
                .Return(MockRepository.GenerateStub<IExtensionCollection<OperationContext>>());
            context.OperationContext.Extensions.Stub(x => x.Find<WcfLocalState.WcfLocalStateExtension>()).Return(null);
            var state = new DefaultLocalStateSelector(context).Get();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<WcfLocalState>());
        }
    }
}