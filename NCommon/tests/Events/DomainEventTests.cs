using System;
using System.Collections.Generic;
using CommonServiceLocator;
using NCommon.Events;
using NCommon.StateStorage;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Events
{
    /// <summary>
    /// Tests the <see cref="DomainEvent"/> class.
    /// </summary>
    [TestFixture]
    public class DomainEventTests
    {
        #region mock event classes
        public class TestEvent1 : IDomainEvent{}
        public class TestEvent2 : IDomainEvent{}
        #endregion

        [Test]
        public void registered_callbacks_are_called_when_event_is_raised()
        {
            var state = new FakeState();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            locator.Stub(x => x.GetInstance<IState>()).Return(state);
            ServiceLocator.SetLocatorProvider(() => locator);

            var mockTestEvent1Handler = MockRepository.GenerateMock<Handles<TestEvent1>>();
            var mockTestEvent2Handler = MockRepository.GenerateMock<Handles<TestEvent2>>();

            DomainEvent.RegisterCallback<TestEvent1>(mockTestEvent1Handler.Handle);
            DomainEvent.RegisterCallback<TestEvent2>(mockTestEvent2Handler.Handle);

            var callbacksListFromStorage = state.Local.Get<IList<Delegate>>("DomainEvent.Callbacks");
            Assert.That(callbacksListFromStorage, Is.Not.Null);
            Assert.That(callbacksListFromStorage.Count, Is.EqualTo(2));

            DomainEvent.Raise(new TestEvent1());
            mockTestEvent1Handler.AssertWasCalled(x => x.Handle(null), options => options.IgnoreArguments());
            mockTestEvent2Handler.AssertWasNotCalled(x => x.Handle(null), options => options.IgnoreArguments());
        }

        [Test]
        public void callbacks_are_cleared_when_clear_is_called ()
        {
            var state = new FakeState();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            locator.Stub(x => x.GetInstance<IState>()).Return(state);
            ServiceLocator.SetLocatorProvider(() => locator);

            var mockTestEvent1Handler = MockRepository.GenerateMock<Handles<TestEvent1>>();
            var mockTestEvent2Handler = MockRepository.GenerateMock<Handles<TestEvent2>>();

            DomainEvent.RegisterCallback<TestEvent1>(mockTestEvent1Handler.Handle);
            DomainEvent.RegisterCallback<TestEvent2>(mockTestEvent2Handler.Handle);

            var callbacksListFromStorage = state.Local.Get<IList<Delegate>>("DomainEvent.Callbacks");
            Assert.That(callbacksListFromStorage, Is.Not.Null);
            Assert.That(callbacksListFromStorage.Count, Is.EqualTo(2));
            DomainEvent.ClearCallbacks();
            callbacksListFromStorage = state.Local.Get<IList<Delegate>>(typeof(DomainEvent).FullName + "DomainEvent.Callbacks");
            Assert.That(callbacksListFromStorage, Is.Null);
        }

        [Test]
        public void registered_handlers_are_executed_when_event_is_raised ()
        {
            var state = new FakeState();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            locator.Stub(x => x.GetInstance<IState>()).Return(state);
            ServiceLocator.SetLocatorProvider(() => locator);
            var mockTest1Handler = MockRepository.GenerateMock<Handles<TestEvent1>>();
            var mockTest2Handler = MockRepository.GenerateMock<Handles<TestEvent2>>();

            locator.Expect(x => x.GetAllInstances<Handles<TestEvent1>>())
                   .Return(new[]{mockTest1Handler});

            ServiceLocator.SetLocatorProvider(() => locator);

            DomainEvent.Raise(new TestEvent1());
            mockTest1Handler.AssertWasCalled(x => x.Handle(null), options => options.IgnoreArguments());
            mockTest2Handler.AssertWasNotCalled(x => x.Handle(null), options => options.IgnoreArguments());
        }
    }
}