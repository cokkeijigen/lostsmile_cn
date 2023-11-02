using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class MiniAnimationData
	{
		[Serializable]
		public class Data
		{
			private enum NamingType
			{
				Default,
				Suffix,
				Swap
			}

			[SerializeField]
			private float duration;

			[SerializeField]
			private string name;

			public float Duration => duration;

			public Data(float duration, string name)
			{
				this.duration = duration;
				this.name = name;
			}

			public string ComvertName(string originalName)
			{
				switch (ParseNamigType())
				{
				case NamingType.Suffix:
					return originalName + GetSffixName();
				case NamingType.Swap:
					return GetSwapName(originalName);
				default:
					return name;
				}
			}

			public string ComvertNameSimple()
			{
				NamingType namingType = ParseNamigType();
				if (namingType == NamingType.Suffix)
				{
					return GetSffixName();
				}
				return name;
			}

			public string GetSffixName()
			{
				return name.Substring(1);
			}

			public string GetSwapName(string originalName)
			{
				if (originalName.Length < 2)
				{
					return originalName;
				}
				int num = originalName.IndexOf('(');
				if (num < 0)
				{
					return originalName;
				}
				return originalName.Substring(0, num) + name.Substring(1);
			}

			private NamingType ParseNamigType()
			{
				if (name.Length < 2)
				{
					return NamingType.Default;
				}
				if (name[0] != '*')
				{
					return NamingType.Default;
				}
				if (name[1] == '(' && name[name.Length - 1] == ')')
				{
					return NamingType.Swap;
				}
				return NamingType.Suffix;
			}
		}

		[SerializeField]
		private List<Data> dataList = new List<Data>();

		public List<Data> DataList => dataList;

		internal bool TryParse(StringGridRow row, int index)
		{
			try
			{
				DataList.Clear();
				while (index + 1 < row.Strings.Length && (!row.IsEmptyCell(index) || !row.IsEmptyCell(index + 1)))
				{
					string name = row.ParseCell<string>(index++);
					float duration = row.ParseCell<float>(index++);
					DataList.Add(new Data(duration, name));
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
