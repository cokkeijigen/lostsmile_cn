using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	public class UtageToolKit
	{
		private static CultureInfo cultureInfJp = new CultureInfo("ja-JP");

		public static bool IsHankaku(char c)
		{
			if (c > '~')
			{
				switch (c)
				{
				case '¥':
				case '‾':
				case '｡':
				case '｢':
				case '｣':
				case '､':
				case '･':
				case 'ｦ':
				case 'ｧ':
				case 'ｨ':
				case 'ｩ':
				case 'ｪ':
				case 'ｫ':
				case 'ｬ':
				case 'ｭ':
				case 'ｮ':
				case 'ｯ':
				case 'ｰ':
				case 'ｱ':
				case 'ｲ':
				case 'ｳ':
				case 'ｴ':
				case 'ｵ':
				case 'ｶ':
				case 'ｷ':
				case 'ｸ':
				case 'ｹ':
				case 'ｺ':
				case 'ｻ':
				case 'ｼ':
				case 'ｽ':
				case 'ｾ':
				case 'ｿ':
				case 'ﾀ':
				case 'ﾁ':
				case 'ﾂ':
				case 'ﾃ':
				case 'ﾄ':
				case 'ﾅ':
				case 'ﾆ':
				case 'ﾇ':
				case 'ﾈ':
				case 'ﾉ':
				case 'ﾊ':
				case 'ﾋ':
				case 'ﾌ':
				case 'ﾍ':
				case 'ﾎ':
				case 'ﾏ':
				case 'ﾐ':
				case 'ﾑ':
				case 'ﾒ':
				case 'ﾓ':
				case 'ﾔ':
				case 'ﾕ':
				case 'ﾖ':
				case 'ﾗ':
				case 'ﾘ':
				case 'ﾙ':
				case 'ﾚ':
				case 'ﾛ':
				case 'ﾜ':
				case 'ﾝ':
				case 'ﾞ':
				case 'ﾟ':
					break;
				default:
					return false;
				}
			}
			return true;
		}

		public static bool IsPlatformStandAloneOrEditor()
		{
			if (!Application.isEditor)
			{
				return IsPlatformStandAlone();
			}
			return true;
		}

		public static bool IsPlatformStandAlone()
		{
			RuntimePlatform platform = Application.platform;
			if ((uint)(platform - 1) <= 1u || platform == RuntimePlatform.LinuxPlayer)
			{
				return true;
			}
			return false;
		}

		public static Texture2D CaptureScreen()
		{
			return CaptureScreen(new Rect(0f, 0f, Screen.width, Screen.height));
		}

		public static Texture2D CaptureScreen(Rect rect)
		{
			return CaptureScreen(TextureFormat.RGB24, rect);
		}

		public static Texture2D CaptureScreen(TextureFormat format, Rect rect)
		{
			Texture2D texture2D = new Texture2D((int)rect.width, (int)rect.height, format, false);
			try
			{
				texture2D.ReadPixels(rect, 0, 0);
				texture2D.Apply();
			}
			catch
			{
			}
			return texture2D;
		}

		public static string DateToStringJp(DateTime date)
		{
			return date.ToString(cultureInfJp);
		}

		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height)
		{
			if (tex == null)
			{
				return null;
			}
			return CreateResizeTexture(tex, width, height, tex.format, tex.mipmapCount > 1);
		}

		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height, TextureFormat format, bool isMipmap)
		{
			if (tex == null)
			{
				return null;
			}
			TextureWrapMode wrapMode = tex.wrapMode;
			tex.wrapMode = TextureWrapMode.Clamp;
			Color[] array = new Color[width * height];
			int num = 0;
			for (int i = 0; i < height; i++)
			{
				float y = 1f * (float)i / (float)(height - 1);
				for (int j = 0; j < width; j++)
				{
					float x = 1f * (float)j / (float)(width - 1);
					array[num] = tex.GetPixelBilinear(x, y);
					num++;
				}
			}
			tex.wrapMode = wrapMode;
			Texture2D texture2D = new Texture2D(width, height, format, isMipmap);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height, TextureFormat format)
		{
			return CreateResizeTexture(tex, width, height, format, false);
		}

		public static Sprite CreateSprite(Texture2D tex, float pixelsToUnits)
		{
			return CreateSprite(tex, pixelsToUnits, new Vector2(0.5f, 0.5f));
		}

		public static Sprite CreateSprite(Texture2D tex, float pixelsToUnits, Vector2 pivot)
		{
			if (tex == null)
			{
				Debug.LogError("texture is null");
				tex = Texture2D.whiteTexture;
			}
			if (tex.mipmapCount > 1)
			{
				Debug.LogWarning(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.SpriteMimMap, tex.name));
			}
			Rect rect = new Rect(0f, 0f, tex.width, tex.height);
			return Sprite.Create(tex, rect, pivot, pixelsToUnits);
		}

		[Obsolete]
		public static bool TryParaseEnum<T>(string str, out T val)
		{
			try
			{
				val = (T)Enum.Parse(typeof(T), str);
				return true;
			}
			catch (Exception)
			{
				val = default(T);
				return false;
			}
		}

		[Obsolete]
		public static void SafeSendMessage(GameObject target, string functionName, object obj = null, bool isForceActive = false)
		{
			if (!(target == null))
			{
				target.SafeSendMessage(functionName, obj, isForceActive);
			}
		}

		[Obsolete]
		public static void SafeSendMessage(object obj, GameObject target, string functionName, bool isForceActive = false)
		{
			SafeSendMessage(target, functionName, obj, isForceActive);
		}

		[Obsolete]
		public static void DestroyChildren(Transform parent)
		{
			parent.DestroyChildren();
		}

		[Obsolete]
		public static void DestroyChildrenInEditorOrPlayer(Transform parent)
		{
			parent.DestroyChildrenInEditorOrPlayer();
		}

		[Obsolete]
		public static GameObject AddChild(Transform parent, GameObject go)
		{
			return parent.AddChild(go, Vector3.zero, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChild(Transform parent, GameObject go, Vector3 localPosition)
		{
			return parent.AddChild(go, localPosition, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChild(Transform parent, GameObject go, Vector3 localPosition, Vector3 localScale)
		{
			return parent.AddChild(go, localPosition, localScale);
		}

		[Obsolete]
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab)
		{
			return parent.AddChildPrefab(prefab, Vector3.zero, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab, Vector3 localPosition)
		{
			return parent.AddChildPrefab(prefab, localPosition, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab, Vector3 localPosition, Vector3 localScale)
		{
			return parent.AddChildPrefab(prefab, localPosition, localScale);
		}

		[Obsolete]
		public static GameObject AddChildUnityObject(Transform parent, UnityEngine.Object obj)
		{
			return parent.AddChildUnityObject(obj);
		}

		[Obsolete]
		public static void ChangeLayerAllChildren(Transform trans, int layer)
		{
			trans.gameObject.ChangeLayerDeep(layer);
		}

		[Obsolete]
		public static T AddChildGameObjectComponent<T>(Transform parent, string name) where T : Component
		{
			return parent.AddChildGameObjectComponent<T>(name, Vector3.zero, Vector3.one);
		}

		[Obsolete]
		public static T AddChildGameObjectComponent<T>(Transform parent, string name, Vector3 localPosition) where T : Component
		{
			return parent.AddChildGameObjectComponent<T>(name, localPosition, Vector3.one);
		}

		[Obsolete]
		public static T AddChildGameObjectComponent<T>(Transform parent, string name, Vector3 localPosition, Vector3 localScale) where T : Component
		{
			return parent.AddChildGameObjectComponent<T>(name, localPosition, localScale);
		}

		[Obsolete]
		public static GameObject AddChildGameObject(Transform parent, string name)
		{
			return parent.AddChildGameObject(name, Vector3.zero, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChildGameObject(Transform parent, string name, Vector3 localPosition)
		{
			return parent.AddChildGameObject(name, localPosition, Vector3.one);
		}

		[Obsolete]
		public static GameObject AddChildGameObject(Transform parent, string name, Vector3 localPosition, Vector3 localScale)
		{
			return parent.AddChildGameObject(name, localPosition, localScale);
		}

		[Obsolete]
		public static T FindParentComponent<T>(Transform transform) where T : Component
		{
			return transform.GetComponentInParent<T>();
		}

		[Obsolete]
		public static Transform FindInChirdlen(Transform trasnform, string name)
		{
			return trasnform.FindDeep(name, true);
		}

		[Obsolete]
		public static T GetCompoentInChildrenCreateIfMissing<T>(Transform trasnform) where T : Component
		{
			return trasnform.GetComponentCreateIfMissing<T>();
		}

		[Obsolete]
		internal static T GetComponentCreateIfMissing<T>(GameObject go) where T : Component
		{
			return go.GetComponentCreateIfMissing<T>();
		}

		[Obsolete]
		public static T[] GetInterfaceCompoents<T>(GameObject go) where T : class
		{
			return go.GetComponents<T>();
		}

		[Obsolete]
		public static T GetInterfaceCompoent<T>(GameObject go) where T : class
		{
			return go.GetComponent<T>();
		}

		[Obsolete]
		public static void WriteLocalTransform(Transform transform, BinaryWriter writer)
		{
			writer.Write(transform.localPosition.x);
			writer.Write(transform.localPosition.y);
			writer.Write(transform.localPosition.z);
			writer.Write(transform.localEulerAngles.x);
			writer.Write(transform.localEulerAngles.y);
			writer.Write(transform.localEulerAngles.z);
			writer.Write(transform.localScale.x);
			writer.Write(transform.localScale.y);
			writer.Write(transform.localScale.z);
		}

		[Obsolete]
		public static void WriteColor(Color color, BinaryWriter writer)
		{
			writer.Write(color.r);
			writer.Write(color.g);
			writer.Write(color.b);
			writer.Write(color.a);
		}

		[Obsolete]
		public static void ReadLocalTransform(Transform transform, BinaryReader reader)
		{
			Vector3 pos = default(Vector3);
			Vector3 euler = default(Vector3);
			Vector3 scale = default(Vector3);
			ReadLocalTransform(reader, out pos, out euler, out scale);
			transform.localPosition = pos;
			transform.localEulerAngles = euler;
			transform.localScale = scale;
		}

		[Obsolete]
		public static void ReadLocalTransform(BinaryReader reader, out Vector3 pos, out Vector3 euler, out Vector3 scale)
		{
			pos.x = reader.ReadSingle();
			pos.y = reader.ReadSingle();
			pos.z = reader.ReadSingle();
			euler.x = reader.ReadSingle();
			euler.y = reader.ReadSingle();
			euler.z = reader.ReadSingle();
			scale.x = reader.ReadSingle();
			scale.y = reader.ReadSingle();
			scale.z = reader.ReadSingle();
		}

		[Obsolete]
		public static Color ReadColor(BinaryReader reader)
		{
			Color result = default(Color);
			result.r = reader.ReadSingle();
			result.g = reader.ReadSingle();
			result.b = reader.ReadSingle();
			result.a = reader.ReadSingle();
			return result;
		}

		[Obsolete]
		public static void AddEventTriggerEntry(EventTrigger eventTrigger, UnityAction<BaseEventData> action, EventTriggerType eventTriggerType)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener(delegate(BaseEventData eventData)
			{
				action(eventData);
			});
			entry.callback = triggerEvent;
			entry.eventID = eventTriggerType;
			WrapperUnityVersion.AddEntryToEventTrigger(eventTrigger, entry);
		}

		[Obsolete]
		internal static T[] AddArrayUnique<T>(T[] array, T[] addArray)
		{
			List<T> list = new List<T>(array);
			foreach (T item in addArray)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}
	}
}
