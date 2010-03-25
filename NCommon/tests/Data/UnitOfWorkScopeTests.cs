#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Data;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;
using NCommon.Data.Tests;
using NCommon.State;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests
{
    /// <summary>
    /// Tests the <see cref="UnitOfWork"/> class.
    /// </summary>
    [TestFixture]
    public class UnitOfWorkScopeTests
    {
        [Test]
        public void Creating_Scope_Starts_New_UnitOfWork_Session()
        {
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var unitOfWork = MockRepository.GenerateMock<IUnitOfWork>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(MockRepository.GenerateStub<IUnitOfWorkFactory>());
            locator.GetInstance<IUnitOfWorkFactory>().Stub(x => x.Create()).Return(unitOfWork);
            unitOfWork.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(MockRepository.GenerateStub<ITransaction>());
            ServiceLocator.SetLocatorProvider(() => locator);

            Assert.That(!UnitOfWork.HasStarted);
            Assert.That(UnitOfWork.Current, Is.Null);

            using (var scope = new UnitOfWork())
            {
                Assert.That(UnitOfWork.HasStarted);
                Assert.That(UnitOfWork.Current, Is.EqualTo(scope));
                Assert.That(scope.UnitOfWork, Is.Not.Null);
                Assert.That(scope.UnitOfWork, Is.SameAs(unitOfWork));
            }
            unitOfWork.VerifyAllExpectations();
        }

        [Test]
        public void Disposing_Scope_Calls_Rollback_On_Transaction ()
        {
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var unitOfWork = MockRepository.GenerateMock<IUnitOfWork>();
            var transaction = MockRepository.GenerateMock<ITransaction>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(MockRepository.GenerateStub<IUnitOfWorkFactory>());
            locator.GetInstance<IUnitOfWorkFactory>().Stub(x => x.Create()).Return(unitOfWork);
            unitOfWork.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(transaction);
            ServiceLocator.SetLocatorProvider(() => locator);
            
            using (new UnitOfWork())
            {
                Assert.That(UnitOfWork.HasStarted);
            }

            Assert.That(!UnitOfWork.HasStarted);
            unitOfWork.AssertWasCalled(x => x.Dispose());
            transaction.AssertWasCalled(x => x.Rollback());
            transaction.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void Commit_Scope_Calls_Flush_On_UOW_And_Commit_On_Transaction ()
        {
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var unitOfWork = MockRepository.GenerateMock<IUnitOfWork>();
            var transaction = MockRepository.GenerateMock<ITransaction>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(MockRepository.GenerateStub<IUnitOfWorkFactory>());
            locator.GetInstance<IUnitOfWorkFactory>().Stub(x => x.Create()).Return(unitOfWork);
            unitOfWork.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(transaction);
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var scope = new UnitOfWork())
            {
                Assert.That(UnitOfWork.HasStarted);
                scope.Commit();
            }

            Assert.That(!UnitOfWork.HasStarted);
            unitOfWork.Expect(x => x.Flush());
            unitOfWork.Expect(x => x.Dispose());
            transaction.Expect(x => x.Commit());
            transaction.Expect(x => x.Dispose());
        }

        [Test]
        public void Creating_Child_Scope_Uses_Same_UnitOfWork_When_No_UnitOfWorkScopeTransactionOption_Specified ()
        {
            IUnitOfWork unitOfWork;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do (new Func<IUnitOfWork>(() =>
                                     {
                                         unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                         unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                             .IgnoreArguments()
                                             .Return(MockRepository.GenerateStub<ITransaction>());
                                         return unitOfWork;
                                     })
                                )
                            .Repeat.Once();
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var parentScope = new UnitOfWork())
            {
                Assert.That(UnitOfWork.HasStarted);
                using (var childScope = new UnitOfWork())
                {
                    Assert.That(parentScope.UnitOfWork, Is.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Different_UnitOfWork_When_Different_IsolationLevel_Is_Specified ()
        {
            IUnitOfWork unitOfWork;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                            {
                                unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                    .IgnoreArguments()
                                    .Return(MockRepository.GenerateStub<ITransaction>());
                                return unitOfWork;
                            })).Repeat.Twice();
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var parentScope = new UnitOfWork(IsolationLevel.Chaos))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (var childScope = new UnitOfWork(IsolationLevel.ReadCommitted))
                {
                    Assert.That(parentScope.UnitOfWork, Is.Not.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Different_UnitOfWork_When_UnitOfWorkScopeTransactionOption_Is_NewTransaction ()
        {
            IUnitOfWork unitOfWork;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                            {
                                unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                    .IgnoreArguments()
                                    .Return(MockRepository.GenerateStub<ITransaction>());
                                return unitOfWork;
                            })).Repeat.Twice();
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var parentScope = new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (var childScope = new UnitOfWork(IsolationLevel.ReadCommitted, UnitOfWorkScopeOptions.CreateNew))
                {
                    Assert.That(parentScope.UnitOfWork, Is.Not.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Same_UnitOfWork_When_Same_IsolationLevel_And_Default_UnitOfWorkScopeTransactionOption_Specified()
        {
            IUnitOfWork unitOfWork;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                            {
                                unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                    .IgnoreArguments()
                                    .Return(MockRepository.GenerateStub<ITransaction>());
                                return unitOfWork;
                            })).Repeat.Once();
            ServiceLocator.SetLocatorProvider(() => locator);
           
            using (new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (new UnitOfWork(IsolationLevel.ReadCommitted)){ }
            }
            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Parent_Scope_Throws_InvalidOperaitonException_When_Child_Scope_Active ()
        {
            IUnitOfWork unitOfWork = null;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                            {
                                unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                    .IgnoreArguments()
                                    .Return(MockRepository.GenerateStub<ITransaction>());
                                return unitOfWork;
                            })).Repeat.Once();
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var parentScope = new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (new UnitOfWork(IsolationLevel.ReadCommitted))
                {
                    Assert.Throws<InvalidOperationException>(parentScope.Commit);
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
            unitOfWork.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Parent_Scope_Throws_InvalidOperationException_When_Child_Disposed_And_Transaction_Rolledback ()
        {
            IUnitOfWork unitOfWork = null;
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            locator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(factory);
            factory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                            {
                                unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
                                unitOfWork.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                    .IgnoreArguments()
                                    .Return(MockRepository.GenerateStub<ITransaction>());
                                return unitOfWork;
                            })).Repeat.Once();
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var parentScope = new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (new UnitOfWork(IsolationLevel.ReadCommitted)) {}
                Assert.Throws<InvalidOperationException>(parentScope.Commit);
            }

            Assert.That(!UnitOfWork.HasStarted);
            factory.VerifyAllExpectations();
            unitOfWork.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Child_Scope_DoesNot_Commit_Transaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Never();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (var childScope = new UnitOfWork(IsolationLevel.ReadCommitted)) 
                {
                    childScope.Commit();
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Parent_Scope_After_Child_Commit_Calls_Flush_On_UOW_And_Commit_On_Transaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockUOW.Expect(x => x.Flush()).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Once();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Never();


            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (var childScope = new UnitOfWork(IsolationLevel.ReadCommitted))
                {
                    childScope.Commit();
                }
                parentScope.Commit();
            }

            Assert.That(!UnitOfWork.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Dispose_On_Parent_Scope_Throws_InvalidOperationException_When_Child_Scope_Still_Active ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Never();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWork(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWork.HasStarted);
                using (new UnitOfWork(IsolationLevel.ReadCommitted))
                {
                    Assert.Throws<InvalidOperationException>(parentScope.Dispose);
                }
            }

            Assert.That(!UnitOfWork.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

		[Test]
		public void Starting_UnitOfWork_with_AutoComplete_automatically_commits_when_disposed()
		{
			var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
			var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
			var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
			mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
			mockUOWFactory.Expect(x => x.Create()).Return(mockUOW);
			mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction);
			mockTransaction.Expect(x => x.Commit()).Repeat.Once();
			mockTransaction.Expect(x => x.Rollback()).Repeat.Never();

			ServiceLocator.SetLocatorProvider(() => mockLocator);
			using (var scope = new UnitOfWork(IsolationLevel.ReadCommitted, UnitOfWorkScopeOptions.AutoComplete))
			{
				Assert.That(scope, Is.Not.Null);
			}

			mockUOW.VerifyAllExpectations();
			mockUOWFactory.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void Rollback_should_be_called_when_Commit_on_UnitOfWorkScope_throws_exception ()
		{
			var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
			var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
			var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
			mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
			mockUOWFactory.Expect(x => x.Create()).Return(mockUOW);
			mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction);
			mockTransaction.Expect(x => x.Commit()).Throw(new Exception()).Repeat.Once();
			mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

			ServiceLocator.SetLocatorProvider(() => mockLocator);

			try
			{
				using (var scope = new UnitOfWork(IsolationLevel.ReadCommitted, UnitOfWorkScopeOptions.AutoComplete))
				{
					Assert.That(scope, Is.Not.Null);
				}
			}
			catch {}

			mockUOW.VerifyAllExpectations();
			mockUOWFactory.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}
    }
}