using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvReadHistorySaveData : IBinaryIO
	{
		protected DictionaryInt data = new DictionaryInt();

		private const int VERSION = 0;

		public DictionaryInt Data => data;

		public string SaveKey => "AdvReadHistorySaveData";

		public void AddReadPage(string scenarioLabel, int page)
		{
			if (data.TryGetValue(scenarioLabel, out DictionaryKeyValueInt val))
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
			if (data.TryGetValue(scenarioLabel, out DictionaryKeyValueInt val) && pageNo <= val.value)
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
