using System;
using System.Collections;
using System.Collections.Generic;

namespace NCommon.Collections
{
	public class ListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
	{
		readonly Dictionary<TKey, List<TValue>> innerValues = new Dictionary<TKey, List<TValue>>();

		public int Count
		{
			get { return innerValues.Count; }
		}

		public List<TValue> this[TKey key]
		{
			get
			{
				if (innerValues.ContainsKey(key) == false)
					innerValues.Add(key, new List<TValue>());

				return innerValues[key];
			}
			set { innerValues[key] = value; }
		}

		public ICollection<TKey> Keys
		{
			get { return innerValues.Keys; }
		}

		public List<TValue> Values
		{
			get
			{
				List<TValue> values = new List<TValue>();

				foreach (List<TValue> list in innerValues.Values)
					values.AddRange(list);

				return values;
			}
		}

		public void Add(TKey key)
		{
			Guard.IsNotNull(key, "key");

			CreateNewList(key);
		}

		public void Add(TKey key,
						TValue value)
		{
			Guard.IsNotNull(key, "key");
			Guard.IsNotNull(value, "value");

			if (innerValues.ContainsKey(key))
				innerValues[key].Add(value);
			else
			{
				List<TValue> values = CreateNewList(key);
				values.Add(value);
			}
		}

		public void Clear()
		{
			innerValues.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			Guard.IsNotNull(key, "key");

			return innerValues.ContainsKey(key);
		}

		public bool ContainsValue(TValue value)
		{
			foreach (KeyValuePair<TKey, List<TValue>> pair in innerValues)
				if (pair.Value.Contains(value))
					return true;

			return false;
		}

		List<TValue> CreateNewList(TKey key)
		{
			List<TValue> values = new List<TValue>();
			innerValues.Add(key, values);
			return values;
		}

		public IEnumerable<TValue> FindByKey(Predicate<TKey> keyFilter)
		{
			foreach (KeyValuePair<TKey, List<TValue>> pair in this)
				if (keyFilter(pair.Key))
					foreach (TValue value in pair.Value)
						yield return value;
		}

		public IEnumerable<TValue> FindByKeyAndValue(Predicate<TKey> keyFilter,
													 Predicate<TValue> valueFilter)
		{
			foreach (KeyValuePair<TKey, List<TValue>> pair in this)
				if (keyFilter(pair.Key))
					foreach (TValue value in pair.Value)
						if (valueFilter(value))
							yield return value;
		}

		public IEnumerable<TValue> FindByValue(Predicate<TValue> valueFilter)
		{
			foreach (KeyValuePair<TKey, List<TValue>> pair in this)
				foreach (TValue value in pair.Value)
					if (valueFilter(value))
						yield return value;
		}

		public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
		{
			return innerValues.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return innerValues.GetEnumerator();
		}

		public bool Remove(TKey key)
		{
			Guard.IsNotNull(key, "key");

			return innerValues.Remove(key);
		}

		public void Remove(TKey key,
						   TValue value)
		{
			Guard.IsNotNull(key, "key");
			Guard.IsNotNull(value, "value");

			if (innerValues.ContainsKey(key))
				innerValues[key].RemoveAll(delegate (TValue item)
				{
					return value.Equals(item);
				});
		}

		public void Remove(TValue value)
		{
			foreach (KeyValuePair<TKey, List<TValue>> pair in innerValues)
				Remove(pair.Key, value);
		}
	}
}