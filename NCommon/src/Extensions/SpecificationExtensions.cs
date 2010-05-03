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

#region attributions
// Code attributed to: Pablo Rui [http://ncommon.codeplex.com/Thread/View.aspx?ThreadId=50981]
#endregion  

using System;
using System.Linq;
using System.Linq.Expressions;
using NCommon.Specifications;

namespace NCommon.Extensions
{
    ///<summary>
    /// Extension methods for <see cref="ISpecification{T}"/>.
    ///</summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Retuns a new specification adding this one with the passed one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rightHand">The right hand.</param>
        /// <param name="leftHand">The left hand.</param>
        /// <returns></returns>
        public static ISpecification<T> And<T>(this ISpecification<T> rightHand, ISpecification<T> leftHand)
        {
            var rightInvoke = Expression.Invoke(rightHand.Predicate,
                                                leftHand.Predicate.Parameters.Cast<Expression>());
            var newExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftHand.Predicate.Body,
                                                      rightInvoke);
            return new Specification<T>(
                Expression.Lambda<Func<T, bool>>(newExpression, leftHand.Predicate.Parameters)
                );
        }

        /// <summary>
        /// Retuns a new specification or'ing this one with the passed one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rightHand">The right hand.</param>
        /// <param name="leftHand">The left hand.</param>
        /// <returns></returns>
        public static ISpecification<T> Or<T>(this ISpecification<T> rightHand, ISpecification<T> leftHand)
        {
            var rightInvoke = Expression.Invoke(rightHand.Predicate,
                                                leftHand.Predicate.Parameters.Cast<Expression>());
            var newExpression = Expression.MakeBinary(ExpressionType.OrElse, leftHand.Predicate.Body,
                                                      rightInvoke);
            return new Specification<T>(
                Expression.Lambda<Func<T, bool>>(newExpression, leftHand.Predicate.Parameters)
                );
        }
    }
}