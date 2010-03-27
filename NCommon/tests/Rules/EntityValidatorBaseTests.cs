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

using System.Linq;
using NCommon.Extensions;
using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Rules.Tests
{
    /// <summary>
    /// Tests the <see cref="EntityValidatorBase{TEntity}"/>
    /// </summary>
    [TestFixture]
    public class EntityValidatorBaseTests
    {
        #region MockEntityValidator
        private class MockEntityValidator : EntityValidatorBase<object>
        {
            public new void AddValidation(string ruleName, IValidationRule<object> rule)
            {
                base.AddValidation(ruleName, rule);
            }
        }
        #endregion
        
        [Test]
        public void Validate_returns_two_validation_errors ()
        {
            ISpecification<object> failedSpec1 = MockRepository.GenerateStub<ISpecification<object>>();
            failedSpec1.Stub(x => x.IsSatisfiedBy(null)).IgnoreArguments().Return(false);

            ISpecification<object> failedSpec2 = MockRepository.GenerateStub<ISpecification<object>>();
            failedSpec2.Stub(x => x.IsSatisfiedBy(null)).IgnoreArguments().Return(false);

            ISpecification<object> passedSpec = MockRepository.GenerateStub<ISpecification<object>>();
            passedSpec.Stub(x => x.IsSatisfiedBy(null)).IgnoreArguments().Return(true);

            MockEntityValidator validator = new MockEntityValidator();
            validator.AddValidation("Failed Rule1", new ValidationRule<object>(failedSpec1, "Validation 1 Failed", "Validation1"));
            validator.AddValidation("Failed Rule2", new ValidationRule<object>(failedSpec2, "Validation 2 Failed", "Validation2"));
            validator.AddValidation("Passed Rule", new ValidationRule<object>(passedSpec, "Successfull Validation", "ShouldNotAppear"));

            ValidationResult result = validator.Validate(new object());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Errors.Count(), Is.GreaterThan(0));
            Assert.That(result.Errors.Count(), Is.EqualTo(2));

            result.Errors.ForEach(x => Assert.That(x.Property, Is.Not.EqualTo("ShouldNotAppear")));
        }
    }
}
