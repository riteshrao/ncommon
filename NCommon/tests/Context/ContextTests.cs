using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.Web;
using NCommon.Context;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Context
{
    [TestFixture]
    public class ContextTests
    {
        [Test]
        public void IsWebApplication_returns_false_when_HttpContext_is_null()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(null);
            Assert.That(context.IsWebApplication, Is.False);
        }

        [Test]
        public void IsWebApplication_returns_true_when_HttpContext_is_not_null()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            Assert.That(context.IsWebApplication, Is.True);
        }

        [Test]
        public void IsWcfApplication_returns_false_when_OperationContext_is_null()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.OperationContext).Return(null);
            Assert.That(context.IsWcfApplication, Is.False);
        }

        [Test]
        public void IsWcfApplication_returns_true_when_OperationContext_is_not_null()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            Assert.That(context.IsWcfApplication, Is.True);
        }
        
        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_application_is_not_a_WCF_app()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.OperationContext).Return(null);
            Assert.That(context.IsAspNetCompatEnabled, Is.False);
        }

        [Test]
        public void IsAsMetCompatEnabled_returns_false_when_no_wcf_service_does_not_specify_aspnet_compat()
        {
            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(new ServiceDescription());
            Assert.That(context.IsAspNetCompatEnabled, Is.False);
        }

        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_wcf_service_specifies_aspnet_compat_as_not_allowed()
        {
            var serviceDescription = new ServiceDescription();
            serviceDescription.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute
            {
                RequirementsMode = AspNetCompatibilityRequirementsMode.NotAllowed
            });

            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(serviceDescription);
            Assert.That(context.IsAspNetCompatEnabled, Is.False);
        }

        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_wcf_service_specifies_aspnet_compat_as_allowed_and_is_not_web_application()
        {
            var serviceDescription = new ServiceDescription();
            serviceDescription.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute
            {
                RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed
            });

            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(null);
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(serviceDescription);
            Assert.That(context.IsAspNetCompatEnabled, Is.False);
        }

        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_wcf_service_specifies_aspnet_compat_as_allowed_is_web_application()
        {
            var serviceDescription = new ServiceDescription();
            serviceDescription.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute
            {
                RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed
            });

            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(serviceDescription);
            Assert.That(context.IsAspNetCompatEnabled, Is.True);
        }

        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_wcf_service_specifies_aspnet_compat_as_required_and_is_not_web_application()
        {
            var serviceDescription = new ServiceDescription();
            serviceDescription.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute
            {
                RequirementsMode = AspNetCompatibilityRequirementsMode.Required
            });

            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(null);
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(serviceDescription);
            Assert.That(context.IsAspNetCompatEnabled, Is.False);
        }

        [Test]
        public void IsAspNetCompatEnabled_returns_false_when_wcf_service_specifies_aspnet_compat_as_required_is_web_application()
        {
            var serviceDescription = new ServiceDescription();
            serviceDescription.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute
            {
                RequirementsMode = AspNetCompatibilityRequirementsMode.Required
            });

            var context = MockRepository.GenerateStub<NCommon.Context.Impl.Context>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.Stub(x => x.OperationContext).Return(MockRepository.GenerateStub<IOperationContext>());
            context.OperationContext.Stub(x => x.Host).Return(MockRepository.GenerateStub<IServiceHost>());
            context.OperationContext.Host.Stub(x => x.Description).Return(serviceDescription);
            Assert.That(context.IsAspNetCompatEnabled, Is.True);
        }
    }
}