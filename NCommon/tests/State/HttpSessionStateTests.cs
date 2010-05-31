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
        Hashtable _stateData;
        IContext _context;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _stateData = new Hashtable();
            _context = MockRepository.GenerateStub<IContext>();
            _context.Stub(x => x.HttpContext).Return(MockRepository.GenerateStub<HttpContextBase>());
            _context.HttpContext.Stub(x => x.Session).Return(MockRepository.GenerateMock<HttpSessionStateBase>());
            _context.HttpContext.Session.Stub(x => x.SyncRoot).Return(new object());
            _context.HttpContext.Session.Stub(x => x.Clear()).WhenCalled(invocation => _stateData.Clear());
            _context.HttpContext.Session.Stub(x => x.Remove(Arg<string>.Is.Anything)).WhenCalled(invocation =>
            {
                var key = invocation.Arguments[0];
                _stateData.Remove(key);
            });

            _context.HttpContext.Session.Stub(x => x[Arg<string>.Is.Anything])
                .Return("")
                .WhenCalled(invocation =>
                {
                    var key = invocation.Arguments[0];
                    invocation.ReturnValue = _stateData[key];
                });

            _context.HttpContext.Session.Stub(x => x[Arg<string>.Is.Anything] = Arg<object>.Is.Anything)
                .WhenCalled(invocation =>
                {
                    var key = invocation.Arguments[0];
                    var value = invocation.Arguments[1];
                    _stateData[key] = value;
                });
            //_context.HttpContext.Session.Expect(x => x[Utils.BuildFullKey<string>(null)])
            //    .Return("Blah")
            //    .Do(invocation => invocation.ReturnValue = "DEF");
            //_context.HttpContext.Session[Utils.BuildFullKey<string>(null)] = _stateData[Utils.BuildFullKey<string>(null)];
            //_context.HttpContext.Session["test_key".BuildFullKey<string>()] = _stateData["test_key".BuildFullKey<string>()];
        }

        [SetUp]
        public void SetUp()
        {
            _stateData.Clear();
        }


        [Test]
        public void Can_put()
        {
            var data = "test";
            new HttpSessionState(_context).Put("test_key", data);
            Assert.That(_stateData.ContainsKey(typeof(string).FullName + "test_key"));
            Assert.That(_stateData[typeof (string).FullName + "test_key"], Is.EqualTo(data));
        }

        [Test]
        public void Can_put_using_default_key()
        {
            var data = "test";
            new HttpSessionState(_context).Put(data);
            Assert.That(_stateData.ContainsKey(Utils.BuildFullKey<string>(null)));
            Assert.That(_stateData[Utils.BuildFullKey<string>(null)], Is.EqualTo(data));
        }

        [Test]
        public void Can_get()
        {
            var data = "test";
            _stateData[typeof (string).FullName + "test_key"] = data;
            var returned = new HttpSessionState(_context).Get<string>("test_key");
            Assert.That(returned, Is.EqualTo(data));
        }

        [Test]
        public void Can_get_using_default_key()
        {
            var data = "test";
            _stateData[Utils.BuildFullKey<string>(null)] = data;
            var returned = new HttpSessionState(_context).Get<string>();
            Assert.That(returned, Is.EqualTo(data));
        }

        [Test]
        public void Can_remove()
        {
            var data = "test";
            _stateData[typeof(string).FullName + "test_key"] = data;
            new HttpSessionState(_context).Remove<string>("test_key");
            Assert.That(_stateData.ContainsKey(typeof(string).FullName + "test_key"), Is.False);
        }

        [Test]
        public void Can_remove_using_default_key()
        {
            var data = "test";
            _stateData[Utils.BuildFullKey<string>(null)] = data;
            new HttpSessionState(_context).Remove<string>();
            Assert.IsFalse(_stateData.ContainsKey(Utils.BuildFullKey<string>(null)));
        }
    }
}