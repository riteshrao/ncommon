

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Text;
using System.Linq;

namespace NCommon.Extensions
{
    /// <summary>
    /// Contains some usefull extensions for working will collections.
    /// </summary>
    public static class CollectionExtensions
    {
        public static readonly char CommaDelimiter = ',';

        /// <summary>
        /// Returns a comma-delimited string from an <c>IList</c>
        /// </summary>
        /// <param name="source">The list of elements to create delimited string from</param>
        /// <returns>The string consisting of comma-separated elements (using the ToString() method) from the input list</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        public static string GetCommaDelimitedString<T>(this IEnumerable<T> source)
        {
            return source.GetDelimitedString(CommaDelimiter);
        }

        /// <summary>
        /// Returns a comma-delimited string containing the data from each element in the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="source">The list of elements whose data will be concatenated.</param>
        /// <param name="funcToGetString">The delegate to return data to be extracted from each element</param>
        /// <returns>comma-delimited string</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        public static string GetCommaDelimitedString<T>(this IEnumerable<T> source,
                                                        Func<T, string> funcToGetString)
        {
            return source.GetDelimitedString(funcToGetString, CommaDelimiter, false, true);
        }


        /// <summary>
        /// Returns a string of elements separated by a user-specified delimiter character
        /// </summary>
        /// <param name="delimiter">The specified delimiter character to be used to separate integers</param>
        /// <param name="source">The list of elements to create delimited string from</param>
        /// <returns>The string consisting of delimiter-separated elements (using the ToString() method) from the input list</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        public static string GetDelimitedString<T>(this IEnumerable<T> source, char delimiter)
        {
            return source.GetDelimitedString(t => t.ToString(), delimiter, false, true);
        }

        /// <summary>
        /// Returns a string of elements separated by a user-specified delimiter character
        /// </summary>
        /// <param name="source">The list of elements to create delimited string from</param>
        /// <param name="funcToGetString">The delegate to return data to be extracted from each element</param>
        /// <param name="delimiter">The specified delimiter character to be used to separate integers</param>
        /// <param name="addLeadingDelimiter">A flag indicating if the trailing delimiter should be removed</param>
        /// <param name="removeTrailingDelimiter">A flag indicating if the trailing delimiter should be removed</param>
        /// <returns>The string consisting of delimiter-separated elements (using the ToString() method) from the input list</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="funcToGetString"/> is null</exception>
        public static string GetDelimitedString<T>(this IEnumerable<T> source,
          Func<T, string> funcToGetString, char delimiter,
          bool addLeadingDelimiter, bool removeTrailingDelimiter)
        {
            Guard.IsNotNull(source, "source");
            Guard.IsNotNull(funcToGetString, "funcToGetString");
            if (source.Count() == 0) return null;

            StringBuilder sbuf = source.Aggregate(new StringBuilder(),
              (soFar, item) => soFar.Append(funcToGetString(item)).Append(delimiter));

            if (addLeadingDelimiter) sbuf.Insert(0, delimiter);
            if (removeTrailingDelimiter) sbuf.Remove(sbuf.Length - 1, 1);

            return sbuf.ToString();
        }


        /// <summary>
        /// Copies an Ilist. Doing this rather than implementing ICloneable in a bunch of different places.
        /// If this doesn't get refactored away it should go to Utilities, perhaps?
        /// GRE 3/31/2008
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<TType> Copy<TType>(this IList<TType> list)
        {
            if (list == null) return new List<TType>();

            return new List<TType>(list);
        }

        /// <summary>
        /// Returns a IList that contains no duplicated objects.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionWithPossibleDuplicates"></param>
        /// <param name="retainOriginalOrder">A boolean indicating if the original order in the collectionWithPossibleDuplicates should be retained</param>
        /// <returns></returns>
        public static IList<T> ConvertToListWithNoDuplicates<T>(
          this IEnumerable<T> collectionWithPossibleDuplicates, bool retainOriginalOrder)
        {
            if (collectionWithPossibleDuplicates == null) return null;

            if (collectionWithPossibleDuplicates is ICollection<T> &&
                ((ICollection<T>)collectionWithPossibleDuplicates).Count <= 1)
            {
                return new List<T>(collectionWithPossibleDuplicates);
            }

            // remove duplicates using a Set

            if (!retainOriginalOrder)
            {
                HashSet<T> set = new HashSet<T>(collectionWithPossibleDuplicates);
                return new List<T>(set);
            }
            else
            {
                // retain the original order in the collectionWithPossibleDuplicates
                IList<T> rv = new List<T>();
                HashSet<T> set = new HashSet<T>();

                foreach (T t in collectionWithPossibleDuplicates)
                {
                    if (!set.Contains(t))
                    {
                        set.Add(t);
                        rv.Add(t);
                    }
                }

                return rv;
            }
        }

