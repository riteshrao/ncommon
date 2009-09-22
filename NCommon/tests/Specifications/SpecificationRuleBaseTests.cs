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
using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Rules.Tests
{
    /// <summary>
    /// Tests the <see cref="SpecificationRuleBase{TEntity}"/> base class.
    /// </summary>
    [TestFixture]
    public class SpecificationRuleBaseTests
    {
        [Test]
        public void Constructor_Throws_ArgumentNullException_When_Null_Specification_Created ()
        {
            //Since while creating a mock with a null argument, Rhino Mocks is going to return the inner
            //exception wrapped within a Exception instance, we are checking for a generic exception.
            Assert.Throws<Exception>(() => MockRepository.GenerateStub<SpecificationRuleBase<object>>(new object[] {null}));
        }

        [Test]
        public void IsSatisfied_Throws_ArgumentNullException_When_Null_Entity_Provided ()
        {
            var mockSpec = MockRepository.GenerateStub<ISpecification<object>>();
            var ruleUnderTest =
                MockRepository.GenerateStub<SpecificationRuleBase<object>>(mockSpec);

            Assert.Throws<ArgumentNullException>(() => ruleUnderTest.IsSatisfied(null));
        }

        [Test]
        public void IsSatisfied_Calls_Underlying_Specifications_IsSatisfiedBy()
        {
            var mockSpec = MockRepository.GenerateMock<ISpecification<object>>();
            mockSpec.Expect(x => x.IsSatisfiedBy(null))
                    .IgnoreArguments()
                    .Return(true);
            var ruleUnderTest =
                MockRepository.GenerateStub<SpecificationRuleBase<object>>(mockSpec);

            var result = ruleUnderTest.IsSatisfied(new object());

            mockSpec.VerifyAllExpectations();
            Assert.That(result);
        }
    }
}
