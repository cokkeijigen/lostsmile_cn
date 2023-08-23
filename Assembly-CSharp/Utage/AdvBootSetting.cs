using System;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[Serializable]
	public class AdvBootSetting
	{
		[Serializable]
		public class DefaultDirInfo
		{
			public string defaultDir;

			public string defaultExt;

			public string FileNameToPath(string fileName)
			{
				return FileNameToPath(fileName, "");
			}

			public string FileNameToPath(string fileName, string LocalizeDir)
			{
				if (string.IsNullOrEmpty(fileName))
				{
					return fileName;
				}
                string path;
				if (FilePathUtil.IsAbsoluteUri(fileName))
				{
					path = fileName;
				}
				else
				{
					try
					{
						if (string.IsNullOrEmpty(FilePathUtil.GetExtension(fileName)))
						{
							fileName += defaultExt;
						}
						path = defaultDir + LocalizeDir + "/" + fileName;
					}
					catch (Exception ex)
					{
						Debug.LogError(fileName + "  " + ex.ToString());
						path = defaultDir + LocalizeDir + "/" + fileName;
					}
				}
				return ExtensionUtil.ChangeSoundExt(path);
			}
		}

		private DefaultDirInfo characterDirInfo;

		private DefaultDirInfo bgDirInfo;

		private DefaultDirInfo eventDirInfo;

		private DefaultDirInfo spriteDirInfo;

		private DefaultDirInfo thumbnailDirInfo;

		private DefaultDirInfo bgmDirInfo;

		private DefaultDirInfo seDirInfo;

		private DefaultDirInfo ambienceDirInfo;

		private DefaultDirInfo voiceDirInfo;

		private DefaultDirInfo particleDirInfo;

		private DefaultDirInfo videoDirInfo;

		public string ResourceDir { get; set; }

		public DefaultDirInfo CharacterDirInfo
		{
			get
			{
				return characterDirInfo;
			}
		}

		public DefaultDirInfo BgDirInfo
		{
			get
			{
				return bgDirInfo;
			}
		}

		public DefaultDirInfo EventDirInfo
		{
			get
			{
				return eventDirInfo;
			}
		}

		public DefaultDirInfo SpriteDirInfo
		{
			get
			{
				return spriteDirInfo;
			}
		}

		public DefaultDirInfo ThumbnailDirInfo
		{
			get
			{
				return thumbnailDirInfo;
			}
		}

		public DefaultDirInfo BgmDirInfo
		{
			get
			{
				return bgmDirInfo;
			}
		}

		public DefaultDirInfo SeDirInfo
		{
			get
			{
				return seDirInfo;
			}
		}

		public DefaultDirInfo AmbienceDirInfo
		{
			get
			{
				return ambienceDirInfo;
			}
		}

		public DefaultDirInfo VoiceDirInfo
		{
			get
			{
				return voiceDirInfo;
			}
		}

		public DefaultDirInfo ParticleDirInfo
		{
			get
			{
				return particleDirInfo;
			}
		}

		public DefaultDirInfo VideoDirInfo
		{
			get
			{
				return videoDirInfo;
			}
		}

		public void BootInit(string resourceDir)
		{
			ResourceDir = resourceDir;
			characterDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Character",
				defaultExt = ".png"
			};
			bgDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/BG",
				defaultExt = ".jpg"
			};
			eventDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Event",
				defaultExt = ".jpg"
			};
			spriteDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Sprite",
				defaultExt = ".png"
			};
			thumbnailDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Thumbnail",
				defaultExt = ".jpg"
			};
			bgmDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/BGM",
				defaultExt = ".wav"
			};
			seDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/SE",
				defaultExt = ".wav"
			};
			ambienceDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/Ambience",
				defaultExt = ".wav"
			};
			voiceDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/Voice",
				defaultExt = ".wav"
			};
			particleDirInfo = new DefaultDirInfo
			{
				defaultDir = "Particle",
				defaultExt = ".prefab"
			};
			videoDirInfo = new DefaultDirInfo
			{
				defaultDir = "Video",
				defaultExt = ".mp4"
			};
			InitDefaultDirInfo(ResourceDir, characterDirInfo);
			InitDefaultDirInfo(ResourceDir, bgDirInfo);
			InitDefaultDirInfo(ResourceDir, eventDirInfo);
			InitDefaultDirInfo(ResourceDir, spriteDirInfo);
			InitDefaultDirInfo(ResourceDir, thumbnailDirInfo);
			InitDefaultDirInfo(ResourceDir, bgmDirInfo);
			InitDefaultDirInfo(ResourceDir, seDirInfo);
			InitDefaultDirInfo(ResourceDir, ambienceDirInfo);
			InitDefaultDirInfo(ResourceDir, voiceDirInfo);
			InitDefaultDirInfo(ResourceDir, particleDirInfo);
			InitDefaultDirInfo(ResourceDir, videoDirInfo);
		}

		private void InitDefaultDirInfo(string root, DefaultDirInfo info)
		{
			info.defaultDir = FilePathUtil.Combine(root, info.defaultDir);
		}

		public string GetLocalizeVoiceFilePath(string file)
		{
			if (LanguageManagerBase.Instance.IgnoreLocalizeVoice)
			{
				return VoiceDirInfo.FileNameToPath(file);
			}
			string currentLanguage = LanguageManagerBase.Instance.CurrentLanguage;
			if (LanguageManagerBase.Instance.VoiceLanguages.Contains(currentLanguage))
			{
				return VoiceDirInfo.FileNameToPath(file, currentLanguage);
			}
			return VoiceDirInfo.FileNameToPath(file);
		}
	}
}
