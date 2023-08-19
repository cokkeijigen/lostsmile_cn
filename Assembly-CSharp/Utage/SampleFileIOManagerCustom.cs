using System;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Examples/FileIOManagerCustom")]
	public class SampleFileIOManagerCustom : FileIOManager
	{
		public override void CreateDirectory(string path)
		{
		}

		public override void DeleteDirectory(string path)
		{
		}

		public override bool Exists(string path)
		{
			Debug.Log("Custom File Check");
			return false;
		}

		protected override byte[] FileReadAllBytes(string path)
		{
			Debug.Log("Custom FileRead");
			return Convert.FromBase64String("");
		}

		protected override void FileWriteAllBytes(string path, byte[] bytes)
		{
			Convert.ToBase64String(bytes);
			Debug.Log("Custom File Write");
		}

		public override void Delete(string path)
		{
			Debug.Log("Custom File Delete");
		}
	}
}
