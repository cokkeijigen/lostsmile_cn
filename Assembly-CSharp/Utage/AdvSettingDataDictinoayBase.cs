using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public abstract class AdvSettingDataDictinoayBase<T> : AdvSettingBase where T : AdvSettingDictinoayItemBase, new()
	{
		public List<T> List { get; private set; }

		public Dictionary<string, T> Dictionary { get; private set; }

		public AdvSettingDataDictinoayBase()
		{
			Dictionary = new Dictionary<string, T>();
			List = new List<T>();
		}

		protected override void OnParseGrid(StringGrid grid)
		{
			T last = null;
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex >= grid.DataTopRow && !row.IsEmptyOrCommantOut && !TryParseContinues(last, row))
				{
					T val = ParseFromStringGridRow(last, row);
					if (val != null)
					{
						last = val;
					}
				}
			}
		}

		protected virtual bool TryParseContinues(T last, StringGridRow row)
		{
			_ = last;
			return false;
		}

		protected virtual T ParseFromStringGridRow(T last, StringGridRow row)
		{
			T val = new T();
			if (val.InitFromStringGridRowMain(row))
			{
				if (!Dictionary.ContainsKey(val.Key))
				{
					AddData(val);
					return val;
				}
				Debug.LogError("" + row.ToErrorString(ColorUtil.AddColorTag(val.Key, Color.red) + "  is already contains"));
			}
			return null;
		}

		protected void AddData(T data)
		{
			List.Add(data);
			Dictionary.Add(data.Key, data);
		}
	}
}
