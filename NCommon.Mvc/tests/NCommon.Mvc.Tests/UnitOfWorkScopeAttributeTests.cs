using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using NCommon.DataServices.Transactions;
using NCommon.Mvc;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    [TestFixture]
    public class UnitOfWorkScopeAttributeTests
    {
        readonly FakeTransactionManager _transactionManager = new FakeTransactionManager();

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(() => _transactionManager);
        }

        [Test]
        public void Start_configures_and_adds_new_scope_to_current_http_context()
        {
            var contextItems = new Dictionary<string, object>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ControllerContext>();
            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;

            new UnitOfWorkAttribute().Start(mockControllerContext);
            
            Assert.NotNull(contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey]);
        }

        [Test]
        public void OnActionExecuting_starts_a_new_scope()
        {
            var contextItems = new Dictionary<string, object>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ActionExecutingContext>();
            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;

            new UnitOfWorkAttribute().OnActionExecuting(mockControllerContext);

            Assert.NotNull(contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey]);
        }

        [Test]
        public void OnActionExecuted_commits_transaction_if_filter_scope_is_action()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ActionExecutedContext>();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Action }.OnActionExecuted(mockControllerContext);

            mockScope.AssertWasCalled(x => x.Commit());
            mockScope.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void OnActionExecuted_rollsback_transaction_if_exception_occured()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ActionExecutedContext>();
            mockControllerContext.Exception = new Exception();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Action }.OnActionExecuted(mockControllerContext);

            mockScope.AssertWasNotCalled(x => x.Commit());
            mockScope.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void OnActionExecuted_does_nothing_if_filter_scope_is_result()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ActionExecutedContext>();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Result }.OnActionExecuted(mockControllerContext);

            mockScope.AssertWasNotCalled(x => x.Commit());
            mockScope.AssertWasNotCalled(x => x.Dispose());
        }

        [Test]
        public void OnResultExecuted_does_nothing_if_filter_scope_is_action()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ResultExecutedContext>();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Action }.OnResultExecuted(mockControllerContext);

            mockScope.AssertWasNotCalled(x => x.Commit());
            mockScope.AssertWasNotCalled(x => x.Dispose());
        }

        [Test]
        public void OnResultExecuted_rollsback_if_filter_scope_is_result_and_exception_occured()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ResultExecutedContext>();
            mockControllerContext.Exception =  new Exception();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Result }.OnResultExecuted(mockControllerContext);

            mockScope.AssertWasNotCalled(x => x.Commit());
            mockScope.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void OnResultExecuted_comitts_transaction()
        {
            var contextItems = new Dictionary<string, object>();
            var mockScope = MockRepository.GenerateMock<IUnitOfWorkScope>();
            var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var mockControllerContext = MockRepository.GenerateStub<ResultExecutedContext>();

            mockHttpContext.Stub(x => x.Items).Return(contextItems);
            mockControllerContext.HttpContext = mockHttpContext;
            contextItems[UnitOfWorkAttribute.ContextUnitOfWorkKey] = mockScope;

            new UnitOfWorkAttribute { Scope = UnitOfWorkAttribute.FilterScope.Result }.OnResultExecuted(mockControllerContext);

            mockScope.AssertWasCalled(x => x.Commit());
            mockScope.AssertWasCalled(x => x.Dispose());
        }
    }
}