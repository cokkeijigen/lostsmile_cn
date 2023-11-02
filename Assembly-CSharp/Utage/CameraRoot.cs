using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Camera/CameraRoot")]
	public class CameraRoot : MonoBehaviour
	{
		private LetterBoxCamera letterBoxCamera;

		private Vector3 startPosition = Vector3.zero;

		private Vector3 startScale = Vector3.one;

		private Vector3 startEulerAngles = Vector3.one;

		private const int Version = 0;

		public LetterBoxCamera LetterBoxCamera
		{
			get
			{
				if (letterBoxCamera == null)
				{
					letterBoxCamera = base.gameObject.GetComponentInChildren<LetterBoxCamera>(true);
				}
				return letterBoxCamera;
			}
		}

		private void Awake()
		{
			startPosition = base.transform.localPosition;
			startScale = base.transform.localScale;
			startEulerAngles = base.transform.localEulerAngles;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.WriteLocalTransform(base.transform);
			writer.WriteBuffer(LetterBoxCamera.Write);
			ImageEffectBase[] components = LetterBoxCamera.GetComponents<ImageEffectBase>();
			writer.Write(components.Length);
			for (int i = 0; i < components.Length; i++)
			{
				string value = ImageEffectUtil.ToImageEffectType(components[i].GetType());
				writer.Write(value);
				writer.WriteBuffer(components[i].Write);
			}
		}

		public void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			reader.ReadLocalTransform(base.transform);
			reader.ReadBuffer(LetterBoxCamera.Read);
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string text = reader.ReadString();
				if (!ImageEffectUtil.TryGetComonentCreateIfMissing(text, out var component, out var _, LetterBoxCamera.gameObject))
				{
					Debug.LogError("Unkonwo Image Effect Type [ " + text + " ]");
					reader.SkipBuffer();
				}
				else
				{
					reader.ReadBuffer(component.Read);
				}
			}
		}

		internal void OnClear()
		{
			base.transform.localPosition = startPosition;
			base.transform.localScale = startScale;
			base.transform.localEulerAngles = startEulerAngles;
			LetterBoxCamera.gameObject.RemoveComponents<ImageEffectBase>();
		}
	}
}
