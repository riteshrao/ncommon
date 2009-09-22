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
using NCommon.LinqToSql.Tests.Domain;
using NUnit.Framework;

namespace NCommon.Data.LinqToSql.Tests
{
    /// <summary>
    /// Tests the <see cref="LinqToSqlUnitOfWorkFactory"/> class.
    /// </summary>
    [TestFixture]
    public class LinqToSqlUnitOfWorkFactoryTests
    {
        [Test]
        public void Create_Throws_InvalidOperationException_When_No_DataContextProvider_Has_Been_Set()
        {
            var factory = new LinqToSqlUnitOfWorkFactory();
            Assert.Throws<InvalidOperationException>(
                () => factory.Create()
                );
        }

        [Test]
        public void Create_Returns_LinqToSqlUnitOfWork_Instance_When_DataContextProvider_Has_Been_Set()
        {

            LinqToSqlUnitOfWorkFactory.SetDataContextProvider(() => new TestDataDataContext());

            var factory = new LinqToSqlUnitOfWorkFactory();
            var uowInstance = factory.Create();

            Assert.That(uowInstance, Is.Not.Null);
            Assert.That(uowInstance, Is.TypeOf(typeof(LinqToSqlUnitOfWork)));

            LinqToSqlUnitOfWorkFactory.SetDataContextProvider(null);
        }
    }
}
