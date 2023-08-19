using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AdvScenarioDataExported : ScriptableObject
	{
		[SerializeField]
		private StringGridDictionary dictionary;

		public List<StringGridDictionaryKeyValue> List
		{
			get
			{
				return dictionary.List;
			}
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public void ParseFromExcel(string sheetName, StringGrid grid)
		{
			dictionary.Add(sheetName, grid);
		}
	}
}
