using UnityEngine;

namespace Utage
{
	public interface AssetFile
	{
		string FileName { get; }

		IAssetFileSettingData SettingData { get; }

		AssetFileType FileType { get; }

		bool IsLoadEnd { get; }

		bool IsLoadError { get; }

		string LoadErrorMsg { get; }

		TextAsset Text { get; }

		Texture2D Texture { get; }

		AudioClip Sound { get; }

		Object UnityObject { get; }

		void Use(object obj);

		void Unuse(object obj);

		void AddReferenceComponent(GameObject go);
	}
}
