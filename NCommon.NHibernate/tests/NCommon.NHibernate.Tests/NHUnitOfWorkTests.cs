using System;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
    [TestFixture]
    public class NHUnitOfWorkTests
    {
        [Test]
        public void ctor_throws_ArgumentNullException_when_sessionResolver_parameter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new NHUnitOfWork(null));
        }

        [Test]
        public void GetSessionFor_returns_session_for_type()
        {
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(resolver);
            var session = unitOfWork.GetSession<string>();
            Assert.That(session, Is.Not.Null);
        }

        [Test]
        public void GetSessionFor_returns_same_session_for_types_handled_by_same_factory()
        {
            var sessionKey = Guid.NewGuid();
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(sessionKey);
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(sessionKey);
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(resolver);
            var stringSession = unitOfWork.GetSession<string>();
            var intSession = unitOfWork.GetSession<int>();

            Assert.That(stringSession, Is.SameAs(intSession));
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<string>());
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<int>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<string>(), c => c.Repeat.Once());
        }

        [Test]
        public void GetSessionFor_returns_different_session_for_types_handled_by_different_factory()
        {
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(resolver);
            var stringSession = unitOfWork.GetSession<string>();
            var intSession = unitOfWork.GetSession<int>();

            Assert.That(stringSession, Is.Not.SameAs(intSession));
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<string>());
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<int>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<string>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<int>());
        }

        [Test]
        public void Flush_calls_flush_on_all_open_ISession_instances()
        {
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(resolver);
            unitOfWork.GetSession<string>();
            unitOfWork.GetSession<int>();

            unitOfWork.Flush();
            resolver.OpenSessionFor<string>().AssertWasCalled(x => x.Flush());
            resolver.OpenSessionFor<int>().AssertWasCalled(x => x.Flush());
        }

        [Test]
        public void Dispose_disposes_all_open_ISession_instances()
        {
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(resolver);
            unitOfWork.GetSession<string>();
            unitOfWork.GetSession<int>();

            unitOfWork.Dispose();
            resolver.OpenSessionFor<string>().AssertWasCalled(x => x.Dispose());
            resolver.OpenSessionFor<int>().AssertWasCalled(x => x.Dispose());
        }
    }
}