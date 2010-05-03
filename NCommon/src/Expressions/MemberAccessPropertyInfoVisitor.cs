#region license
//Copyright 2010 Ritesh Rao 

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
using System.Linq.Expressions;
using System.Reflection;

namespace NCommon.Expressions
{
    /// <summary>
    /// Inherits from the <see cref="ExpressionVisitor"/> base class and implements a expression visitor
    /// that gets a <see cref="PropertyInfo"/> that represents the property representd by the expresion.
    /// </summary>
    public class MemberAccessPropertyInfoVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> that the expression represents.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Overriden. Overrides all MemberAccess to build a path string.
        /// </summary>
        /// <param name="methodExp"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression methodExp)
        {
            if (methodExp.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException("MemberAccessPathVisitor does not support a member access of type " +
                                                methodExp.Member.MemberType);
            this.Property = (PropertyInfo) methodExp.Member;
            return base.VisitMemberAccess(methodExp);
        }
    }
}