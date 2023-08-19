using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AvatarPattern
	{
		[Serializable]
		public class PartternData
		{
			public string tag;

			public string patternName;
		}

		[SerializeField]
		private List<PartternData> avatarPatternDataList = new List<PartternData>();

		[SerializeField]
		private List<string> optionPatternNameList = new List<string>();

		public List<PartternData> DataList
		{
			get
			{
				return avatarPatternDataList;
			}
		}

		public List<string> OptionPatternNameList
		{
			get
			{
				return optionPatternNameList;
			}
		}

		public void SetPatternName(string tag, string patternName)
		{
			PartternData partternData = DataList.Find((PartternData x) => x.tag == tag);
			if (partternData == null)
			{
				Debug.LogError(string.Format("Unknown Pattern [{0}], tag[{1}] ", patternName, tag));
			}
			else
			{
				partternData.patternName = patternName;
			}
		}

		public string GetPatternName(string tag)
		{
			PartternData partternData = DataList.Find((PartternData x) => x.tag == tag);
			if (partternData != null)
			{
				return partternData.patternName;
			}
			return "";
		}

		internal void SetPattern(StringGridRow rowData)
		{
			foreach (KeyValuePair<string, int> keyValue in rowData.Grid.ColumnIndexTbl)
			{
				PartternData partternData = DataList.Find((PartternData x) => x.tag == keyValue.Key);
				if (partternData != null)
				{
					partternData.patternName = rowData.Strings[keyValue.Value];
				}
			}
		}

		internal bool Rebuild(AvatarData data)
		{
			if (data == null)
			{
				return false;
			}
			bool result = false;
			foreach (AvatarData.Category category in data.categories)
			{
				PartternData partternData = DataList.Find((PartternData x) => x.tag == category.Tag);
				if (partternData == null)
				{
					partternData = new PartternData();
					partternData.tag = category.Tag;
					DataList.Add(partternData);
					result = true;
				}
			}
			return result;
		}
	}
}
