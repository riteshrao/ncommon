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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.Tests
{
    /// <summary>
    /// The UnitOfWorkTests class tests the <see cref="UnitOfWork"/> class.
    /// </summary>
    [TestFixture]
    public class UnitOfWorkTests
    {
        [Test]
        public void Calling_Start_Starts_A_UnitOfWork_Instance()
        {
            Assert.That(!UnitOfWork.HasStarted);
            Assert.That(UnitOfWork.Current, Is.Null);

            
            var mockUOWFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var mockUOWInstance = MockRepository.GenerateStub<IUnitOfWork>();
            mockUOWFactory.Stub(x => x.Create()).Return(mockUOWInstance);

            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).IgnoreArguments().Return(mockUOWFactory);
            ServiceLocator.SetLocatorProvider(() => mockLocator);

            var uowInstance = UnitOfWork.Start();

            Assert.That(UnitOfWork.HasStarted);
            Assert.That(uowInstance, Is.Not.Null);
            Assert.That(UnitOfWork.Current, Is.Not.Null);
            Assert.That(uowInstance, Is.SameAs(UnitOfWork.Current));
            Assert.That(uowInstance, Is.SameAs(mockUOWInstance));
        }

        [Test]
        public void Calling_Start_On_Already_Started_UnitOfWork_Returns_Same_UnitOfWork ()
        {
            var mockUOWFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var mockUOWInstance = MockRepository.GenerateStub<IUnitOfWork>();
            mockUOWFactory.Stub(x => x.Create()).Return(mockUOWInstance);

            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).IgnoreArguments().Return(mockUOWFactory);
            ServiceLocator.SetLocatorProvider(() => mockLocator);

            IUnitOfWork uowInstance = UnitOfWork.Start();
            Assert.That(UnitOfWork.HasStarted);
            Assert.That(UnitOfWork.Current, Is.SameAs(uowInstance));
            Assert.That(UnitOfWork.Start(), Is.SameAs(uowInstance));
            UnitOfWork.Finish(false);
        }

        [Test]
        public void Calling_Finish_Without_Start_Should_Throw_InvalidOperationException()
        {
            Assert.That(!UnitOfWork.HasStarted);
            Assert.That(UnitOfWork.Current, Is.Null);
            Assert.Throws(typeof(InvalidOperationException), () => UnitOfWork.Finish(false));
        }

        [Test]
        public void Calling_Finish_When_UnitOfWork_Started_Finishes_Current_UnitOfWork_An_Resets_Current_UnitOfWork()
        {
            //Mock setups
            var mockUOWFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var mockUOWInstance = MockRepository.GenerateStub<IUnitOfWork>();

            mockUOWFactory.Stub(x => x.Create()).Return(mockUOWInstance);

            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            mockLocator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).IgnoreArguments().Return(mockUOWFactory);
            ServiceLocator.SetLocatorProvider(() => mockLocator);

            IUnitOfWork uowInstance = UnitOfWork.Start();
            Assert.That(UnitOfWork.HasStarted);
            Assert.That(UnitOfWork.Current, Is.Not.Null);
            Assert.That(uowInstance, Is.Not.Null);

            UnitOfWork.Finish(true);
        }
    }
}