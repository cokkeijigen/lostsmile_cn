using UnityEngine;

namespace Utage
{
	public class AssetBundleInfo
	{
		public string Url { get; set; }

		public Hash128 Hash { get; set; }

		public int Version { get; set; }

		public int Size { get; set; }

		internal AssetBundleInfo(string url, Hash128 hash, int size = 0)
		{
			Url = url;
			Hash = hash;
			Version = int.MinValue;
			Size = size;
		}

		internal AssetBundleInfo(string url, int version, int size = 0)
		{
			Url = url;
			Version = version;
			Size = size;
		}
	}
}
