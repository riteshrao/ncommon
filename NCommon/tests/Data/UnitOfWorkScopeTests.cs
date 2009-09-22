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
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests
{
    /// <summary>
    /// Tests the <see cref="UnitOfWorkScope"/> class.
    /// </summary>
    [TestFixture]
    public class UnitOfWorkScopeTests
    {
        [Test]
        public void Creating_Scope_Starts_New_UnitOfWork_Session ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
            mockUOWFactory.Expect(x => x.Create()).IgnoreArguments().Return(mockUOW);
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(mockTransaction);
            ServiceLocator.SetLocatorProvider(() => mockLocator);

            Assert.That(!UnitOfWorkScope.HasStarted);
            Assert.That(UnitOfWorkScope.Current, Is.Null);

            using (var scope = new UnitOfWorkScope())
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                Assert.That(UnitOfWorkScope.Current, Is.EqualTo(scope));
                Assert.That(scope.UnitOfWork, Is.Not.Null);
                Assert.That(scope.UnitOfWork, Is.SameAs(mockUOW));
            }

            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
        }

        [Test]
        public void Disposing_Scope_Calls_Rollback_On_Transaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
            mockUOWFactory.Expect(x => x.Create()).IgnoreArguments().Return(mockUOW);
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(mockTransaction);
            mockUOW.Expect(x => x.Dispose());

            mockTransaction.Expect(x => x.Rollback());
            mockTransaction.Expect(x => x.Dispose());

            ServiceLocator.SetLocatorProvider(() => mockLocator);

            using (new UnitOfWorkScope())
            {
                Assert.That(UnitOfWorkScope.HasStarted);
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Commit_Scope_Calls_Flush_On_UOW_And_Commit_On_Transaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
            mockUOWFactory.Expect(x => x.Create()).IgnoreArguments().Return(mockUOW);
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(mockTransaction);
            mockUOW.Expect(x => x.Flush());
            mockUOW.Expect(x => x.Dispose());

            mockTransaction.Expect(x => x.Commit());
            mockTransaction.Expect(x => x.Dispose());

            ServiceLocator.SetLocatorProvider(() => mockLocator);

            using (var scope = new UnitOfWorkScope())
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                scope.Commit();
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Same_UnitOfWork_When_No_UnitOfWorkScopeTransactionOption_Specified ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            IUnitOfWork mockUOW;
            
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
            mockUOWFactory.Expect(x => x.Create())
                            .Do (new Func<IUnitOfWork>(() =>
                                     {
                                         mockUOW = MockRepository.GenerateStub<IUnitOfWork>();
                                         mockUOW.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                                             .IgnoreArguments()
                                             .Return(MockRepository.GenerateStub<ITransaction>());
                                         return mockUOW;
                                     })
                                )
                            .Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);

            using (var parentScope = new UnitOfWorkScope())
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (var childScope = new UnitOfWorkScope())
                {
                    Assert.That(parentScope.UnitOfWork, Is.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Different_UnitOfWork_When_Different_IsolationLevel_Is_Specified ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            IUnitOfWork mockUOW;

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Twice();
            mockUOWFactory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                                                          {
                                                              mockUOW = MockRepository.GenerateStub<IUnitOfWork>();
                                                              mockUOW.Stub(
                                                                  x => x.BeginTransaction(IsolationLevel.Unspecified))
                                                                  .IgnoreArguments()
                                                                  .Return(MockRepository.GenerateStub<ITransaction>());
                                                              return mockUOW;
                                                          }))
                            .Repeat.Twice();


            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.Chaos))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (var childScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
                {
                    Assert.That(parentScope.UnitOfWork, Is.Not.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Different_UnitOfWork_When_UnitOfWorkScopeTransactionOption_Is_NewTransaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            IUnitOfWork mockUOW = null;

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Twice();
            mockUOWFactory.Expect(x => x.Create())
                            .Do(new Func<IUnitOfWork>(() =>
                                                          {
                                                              mockUOW = MockRepository.GenerateStub<IUnitOfWork>();
                                                              mockUOW.Stub(
                                                                  x => x.BeginTransaction(IsolationLevel.Unspecified))
                                                                  .IgnoreArguments()
                                                                  .Return(MockRepository.GenerateStub<ITransaction>());
                                                              return mockUOW;
                                                          }))
                            .Repeat.Twice();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (var childScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted, UnitOfWorkScopeTransactionOptions.CreateNew))
                {
                    Assert.That(parentScope.UnitOfWork, Is.Not.SameAs(childScope.UnitOfWork));
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
        }

        [Test]
        public void Creating_Child_Scope_Uses_Same_UnitOfWork_When_Same_IsolationLevel_And_Default_UnitOfWorkScopeTransactionOption_Specified()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (new UnitOfWorkScope(IsolationLevel.ReadCommitted)){ }
            }
            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();   
        }

        [Test]
        public void Calling_Commit_On_Parent_Scope_Throws_InvalidOperaitonException_When_Child_Scope_Active ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();
            
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (new UnitOfWorkScope(IsolationLevel.ReadCommitted))
                {
                    Assert.Throws<InvalidOperationException>(parentScope.Commit);
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Parent_Scope_Throws_InvalidOperationException_When_Child_Disposed_And_Transaction_Rolledback ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (new UnitOfWorkScope(IsolationLevel.ReadCommitted)) {}
                Assert.Throws<InvalidOperationException>(parentScope.Commit);
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
            mockUOWFactory.VerifyAllExpectations();
            mockUOW.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Calling_Commit_On_Child_Scope_DoesNot_Commit_Transaction ()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            var mockUOWFactory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            var mockUOW = MockRepository.GenerateMock<IUnitOfWork>();
            var mockTransaction = MockRepository.GenerateMock<ITransaction>();

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Never();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (var childScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted)) 
                {
                    childScope.Commit();
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
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
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockUOW.Expect(x => x.Flush()).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Once();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Never();


            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (var childScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
                {
                    childScope.Commit();
                }
                parentScope.Commit();
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
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

            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory).Repeat.Once();
            mockUOWFactory.Expect(x => x.Create()).Return(mockUOW).Repeat.Once();
            mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction).Repeat.Once();
            mockTransaction.Expect(x => x.Commit()).Repeat.Never();
            mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

            ServiceLocator.SetLocatorProvider(() => mockLocator);
            using (var parentScope = new UnitOfWorkScope(IsolationLevel.ReadCommitted))
            {
                Assert.That(UnitOfWorkScope.HasStarted);
                using (new UnitOfWorkScope(IsolationLevel.ReadCommitted))
                {
                    Assert.Throws<InvalidOperationException>(parentScope.Dispose);
                }
            }

            Assert.That(!UnitOfWorkScope.HasStarted);
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

			mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
			mockUOWFactory.Expect(x => x.Create()).Return(mockUOW);
			mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction);
			mockTransaction.Expect(x => x.Commit()).Repeat.Once();
			mockTransaction.Expect(x => x.Rollback()).Repeat.Never();

			ServiceLocator.SetLocatorProvider(() => mockLocator);
			using (var scope = new UnitOfWorkScope(IsolationLevel.ReadCommitted, UnitOfWorkScopeTransactionOptions.AutoComplete))
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

			mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(mockUOWFactory);
			mockUOWFactory.Expect(x => x.Create()).Return(mockUOW);
			mockUOW.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).IgnoreArguments().Return(mockTransaction);
			mockTransaction.Expect(x => x.Commit()).Throw(new Exception()).Repeat.Once();
			mockTransaction.Expect(x => x.Rollback()).Repeat.Once();

			ServiceLocator.SetLocatorProvider(() => mockLocator);

			try
			{
				using (var scope = new UnitOfWorkScope(IsolationLevel.ReadCommitted, UnitOfWorkScopeTransactionOptions.AutoComplete))
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