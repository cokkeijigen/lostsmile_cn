using UnityEngine;

namespace Utage
{
	public class AssetBundleHelper
	{
		public static AssetBundleTargetFlags RuntimeAssetBundleTarget()
		{
			return RuntimePlatformToBuildTargetFlag(Application.platform);
		}

		public static AssetBundleTargetFlags RuntimePlatformToBuildTargetFlag(RuntimePlatform platform)
		{
			switch (platform)
			{
			case RuntimePlatform.Android:
				return AssetBundleTargetFlags.Android;
			case RuntimePlatform.IPhonePlayer:
				return AssetBundleTargetFlags.iOS;
			case RuntimePlatform.WebGLPlayer:
				return AssetBundleTargetFlags.WebGL;
			case RuntimePlatform.WindowsPlayer:
				return AssetBundleTargetFlags.Windows;
			case RuntimePlatform.OSXPlayer:
				return AssetBundleTargetFlags.OSX;
			default:
				Debug.LogError("Not support " + platform);
				return (AssetBundleTargetFlags)0;
			}
		}
	}
}
