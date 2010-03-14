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

namespace NCommon
{
    /// <summary>
    /// Provides utility methods to guard parameter and local variables.
    /// </summary>
    public class Guard
    {
        /// <summary>
        /// Throws an exception of type <typeparamref name="TException"/> with the specified message
        /// when the assertion statement is true.
        /// </summary>
        /// <typeparam name="TException">The type of exception to throw.</typeparam>
        /// <param name="assertion">The assertion to evaluate. If true then the <typeparamref name="TException"/> exception is thrown.</param>
        /// <param name="message">string. The exception message to throw.</param>
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
                throw (TException) Activator.CreateInstance(typeof (TException), message);
        }

        /// <summary>
        /// Throws an exception of type <typeparamref name="TException"/> with the specified message
        /// when the assertion
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        public static void Against<TException>(Func<bool> assertion, string message) where TException : Exception
        {
            //Execute the lambda and if it evaluates to true then throw the exception.
            if (assertion())
                throw (TException) Activator.CreateInstance(typeof (TException), message);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> when the specified object
        /// instance does not inherit from <typeparamref name="TBase"/> type.
        /// </summary>
        /// <typeparam name="TBase">The base type to check for.</typeparam>
        /// <param name="instance">The object to check if it inherits from <typeparamref name="TBase"/> type.</param>
        /// <param name="message">string. The exception message to throw.</param>
        public static void InheritsFrom<TBase>(object instance, string message) where TBase : Type
        {
            InheritsFrom<TBase>(instance.GetType(), message);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> when the specified type does not
        /// inherit from the <typeparamref name="TBase"/> type.
        /// </summary>
        /// <typeparam name="TBase">The base type to check for.</typeparam>
        /// <param name="type">The <see cref="Type"/> to check if it inherits from <typeparamref name="TBase"/> type.</param>
        /// <param name="message">string. The exception message to throw.</param>
        public static void InheritsFrom<TBase>(Type type, string message)
        {
            if (type.BaseType != typeof (TBase))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> when the specified object
        /// instance does not implement the <typeparamref name="TInterface"/> interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type the object instance should implement.</typeparam>
        /// <param name="instance">The object insance to check if it implements the <typeparamref name="TInterface"/> interface</param>
        /// <param name="message">string. The exception message to throw.</param>
        public static void Implements<TInterface>(object instance, string message)
        {
            Implements<TInterface>(instance.GetType(), message);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when the specified type does not
        /// implement the <typeparamref name="TInterface"/> interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type that the <paramref name="type"/> should implement.</typeparam>
        /// <param name="type">The <see cref="Type"/> to check if it implements from <typeparamref name="TInterface"/> interface.</param>
        /// <param name="message">string. The exception message to throw.</param>
        public static void Implements<TInterface>(Type type, string message)
        {
            if (!typeof (TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when the specified object instance is
        /// not of the specified type.
        /// </summary>
        /// <typeparam name="TType">The Type that the <paramref name="instance"/> is expected to be.</typeparam>
        /// <param name="instance">The object instance whose type is checked.</param>
        /// <param name="message">The message of the <see cref="InvalidOperationException"/> exception.</param>
        public static void TypeOf<TType> (object instance, string message)
        {
            if (!(instance is TType))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws an exception if an instance of an object is not equal to another object instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception to throw when the guard check evaluates false.</typeparam>
        /// <param name="compare">The comparison object.</param>
        /// <param name="instance">The object instance to compare with.</param>
        /// <param name="message">string. The message of the exception.</param>
        public static void IsEqual<TException>(object compare, object instance, string message) where TException : Exception
        {
            if (compare != instance)
                throw (TException) Activator.CreateInstance(typeof (TException), message);
        }
    }
}