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
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.LinqToSql.Tests
{
    /// <summary>
    /// Tests the <see cref="LinqToSqlUnitOfWork"/> class.
    /// </summary>
    [TestFixture]
    public class LinqToSqlUnitOfWorkTests
    {
        [Test]
        public void ctor_throws_ArgumentNullException_when_sessionResolver_parameter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LinqToSqlUnitOfWork(null));
        }

        [Test]
        public void GetSessionFor_returns_session_for_type()
        {
            var resolver = MockRepository.GenerateStub<ILinqToSqlSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());

            var unitOfWork = new LinqToSqlUnitOfWork(resolver);
            var session = unitOfWork.GetSession<string>();
            Assert.That(session, Is.Not.Null);
        }

        [Test]
        public void GetSessionFor_returns_same_session_for_types_handled_by_same_context()
        {
            var sessionKey = Guid.NewGuid();
            var resolver = MockRepository.GenerateMock<ILinqToSqlSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(sessionKey);
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(sessionKey);
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());

            var unitOfWork = new LinqToSqlUnitOfWork(resolver);
            var stringSession = unitOfWork.GetSession<string>();
            var intSession = unitOfWork.GetSession<int>();

            Assert.That(stringSession, Is.SameAs(intSession));
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<string>());
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<int>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<string>(), c => c.Repeat.Once());
        }

        [Test]
        public void GetSessionFor_returns_different_session_for_types_handled_by_different_context()
        {
            var resolver = MockRepository.GenerateMock<ILinqToSqlSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());

            var unitOfWork = new LinqToSqlUnitOfWork(resolver);
            var stringSession = unitOfWork.GetSession<string>();
            var intSession = unitOfWork.GetSession<int>();

            Assert.That(stringSession, Is.Not.SameAs(intSession));
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<string>());
            resolver.AssertWasCalled(x => x.GetSessionKeyFor<int>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<string>());
            resolver.AssertWasCalled(x => x.OpenSessionFor<int>());
        }

        [Test]
        public void Flush_calls_SubmitChanges_on_all_open_ILinqToSqlSession_instances()
        {
            var resolver = MockRepository.GenerateMock<ILinqToSqlSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());

            var unitOfWork = new LinqToSqlUnitOfWork(resolver);
            unitOfWork.GetSession<string>();
            unitOfWork.GetSession<int>();

            unitOfWork.Flush();
            resolver.OpenSessionFor<string>().AssertWasCalled(x => x.SubmitChanges());
            resolver.OpenSessionFor<int>().AssertWasCalled(x => x.SubmitChanges());
        }

        [Test]
        public void Dispose_disposes_all_open_ILinqToSqlSession_instances()
        {
            var resolver = MockRepository.GenerateMock<ILinqToSqlSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ILinqToSqlSession>());

            var unitOfWork = new LinqToSqlUnitOfWork(resolver);
            unitOfWork.GetSession<string>();
            unitOfWork.GetSession<int>();

            unitOfWork.Dispose();
            resolver.OpenSessionFor<string>().AssertWasCalled(x => x.Dispose());
            resolver.OpenSessionFor<int>().AssertWasCalled(x => x.Dispose());
        }
    }
}