using UnityEngine;

namespace Utage
{
	public class AdvLipSynchData : AdvSettingDictinoayItemBase
	{
		[SerializeField]
		private LipSynchType type = LipSynchType.TextAndVoice;

		[SerializeField]
		private float interval = 0.2f;

		[SerializeField]
		private float scaleVoiceVolume = 1f;

		[SerializeField]
		private string tag = "eye";

		[SerializeField]
		private MiniAnimationData animationData = new MiniAnimationData();

		public LipSynchType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public float Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		public float ScaleVoiceVolume
		{
			get
			{
				return scaleVoiceVolume;
			}
			set
			{
				scaleVoiceVolume = value;
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
			Type = AdvParser.ParseCellOptional(row, AdvColumnName.Type, LipSynchType.TextAndVoice);
			Interval = AdvParser.ParseCellOptional(row, AdvColumnName.Interval, 0.2f);
			ScaleVoiceVolume = AdvParser.ParseCellOptional(row, AdvColumnName.ScaleVoiceVolume, 1);
			Tag = AdvParser.ParseCellOptional(row, AdvColumnName.Tag, "lip");
			if (row.Grid.TryGetColumnIndex(AdvColumnName.Name0.QuickToString(), out var index))
			{
				animationData.TryParse(row, index);
			}
			return true;
		}
	}
}
