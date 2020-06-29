using System;
using System.Diagnostics;
using System.Collections.Generic;
using NCommon.Extensions;

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
                throw (TException)Activator.CreateInstance(typeof(TException), message);
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
                throw (TException)Activator.CreateInstance(typeof(TException), message);
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
            if (type.BaseType != typeof(TBase))
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
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when the specified object instance is
        /// not of the specified type.
        /// </summary>
        /// <typeparam name="TType">The Type that the <paramref name="instance"/> is expected to be.</typeparam>
        /// <param name="instance">The object instance whose type is checked.</param>
        /// <param name="message">The message of the <see cref="InvalidOperationException"/> exception.</param>
        public static void TypeOf<TType>(object instance, string message)
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
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        [DebuggerStepThrough]
        public static void IsNotEmpty(Guid argument, string argumentName)
        {
            IsNotEmpty(argument, argumentName, true);
        }

        [DebuggerStepThrough]
        public static bool IsNotEmpty(Guid argument, string argumentName, bool throwException)
        {
            if (argument == Guid.Empty)
            {
                if (throwException)
                {
                    throw new ArgumentException("\"{0}\" cannot be empty guid.".FormatWith(argumentName), argumentName);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        [DebuggerStepThrough]
        public static void IsNotEmpty(string argument, string argumentName)
        {
            IsNotEmpty(argument, argumentName, true);
        }

        [DebuggerStepThrough]
        public static bool IsNotEmpty(string argument, string argumentName, bool throwException)
        {
            if (string.IsNullOrEmpty((argument ?? string.Empty).Trim()))
            {
                if (throwException)
                {
                    throw new ArgumentException("\"{0}\" cannot be blank.".FormatWith(argumentName), argumentName);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        [DebuggerStepThrough]
        public static void IsNotOutOfLength(string argument, int length, string argumentName)
        {
            if (argument.Trim().Length > length)
            {
                throw new ArgumentException("\"{0}\" cannot be more than {1} character.".FormatWith(argumentName, length), argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegative(int argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegativeOrZero(int argument, string argumentName)
        {
            IsNotNegativeOrZero(argument, argumentName, true);
        }

        public static bool IsNotNegativeOrZero(int argument, string argumentName, bool throwException)
        {
            if (argument <= 0)
            {
                if (throwException)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegative(long argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegativeOrZero(long argument, string argumentName)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegative(float argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegativeOrZero(float argument, string argumentName)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegative(decimal argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegativeOrZero(decimal argument, string argumentName)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotInvalidDate(DateTime argument, string argumentName)
        {
            if (!argument.IsValid())
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotInPast(DateTime argument, string argumentName)
        {
            if (argument < SystemTime.Now())
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotInFuture(DateTime argument, string argumentName)
        {
            if (argument > SystemTime.Now())
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegative(TimeSpan argument, string argumentName)
        {
            if (argument < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNegativeOrZero(TimeSpan argument, string argumentName)
        {
            if (argument <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotEmpty<T>(ICollection<T> argument, string argumentName)
        {
            IsNotNull(argument, argumentName);

            if (argument.Count == 0)
            {
                throw new ArgumentException("Collection cannot be empty.", argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotOutOfRange(int argument, int min, int max, string argumentName)
        {
            if ((argument < min) || (argument > max))
            {
                throw new ArgumentOutOfRangeException(argumentName, "{0} must be between \"{1}\"-\"{2}\".".FormatWith(argumentName, min, max));
            }
        }

        [DebuggerStepThrough]
        public static void IsNotInvalidEmail(string argument, string argumentName)
        {
            IsNotEmpty(argument, argumentName);

            if (!argument.IsEmail())
            {
                throw new ArgumentException("\"{0}\" is not a valid email address.".FormatWith(argumentName), argumentName);
            }
        }

        [DebuggerStepThrough]
        public static void IsNotInvalidWebUrl(string argument, string argumentName)
        {
            IsNotEmpty(argument, argumentName);

            if (!argument.IsWebUrl())
            {
                throw new ArgumentException("\"{0}\" is not a valid web url.".FormatWith(argumentName), argumentName);
            }
        }
    }
}