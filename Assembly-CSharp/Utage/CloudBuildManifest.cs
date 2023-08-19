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

		public string ScmCommitId
		{
			get
			{
				return scmCommitId;
			}
		}

		public string ScmBranch
		{
			get
			{
				return scmBranch;
			}
		}

		public string BuildNumber
		{
			get
			{
				return buildNumber;
			}
		}

		public string BuildStartTime
		{
			get
			{
				return buildStartTime;
			}
		}

		public string ProjectId
		{
			get
			{
				return projectId;
			}
		}

		public string BundleId
		{
			get
			{
				return bundleId;
			}
		}

		public string UnityVersion
		{
			get
			{
				return unityVersion;
			}
		}

		public string XCodeVersion
		{
			get
			{
				return xcodeVersion;
			}
		}

		public string CloudBuildTargetName
		{
			get
			{
				return cloudBuildTargetName;
			}
		}

		public static string VersionText()
		{
			CloudBuildManifest cloudBuildManifest = Instance();
			if (cloudBuildManifest == null)
			{
				return "Not Unity Cloud Build";
			}
			return string.Format("{0} #{1}  UTC: {2}", cloudBuildManifest.CloudBuildTargetName, cloudBuildManifest.BuildNumber, cloudBuildManifest.BuildStartTime);
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
