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

using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Rules.Tests
{
    /// <summary>
    /// Tests the <see cref="BusinessRulesEvaluatorBase{TEntity}"/> class.
    /// </summary>
    [TestFixture]
    public class BusinessRuleEvaluatorBaseTests
    {
        #region MockBusinessRuleEvaulator
        private class MockBusinessRuleEvaluator : BusinessRulesEvaluatorBase<object>
        {
            public new void AddRule (string ruleName, IBusinessRule<object> rule)
            {
                base.AddRule(ruleName, rule);
            }
        }
        #endregion

        [Test]
        public void Evaluate_Calls_Only_Satisfied_Business_Rules ()
        {
            int timesRuleActionInvoked = 0;

            ISpecification<object> passedSpec = MockRepository.GenerateStub<ISpecification<object>>();
            passedSpec.Stub(x => x.IsSatisfiedBy(null)).IgnoreArguments().Return(true);

            ISpecification<object> failedSpec = MockRepository.GenerateStub<ISpecification<object>>();
            failedSpec.Stub(x => x.IsSatisfiedBy(null)).IgnoreArguments().Return(false);

            MockBusinessRuleEvaluator evaluator = new MockBusinessRuleEvaluator();
            evaluator.AddRule("PassedRule", new BusinessRule<object>(passedSpec, delegate { timesRuleActionInvoked++; }));
            evaluator.AddRule("FailedRule", new BusinessRule<object>(failedSpec, delegate { timesRuleActionInvoked++; }));

            evaluator.Evauluate(new object());
            Assert.That(timesRuleActionInvoked, Is.EqualTo(1));
        }

    }
}
