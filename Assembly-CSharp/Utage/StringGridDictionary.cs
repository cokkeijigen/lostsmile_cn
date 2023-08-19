using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utage
{
	[Serializable]
	public class StringGridDictionary : SerializableDictionary<StringGridDictionaryKeyValue>
	{
		public void Add(string key, StringGrid value)
		{
			Add(new StringGridDictionaryKeyValue(key, value));
		}

		public void RemoveSheets(string pattern)
		{
			List<string> list = new List<string>();
			foreach (string key in base.Keys)
			{
				if (Regex.IsMatch(key, pattern))
				{
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				Remove(item);
			}
		}
	}
}
