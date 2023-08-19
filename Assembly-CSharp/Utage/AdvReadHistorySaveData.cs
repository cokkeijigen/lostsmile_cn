using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvReadHistorySaveData : IBinaryIO
	{
		protected DictionaryInt data = new DictionaryInt();

		private const int VERSION = 0;

		public DictionaryInt Data
		{
			get
			{
				return data;
			}
		}

		public string SaveKey
		{
			get
			{
				return "AdvReadHistorySaveData";
			}
		}

		public void AddReadPage(string scenarioLabel, int page)
		{
			DictionaryKeyValueInt val;
			if (data.TryGetValue(scenarioLabel, out val))
			{
				if (val.value < page)
				{
					val.value = page;
				}
			}
			else
			{
				data.Add(new DictionaryKeyValueInt(scenarioLabel, page));
			}
		}

		public bool CheckReadPage(string scenarioLabel, int pageNo)
		{
			DictionaryKeyValueInt val;
			if (data.TryGetValue(scenarioLabel, out val) && pageNo <= val.value)
			{
				return true;
			}
			return false;
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				data.Read(reader);
				return;
			}
			Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			data.Write(writer);
		}
	}
}
