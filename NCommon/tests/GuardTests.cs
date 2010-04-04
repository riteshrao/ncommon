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

namespace NCommon.Tests
{
    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void Against_Throws_Valid_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.Against<ArgumentNullException>(true, string.Empty));
        }

        [Test]
        public void Against_Throws_Valid_Exception_WithMessage()
        {
            string message = "Exception Message";
            Assert.Throws<ArgumentNullException>
                (
                    () => Guard.Against<ArgumentNullException>(true, message),
                    message
                );
        }

        [Test]
        public void Against_Evaluates_Lambda_WithException()
        {
            Assert.Throws<ArgumentNullException>
                (
                    () => Guard.Against<ArgumentNullException>
                        (
                            () => 1 == 1, "Guard check with lambda"
                        )
                 );
        }

        [Test]
        public void Against_Evaluates_Lambda_WithSuccess()
        {
            Assert.DoesNotThrow
            (
                () => Guard.Against<ArgumentNullException>
                    (
                        () => 1 != 1, "Guard check with lambda"
                    )
            );
        }

        [Test]
        public void TypeOf_Throws_InvalidOperationException_When_Instance_Does_Not_Match_Type ()
        {
            Assert.Throws<InvalidOperationException>(
                () => Guard.TypeOf<InvalidOperationException>(new Exception(), "Guard check with TypeOf")
                );
        }

        [Test]
        public void TypeOf_Does_Not_Throw_When_Instance_Does_Matches_Type()
        {
            Assert.DoesNotThrow(
                () => Guard.TypeOf<InvalidOperationException>(new InvalidOperationException(), "Guard check with TypeOf")
                );
        }
    }
}
