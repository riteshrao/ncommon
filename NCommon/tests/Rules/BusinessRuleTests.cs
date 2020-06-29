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

using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Validation.Tests
{
    /// <summary>
    /// Tests the <see cref="BusinessRule{TEntity}"/> class
    /// </summary>
    [TestFixture]
    public class BusinessRuleTests
    {
        [Test]
        public void ArgumentNullException_is_thrown_when_created_with_null_specification ()
        {
            Assert.Throws<ArgumentNullException>(() => new BusinessRule<object>(null, null));
        }

        [Test]
        public void ArgumentNullException_is_thrown_when_created_with_null_action ()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BusinessRule<object>(MockRepository.GenerateStub<ISpecification<object>>(), null));
        }

        [Test]
        public void no_action_is_performed_when_specification_returns_false ()
        {
            ISpecification<object> mockSpec = MockRepository.GenerateMock<ISpecification<object>>();
            mockSpec.Expect(x => x.IsSatisfiedBy(null))
                .IgnoreArguments()
                .Return(false);

            bool actionPerformed = false;
            BusinessRule<object> rule = new BusinessRule<object>(mockSpec, x => actionPerformed = true);
            rule.Evaluate(new object());

            mockSpec.VerifyAllExpectations();
            Assert.That(!actionPerformed);
        }

        [Test]
        public void action_is_executed_when_specification_returns_true()
        {
            ISpecification<object> mockSpec = MockRepository.GenerateMock<ISpecification<object>>();
            mockSpec.Expect(x => x.IsSatisfiedBy(null))
                .IgnoreArguments()
                .Return(true);

            bool actionPerformed = false;
            BusinessRule<object> rule = new BusinessRule<object>(mockSpec, x => actionPerformed = true);
            rule.Evaluate(new object());

            mockSpec.VerifyAllExpectations();
            Assert.That(actionPerformed);
        }
    }
}
