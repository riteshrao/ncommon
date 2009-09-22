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
    /// Tests the <see cref="ValidationRule{TEntity}"/> class.
    /// </summary>
    [TestFixture]
    public class ValidationRuleTests
    {
        [Test]
        public void Creating_ValidationRule_With_Null_Specification_Throws_ArgumentNullException ()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidationRule<object>(null, string.Empty, string.Empty));
        }

        [Test]
        public void Creating_ValidationRule_With_Null_Message_Throws_ArgumentNullException ()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new ValidationRule<object>(MockRepository.GenerateStub<ISpecification<object>>(), null, "Test Property")
                );
        }

        [Test]
        public void Creating_ValidationRule_With_Null_Property_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new ValidationRule<object>(MockRepository.GenerateStub<ISpecification<object>>(), "Test Message", null)
                );
        }

        [Test]
        public void Validate_Returns_True_When_Specification_Returns_True ()
        {
            ISpecification<object> mockSpecification = MockRepository.GenerateMock<ISpecification<object>>();
            mockSpecification.Expect(x => x.IsSatisfiedBy(null))
                .IgnoreArguments()
                .Return(true);

            ValidationRule<object> rule = new ValidationRule<object>(mockSpecification, "Error message", "Property");
            Assert.That(rule.Validate(new object()));
        }

        [Test]
        public void Validate_Returns_False_When_Specification_Returns_False()
        {
            ISpecification<object> mockSpecification = MockRepository.GenerateMock<ISpecification<object>>();
            mockSpecification.Expect(x => x.IsSatisfiedBy(null))
                .IgnoreArguments()
                .Return(false);

            ValidationRule<object> rule = new ValidationRule<object>(mockSpecification, "Error message", "Property");
            Assert.That(!rule.Validate(new object()));
        }
    }
}
