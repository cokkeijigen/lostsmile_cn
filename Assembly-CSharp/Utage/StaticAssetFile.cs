using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	public class StaticAssetFile : AssetFileBase
	{
		public StaticAsset Asset { get; protected set; }

		public StaticAssetFile(StaticAsset asset, AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
			: base(mangager, fileInfo, settingData)
		{
			Asset = asset;
			base.Text = Asset.Asset as TextAsset;
			base.Texture = Asset.Asset as Texture2D;
			base.Sound = Asset.Asset as AudioClip;
			base.UnityObject = Asset.Asset;
			base.IsLoadEnd = true;
			base.IgnoreUnload = true;
			if (base.Texture != null)
			{
				FileType = AssetFileType.Texture;
			}
			else if (base.Sound != null)
			{
				FileType = AssetFileType.Sound;
			}
			else if (base.UnityObject != null)
			{
				FileType = AssetFileType.UnityObject;
			}
		}

		public override bool CheckCacheOrLocal()
		{
			return true;
		}

		public override IEnumerator LoadAsync(Action onComplete, Action onFailed)
		{
			onComplete();
			yield break;
		}

		public override void Unload()
		{
		}
	}
}
