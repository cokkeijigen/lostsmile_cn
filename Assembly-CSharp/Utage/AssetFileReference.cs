using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/AssetFileReference")]
	public class AssetFileReference : MonoBehaviour
	{
		private AssetFile file;

		public AssetFile File
		{
			get
			{
				return file;
			}
		}

		public void Init(AssetFile file)
		{
			this.file = file;
			this.file.Use(this);
		}

		private void OnDestroy()
		{
			file.Unuse(this);
		}
	}
}
