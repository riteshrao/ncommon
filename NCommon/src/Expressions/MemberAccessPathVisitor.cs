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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NCommon.Expressions
{
    /// <summary>
    /// Inherits from the <see cref="ExpressionVisitor"/> base class and implements a expression visitor
    /// that builds up a path string that represents meber access in a Expression.
    /// </summary>
    public class MemberAccessPathVisitor : ExpressionVisitor
    {
        //StringBuilder instance that will store the path.
        private readonly Stack<string> _path = new Stack<string>();

        /// <summary>
        /// Gets the path analyzed by the visitor.
        /// </summary>
        public string Path
        {
            get {
                var pathString = new StringBuilder();
                foreach (string path in _path)
                {
                    if (pathString.Length == 0)
                        pathString.Append(path);
                    else
                        pathString.AppendFormat(".{0}", path);
                }
                return pathString.ToString();
            }
        }

        /// <summary>
        /// Overriden. Overrides all MemberAccess to build a path string.
        /// </summary>
        /// <param name="methodExp"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression methodExp)
        {
            if (methodExp.Member.MemberType != MemberTypes.Field && methodExp.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException("MemberAccessPathVisitor does not support a member access of type " +
                                                methodExp.Member.MemberType.ToString());
            _path.Push(methodExp.Member.Name);
            return base.VisitMemberAccess(methodExp);
        }

        /// <summary>
        /// Overriden. Throws a <see cref="NotSupportedException"/> when a method call is encountered.
        /// </summary>
        /// <param name="methodCallExp"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExp)
        {
            throw new NotSupportedException(
                "MemberAccessPathVisitor does not support method calls. Only MemberAccess expressions are allowed.");
        }
    }
}
