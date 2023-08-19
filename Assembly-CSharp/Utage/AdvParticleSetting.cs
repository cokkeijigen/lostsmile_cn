using UnityEngine;

namespace Utage
{
	public class AdvParticleSetting : AdvSettingDataDictinoayBase<AdvParticleSettingData>
	{
		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvParticleSettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvParticleSettingData item in base.List)
			{
				AssetFileManager.Download(item.Graphic.File);
			}
		}

		private AdvParticleSettingData FindData(string label)
		{
			AdvParticleSettingData value;
			if (!base.Dictionary.TryGetValue(label, out value))
			{
				return null;
			}
			return value;
		}

		public AdvGraphicInfo LabelToGraphic(string label)
		{
			AdvParticleSettingData advParticleSettingData = FindData(label);
			if (advParticleSettingData == null)
			{
				Debug.LogError("Not found " + label + " in Particle sheet");
				return null;
			}
			return advParticleSettingData.Graphic;
		}
	}
}
