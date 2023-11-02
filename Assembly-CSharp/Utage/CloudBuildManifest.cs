using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class CloudBuildManifest
	{
		private static CloudBuildManifest instance;

		[SerializeField]
		private string scmCommitId = "";

		[SerializeField]
		private string scmBranch = "";

		[SerializeField]
		private string buildNumber = "";

		[SerializeField]
		private string buildStartTime = "";

		[SerializeField]
		private string projectId = "";

		[SerializeField]
		private string bundleId = "";

		[SerializeField]
		private string unityVersion = "";

		[SerializeField]
		private string xcodeVersion = "";

		[SerializeField]
		private string cloudBuildTargetName = "";

		public string ScmCommitId => scmCommitId;

		public string ScmBranch => scmBranch;

		public string BuildNumber => buildNumber;

		public string BuildStartTime => buildStartTime;

		public string ProjectId => projectId;

		public string BundleId => bundleId;

		public string UnityVersion => unityVersion;

		public string XCodeVersion => xcodeVersion;

		public string CloudBuildTargetName => cloudBuildTargetName;

		public static string VersionText()
		{
			CloudBuildManifest cloudBuildManifest = Instance();
			if (cloudBuildManifest == null)
			{
				return "Not Unity Cloud Build";
			}
			return $"{cloudBuildManifest.CloudBuildTargetName} #{cloudBuildManifest.BuildNumber}  UTC: {cloudBuildManifest.BuildStartTime}";
		}

		public static CloudBuildManifest Instance()
		{
			if (instance != null)
			{
				return instance;
			}
			TextAsset textAsset = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
			if (textAsset == null)
			{
				return null;
			}
			instance = JsonUtility.FromJson<CloudBuildManifest>(textAsset.text);
			return instance;
		}
	}
}
