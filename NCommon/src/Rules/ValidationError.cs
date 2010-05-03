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

namespace NCommon.Rules
{
    /// <summary>
    /// Represents a validation error from a <see cref="IEntityValidator{TEntity}.Validate"/> method
    /// call.
    /// </summary>
    public class ValidationError
    {
		///<summary>
		/// The message that describes this validation error.
		///</summary>
		public string Message { get; set; }

		///<summary>
		/// The property that this validation error is associated with.
		///</summary>
		public string Property { get; set;}

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ValidationError"/> data structure.
        /// </summary>
        /// <param name="message">string. The validation error message.</param>
        /// <param name="property">string. The property that was validated.</param>
        public ValidationError(string message, string property)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(message),
                                                 "Please provide a valid non null string as the validation error message");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(property),
                                                 "Please provide a valid non null string as the validation property name");
            Message = message;
            Property = property;
        }

        /// <summary>
        /// Overriden. Gets a string that represents the validation error.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}) - {1}", Property, Message);
        }

        /// <summary>
        /// Overridden. Compares if an object is equal to the <see cref="ValidationError"/> instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (ValidationError)) return false;
            return Equals((ValidationError) obj);
        }

        /// <summary>
        /// Overriden. Compares if a <see cref="ValidationError"/> instance is equal to this
        /// <see cref="ValidationError"/> instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ValidationError obj)
        {
            return Equals(obj.Message, Message) && Equals(obj.Property, Property);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Message.GetHashCode() * 397) ^ Property.GetHashCode();
            }
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ValidationError left, ValidationError right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ValidationError left, ValidationError right)
        {
            return !left.Equals(right);
        }
    }
}