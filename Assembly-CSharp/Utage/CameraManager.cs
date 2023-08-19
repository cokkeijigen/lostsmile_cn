using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Camera/CameraManager")]
	public class CameraManager : MonoBehaviour, IBinaryIO
	{
		private List<CameraRoot> cameraList;

		private const int Version = 0;

		public string SaveKey
		{
			get
			{
				return "CameraManager";
			}
		}

		public List<CameraRoot> CameraList
		{
			get
			{
				if (cameraList == null)
				{
					cameraList = new List<CameraRoot>(GetComponentsInChildren<CameraRoot>(true));
				}
				return cameraList;
			}
		}

		public CameraRoot FindCameraRoot(string name)
		{
			return CameraList.Find((CameraRoot x) => x.name == name);
		}

		internal Camera FindCameraByLayer(int layer)
		{
			int num = 1 << layer;
			foreach (CameraRoot camera in CameraList)
			{
				Camera cachedCamera = camera.LetterBoxCamera.CachedCamera;
				if ((cachedCamera.cullingMask & num) != 0)
				{
					return cachedCamera;
				}
			}
			return null;
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(CameraList.Count);
			foreach (CameraRoot camera in CameraList)
			{
				writer.Write(camera.name);
				writer.WriteBuffer(camera.Write);
			}
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string text = reader.ReadString();
				CameraRoot cameraRoot = FindCameraRoot(text);
				if (cameraRoot != null)
				{
					reader.ReadBuffer(cameraRoot.Read);
				}
				else
				{
					reader.SkipBuffer();
				}
			}
		}

		internal void OnClear()
		{
			foreach (CameraRoot camera in CameraList)
			{
				camera.OnClear();
			}
		}
	}
}