        /// <summary>
        /// Accepts a list of objects and converts them to a strongly typed list.
        /// </summary>
        /// <remarks>This should be put into a utility class as it's not directly
        /// related to data access.</remarks>
        public static IList<T> ConvertToList<T>(this System.Collections.IEnumerable objects)
        {
            if (objects == null) return new List<T>();

            return objects.Cast<T>().ToList();
        }

        /// <summary>
        /// Converts a generic collection into a generic array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] ConvertToArray<T>(this IEnumerable<T> source)
        {
            if (source == null) return null;

            return source.ToArray();
        }
        
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

        

        private static bool ForEachHelper()
        {
            return true;
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

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return (collection == null) || (collection.Count == 0);
        }


        public delegate bool Decide<T>(T item);

        /// <summary>
        /// This allows you to safely remove an item from the collection you are enumerating without worrying about
        /// running into an InvalidOperation exception due to the target collection being modified.
        /// </summary>
        /// <typeparam name="T">The collection type you're working with</typeparam>
        /// <param name="collection">The collection your working with</param>
        /// <param name="decide">A delegate which ultimately decides if you want to keep the item in the collection or not.</param>
        public static void RemoveItems<T>(this ICollection<T> collection, Decide<T> decide)
        {

            List<T> removed = new List<T>();

            foreach (T item in collection)
            {

                if (decide(item))

                    removed.Add(item);

            }

            foreach (T item in removed)
            {

                collection.Remove(item);

            }

            removed.Clear();

        }

        /// <summary>
        /// This allows you to safely remove an item from the collection you are enumerating without worrying about
        /// running into an InvalidOperation exception due to the target collection being modified.
        /// </summary>
        /// <typeparam name="T">The collection type you're working with</typeparam>
        /// <param name="collection">The collection your working with</param>
        /// <param name="action">The action you want to execute on an item in the collection prior to it being removed from the collection.</param>
        /// <param name="decide">A delegate which ultimately decides if you want to keep the item in the collection or not.</param>
        public static void RemoveItems<T>(this ICollection<T> collection, Action<T> action, Decide<T> decide)
        {

            List<T> removed = new List<T>();

            foreach (T item in collection)
            {
                action(item);
                if (decide(item))

                    removed.Add(item);

            }

            foreach (T item in removed)
            {

                collection.Remove(item);

            }

            removed.Clear();

        }

        public static DataTable ToDataTable(this IList alist)
        {
            DataTable dt = new DataTable();

            if (alist == null)
            {
                throw new FormatException("Parameter ArrayList empty");
            }
            dt.TableName = alist[0].GetType().Name;
            DataRow dr;
            System.Reflection.PropertyInfo[] propInfo = alist[0].GetType().GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                dt.Columns.Add(propInfo[i].Name, propInfo[i].PropertyType);
            }

            for (int row = 0; row < alist.Count; row++)
            {
                dr = dt.NewRow();
                for (int i = 0; i < propInfo.Length; i++)
                {
                    object tempObject = alist[row];

                    object t = propInfo[i].GetValue(tempObject, null);
                    /*object t =tempObject.GetType().InvokeMember(propInfo[i].Name,
                             R.BindingFlags.GetProperty , null,tempObject , new object [] {});*/
                    if (t != null)
                        dr[i] = t.ToString();
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static DataTable ToDataTable(this IList alist, ArrayList alColNames)
        {
            DataTable dt = new DataTable();

            if (alist == null)
            {
                throw new FormatException("Parameter ArrayList empty");
            }
            dt.TableName = alist[0].GetType().Name;
            DataRow dr;
            System.Reflection.PropertyInfo[] propInfo = alist[0].GetType().GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                for (int j = 0; j < alColNames.Count; j++)
                {
                    if (alColNames[j].ToString() == propInfo[i].Name)
                    {
                        dt.Columns.Add(propInfo[i].Name, propInfo[i].PropertyType);
                        break;
                    }
                }
            }

            for (int row = 0; row < alist.Count; row++)
            {
                dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    object tempObject = alist[row];

                    object t = propInfo[i].GetValue(tempObject, null);
                    /*object t =tempObject.GetType().InvokeMember(propInfo[i].Name,
                             R.BindingFlags.GetProperty , null,tempObject , new object [] {});*/
                    if (t != null)
                        dr[i] = t.ToString();
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        
    }
}