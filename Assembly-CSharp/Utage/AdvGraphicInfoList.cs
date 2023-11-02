using System;
using System.Collections.Generic;

namespace Utage
{
	public class AdvGraphicInfoList
	{
		private List<AdvGraphicInfo> infoList = new List<AdvGraphicInfo>();

		public string Key { get; protected set; }

		public List<AdvGraphicInfo> InfoList => infoList;

		public AdvGraphicInfo Main
		{
			get
			{
				if (InfoList.Count == 0)
				{
					return null;
				}
				if (InfoList.Count == 1)
				{
					return InfoList[0];
				}
				AdvGraphicInfo advGraphicInfo = null;
				foreach (AdvGraphicInfo info in InfoList)
				{
					if (string.IsNullOrEmpty(info.ConditionalExpression))
					{
						if (advGraphicInfo == null)
						{
							advGraphicInfo = info;
						}
					}
					else if (info.CheckConditionalExpression)
					{
						return info;
					}
				}
				return advGraphicInfo;
			}
		}

		public bool IsLoadEnd
		{
			get
			{
				foreach (AdvGraphicInfo info in infoList)
				{
					if (!info.File.IsLoadEnd)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsDefaultFileType
		{
			get
			{
				foreach (AdvGraphicInfo info in infoList)
				{
					if (!string.IsNullOrEmpty(info.FileType))
					{
						return false;
					}
				}
				return true;
			}
		}

		public AdvGraphicInfoList(string key)
		{
			Key = key;
		}

		internal void Add(string dataType, StringGridRow row, IAdvSettingData settingData)
		{
			infoList.Add(new AdvGraphicInfo(dataType, InfoList.Count, Key, row, settingData));
		}

		internal void BootInit(Func<string, string, string> FileNameToPath, AdvSettingDataManager dataManager)
		{
			foreach (AdvGraphicInfo info in infoList)
			{
				info.BootInit(FileNameToPath, dataManager);
			}
		}

		internal void DownloadAll()
		{
			foreach (AdvGraphicInfo info in infoList)
			{
				AssetFileManager.Download(info.File);
			}
		}
	}
}
