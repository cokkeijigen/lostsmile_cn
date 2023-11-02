using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class SerializableDictionary<T> where T : SerializableDictionaryKeyValue
	{
		[SerializeField]
		private List<T> list;

		protected Dictionary<string, T> dictionary = new Dictionary<string, T>();

		public List<T> List => list ?? (list = new List<T>());

		public int Count
		{
			get
			{
				InitDic();
				return dictionary.Count;
			}
		}

		public Dictionary<string, T>.KeyCollection Keys
		{
			get
			{
				InitDic();
				return dictionary.Keys;
			}
		}

		public Dictionary<string, T>.ValueCollection Values
		{
			get
			{
				InitDic();
				return dictionary.Values;
			}
		}

		public void Add(T val)
		{
			if (dictionary.ContainsKey(val.Key))
			{
				Debug.LogError("<color=red>" + val.Key + "</color>  is already contains");
			}
			InitDic();
			dictionary.Add(val.Key, val);
			List.Add(val);
		}

		public T GetValue(string key)
		{
			InitDic();
			return dictionary[key];
		}

		public bool TryGetValue(string key, out T val)
		{
			InitDic();
			return dictionary.TryGetValue(key, out val);
		}

		public bool Remove(string key)
		{
			InitDic();
			bool num = dictionary.Remove(key);
			if (num)
			{
				List.RemoveAll((T x) => x.Key.CompareTo(key) == 0);
			}
			return num;
		}

		public void Clear()
		{
			dictionary.Clear();
			List.Clear();
		}

		public bool ContainsKey(string key)
		{
			InitDic();
			return dictionary.ContainsKey(key);
		}

		public bool ContainsValue(T val)
		{
			InitDic();
			return dictionary.ContainsValue(val);
		}

		private void InitDic()
		{
			if (dictionary.Count == 0)
			{
				RefreshDictionary();
			}
		}

		public void RefreshDictionary()
		{
			dictionary.Clear();
			foreach (T item in List)
			{
				dictionary.Add(item.Key, item);
			}
		}

		public void Swap(int index0, int index1)
		{
			if (index0 >= 0 && Count > index0 && index1 >= 0 && Count > index1)
			{
				T value = List[index0];
				List[index0] = List[index1];
				List[index1] = value;
				RefreshDictionary();
			}
		}
	}
}
