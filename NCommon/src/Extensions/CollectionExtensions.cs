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

namespace NCommon.Extensions
{
    /// <summary>
    /// Contains some usefull extensions for working will collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// ForEach extension that enumerates over all items in an <see cref="IEnumerable{T}"/> and executes 
        /// an action.
        /// </summary>
        /// <typeparam name="T">The type that this extension is applicable for.</typeparam>
        /// <param name="collection">The enumerable instance that this extension operates on.</param>
        /// <param name="action">The action executed for each iten in the enumerable.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        /// <summary>
        /// ForEach extension that enumerates over all items in an <see cref="IEnumerator{T}"/> and executes 
        /// an action.
        /// </summary>
        /// <typeparam name="T">The type that this extension is applicable for.</typeparam>
        /// <param name="collection">The enumerator instance that this extension operates on.</param>
        /// <param name="action">The action executed for each iten in the enumerable.</param>
        public static void ForEach<T>(this IEnumerator<T> collection, Action<T> action)
        {
            while (collection.MoveNext())
                action(collection.Current);
        }
		
		/// <summary>
		/// For Each extension that enumerates over a enumerable collection and attempts to execute 
		/// the provided action delegate and it the action throws an exception, continues enumerating.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="collection">The IEnumerable instance that ths extension operates on.</param>
		/// <param name="action">The action excecuted for each item in the enumerable.</param>
		public static void TryForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				try
				{
					action(item);
				}catch{}
			}
		}

		/// <summary>
		/// For each extension that enumerates over an enumerator and attempts to execute the provided
		/// action delegate and if the action throws an exception, continues executing.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="enumerator">The IEnumerator instace</param>
		/// <param name="action">The action executed for each item in the enumerator.</param>
		public static void TryForEach<T>(this IEnumerator<T> enumerator, Action<T> action)
		{
			while (enumerator.MoveNext())
			{
				try
				{
					action(enumerator.Current);
				}catch{}
			}
		}
    }
}