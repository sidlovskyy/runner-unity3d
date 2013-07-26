using System;
using System.Collections.Generic;

namespace Assets.Scripts.Auxilary
{
	//from: https://github.com/noocyte/Util.DoubleKeyDictionary
	public class DoubleKeyDictionary<K, T, V> : IEnumerable<DoubleKeyPairValue<K, T, V>>, IEquatable<DoubleKeyDictionary<K, T, V>>
	{
		public DoubleKeyDictionary()
		{
			OuterDictionary = new Dictionary<K, Dictionary<T, V>>();
		}

		private Dictionary<K, Dictionary<T, V>> OuterDictionary { get; set; }

		public void Add(K key1, T key2, V value)
		{
			if (OuterDictionary.ContainsKey(key1))
			{
				if (OuterDictionary[key1].ContainsKey(key2))
				{
					OuterDictionary[key1][key2] = value;
				}
				else
				{
					Dictionary<T, V> innerDictionary = OuterDictionary[key1];
					innerDictionary.Add(key2, value);
					OuterDictionary[key1] = innerDictionary;
				}
			}
			else
			{
				Dictionary<T, V> innerDictionary = new Dictionary<T, V>();
				innerDictionary[key2] = value;
				OuterDictionary.Add(key1, innerDictionary);
			}
		}

		public V this[K index1, T index2]
		{
			get
			{
				return OuterDictionary[index1][index2];
			}
			set
			{
				Add(index1, index2, value);
			}
		}

		public bool ContainsKey(K key1, T key2)
		{
			bool containsKey = false;
			if (OuterDictionary.ContainsKey(key1))
			{
				if (OuterDictionary[key1].ContainsKey(key2))
				{
					containsKey = true;
				}
			}
			return containsKey;
		}

		#region IEnumerable<DoubleKeyPairValue<K,T,V>> Members

		public IEnumerator<DoubleKeyPairValue<K, T, V>> GetEnumerator()
		{
			foreach (KeyValuePair<K, Dictionary<T, V>> outer in OuterDictionary)
			{
				foreach (KeyValuePair<T, V> inner in outer.Value)
				{
					yield return new DoubleKeyPairValue<K, T, V>(outer.Key, inner.Key, inner.Value);
				}
			}

		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEquatable<DoubleKeyDictionary<K,T,V>> Members

		public bool Equals(DoubleKeyDictionary<K, T, V> other)
		{
			if (OuterDictionary.Keys.Count != other.OuterDictionary.Keys.Count)
			{
				return false;
			}

			bool isEqual = true;

			foreach (KeyValuePair<K, Dictionary<T, V>> innerItems in OuterDictionary)
			{
				if (!other.OuterDictionary.ContainsKey(innerItems.Key))
				{
					isEqual = false;
				}

				if (!isEqual)
				{
					break;
				}

				// here we can be sure that the key is in both lists, 
				// but we need to check the contents of the inner dictionary
				Dictionary<T, V> otherInnerDictionary = other.OuterDictionary[innerItems.Key];
				isEqual = CheckInnerDictionary(innerItems, otherInnerDictionary);

				if (!isEqual)
				{
					break;
				}
			}

			return isEqual;
		}

		private static bool CheckInnerDictionary(KeyValuePair<K, Dictionary<T, V>> innerItems, Dictionary<T, V> otherInnerDictionary)
		{
			bool isEqual = true;
			foreach (KeyValuePair<T, V> innerValue in innerItems.Value)
			{
				if (!otherInnerDictionary.ContainsValue(innerValue.Value))
				{
					isEqual = false;
				}
				if (!otherInnerDictionary.ContainsKey(innerValue.Key))
				{
					isEqual = false;
				}
			}
			return isEqual;
		}

		#endregion
	}
}
