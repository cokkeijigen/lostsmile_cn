using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AssetFileDummyOnLoadError
	{
		public bool isEnable;

		public bool outputErrorLog = true;

		public Texture2D texture;

		public AudioClip sound;

		public TextAsset text;

		public UnityEngine.Object asset;
	}
}
