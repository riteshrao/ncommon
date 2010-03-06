using System.Collections;
using System.Web;
using NCommon.Context;
using NCommon.State.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class HttpSessionStateTests
    {
        [Test]
        public void can_put()
        {
            var stateData = new Hashtable();
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateMock<HttpSessionStateBase>());
            context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());
            context.HttpContext.Session.Expect(x => x[typeof (HttpSessionState).AssemblyQualifiedName])
                .Return(stateData);

            var data = "test";
            new HttpSessionState(context).Put("test_key", data);
            Assert.That(stateData.ContainsKey(typeof(string).FullName + "test_key"));
            Assert.That(stateData[typeof (string).FullName + "test_key"], Is.EqualTo(data));
        }

        [Test]
        public void can_get()
        {
            var stateData = new Hashtable();
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateMock<HttpSessionStateBase>());
            context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());
            context.HttpContext.Session.Expect(x => x[typeof(HttpSessionState).AssemblyQualifiedName])
                .Return(stateData);

            var data = "test";
            stateData[typeof (string).FullName + "test_key"] = data;
            var returned = new HttpSessionState(context).Get<string>("test_key");
            Assert.That(returned, Is.EqualTo(data));
        }

        [Test]
        public void can_remove()
        {
            var stateData = new Hashtable();
            var context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateMock<HttpSessionStateBase>());
            context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());
            context.HttpContext.Session.Expect(x => x[typeof(HttpSessionState).AssemblyQualifiedName])
                .Return(stateData);

            var data = "test";
            stateData[typeof(string).FullName + "test_key"] = data;
            new HttpSessionState(context).Remove<string>("test_key");
            Assert.That(stateData.ContainsKey(typeof(string).FullName + "test_key"), Is.False);
        }
    }
}