using System.Collections.Generic;

namespace Utage
{
	public class AdvSoundSetting : AdvSettingDataDictinoayBase<AdvSoundSettingData>
	{
		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvSoundSettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvSoundSettingData item in base.List)
			{
				AssetFileManager.Download(item.FilePath);
			}
		}

		public bool Contains(string label, SoundType type)
		{
			if (FindData(label) == null)
			{
				return false;
			}
			return true;
		}

		public string LabelToFilePath(string label, SoundType type)
		{
			if (FilePathUtil.IsAbsoluteUri(label))
			{
				return ExtensionUtil.ChangeSoundExt(label);
			}
			AdvSoundSettingData advSoundSettingData = FindData(label);
			if (advSoundSettingData == null)
			{
				return label;
			}
			return advSoundSettingData.FilePath;
		}

		public AdvSoundSettingData FindData(string label)
		{
			if (!base.Dictionary.TryGetValue(label, out var value))
			{
				return null;
			}
			return value;
		}

		public StringGridRow FindRowData(string label)
		{
			return FindData(label)?.RowData;
		}

		public List<AdvSoundSettingData> GetSoundRoomList()
		{
			List<AdvSoundSettingData> list = new List<AdvSoundSettingData>();
			foreach (AdvSoundSettingData item in base.List)
			{
				if (!string.IsNullOrEmpty(item.Title))
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
}
