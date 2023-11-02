using UnityEngine;

namespace Utage
{
	public class AdvEyeBlinkData : AdvSettingDictinoayItemBase
	{
		[SerializeField]
		private float intervalMin = 2f;

		[SerializeField]
		private float intervalMax = 6f;

		[SerializeField]
		private float randomDoubleEyeBlink = 0.2f;

		[SerializeField]
		private string tag = "eye";

		[SerializeField]
		private MiniAnimationData animationData = new MiniAnimationData();

		public float IntervalMin
		{
			get
			{
				return intervalMin;
			}
			set
			{
				intervalMin = value;
			}
		}

		public float IntervalMax
		{
			get
			{
				return intervalMax;
			}
			set
			{
				intervalMax = value;
			}
		}

		public float RandomDoubleEyeBlink
		{
			get
			{
				return randomDoubleEyeBlink;
			}
			set
			{
				randomDoubleEyeBlink = value;
			}
		}

		public string Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public MiniAnimationData AnimationData => animationData;

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			string text = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.Label);
			InitKey(text);
			IntervalMin = AdvParser.ParseCellOptional(row, AdvColumnName.IntervalMin, 2f);
			IntervalMax = AdvParser.ParseCellOptional(row, AdvColumnName.IntervalMax, 6f);
			RandomDoubleEyeBlink = AdvParser.ParseCellOptional(row, AdvColumnName.RandomDouble, 0.2f);
			Tag = AdvParser.ParseCellOptional(row, AdvColumnName.Tag, "eye");
			if (row.Grid.TryGetColumnIndex(AdvColumnName.Name0.QuickToString(), out var index))
			{
				animationData.TryParse(row, index);
			}
			return true;
		}
	}
}
