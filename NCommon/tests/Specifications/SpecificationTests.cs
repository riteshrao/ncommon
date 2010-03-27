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

namespace NCommon.Specifications.Tests
{
    /// <summary>
    /// Tests the <see cref="Specification{T}"/> class.
    /// </summary>
    [TestFixture]
    public class SpecificationTests
    {
        private class TestObject
        {
            public string FirstName;
            public string LastName;
            public int Age;
        }

        [Test]
        public void Creating_Specification_With_Null_Expression_Throws_ArgumentNullException ()
        {
            Assert.Throws<ArgumentNullException>(() => new Specification<object>(null));
        }

        [Test]
        public void Simple_Specification_Should_Eval_ToFalse()
        {
            var testObject = new TestObject {FirstName = "John", LastName = "Doe", Age = 20};
            var testSpec = new Specification<TestObject>(x => x.Age > 30);
            Assert.That(!testSpec.IsSatisfiedBy(testObject));
        }

        [Test]
        public void Simple_Specification_Should_Eval_ToTrue()
        {
            var testObject = new TestObject { FirstName = "John", LastName = "Doe", Age = 20 };
            var testSpec = new Specification<TestObject>(x => x.Age < 30);
            Assert.That(testSpec.IsSatisfiedBy(testObject));
        }

        [Test]
        public void Complex_Specification_Should_Eval_ToFalse()
        {
            var testObject = new TestObject { FirstName = "John", LastName = "Doe", Age = 20 };
            var testSpec = new Specification<TestObject>(x => x.Age > 30) &
                           new Specification<TestObject>(x => x.FirstName.StartsWith("J"));
            Assert.That(!testSpec.IsSatisfiedBy(testObject));
        }

        [Test]
        public void Complex_Specification_Should_Eval_ToTrue()
        {
            var testObject = new TestObject { FirstName = "John", LastName = "Doe", Age = 20 };
            var testSpec = new Specification<TestObject>(x => x.Age < 30) &
                           new Specification<TestObject>(x => x.FirstName.StartsWith("J"));
            Assert.That(testSpec.IsSatisfiedBy(testObject));
        }
    }
}
