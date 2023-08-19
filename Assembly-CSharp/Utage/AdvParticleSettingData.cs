namespace Utage
{
	public class AdvParticleSettingData : AdvSettingDictinoayItemBase
	{
		private AdvGraphicInfo graphic;

		public AdvGraphicInfo Graphic
		{
			get
			{
				return graphic;
			}
		}

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			if (row.IsEmptyOrCommantOut)
			{
				return false;
			}
			base.RowData = row;
			string value = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			InitKey(value);
			graphic = new AdvGraphicInfo("Particle", 0, value, row, this);
			return true;
		}

		public void BootInit(AdvSettingDataManager dataManager)
		{
			Graphic.BootInit((string fileName, string fileType) => FileNameToPath(fileName, fileType, dataManager.BootSetting), dataManager);
		}

		private string FileNameToPath(string fileName, string fileType, AdvBootSetting settingData)
		{
			return settingData.ParticleDirInfo.FileNameToPath(fileName);
		}
	}
}
