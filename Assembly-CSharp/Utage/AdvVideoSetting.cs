using UnityEngine;

namespace Utage
{
	public class AdvVideoSetting : AdvSettingDataDictinoayBase<AdvVideoSettingData>
	{
		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvVideoSettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvVideoSettingData item in base.List)
			{
				AssetFileManager.Download(item.Graphic.File);
			}
		}

		private AdvVideoSettingData FindData(string label)
		{
			if (!base.Dictionary.TryGetValue(label, out var value))
			{
				return null;
			}
			return value;
		}

		public AdvGraphicInfo LabelToGraphic(string label)
		{
			AdvVideoSettingData advVideoSettingData = FindData(label);
			if (advVideoSettingData == null)
			{
				Debug.LogError("Not found " + label + " in Particle sheet");
				return null;
			}
			return advVideoSettingData.Graphic;
		}
	}
}
