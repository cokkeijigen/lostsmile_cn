using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicLayer")]
	public class AdvGraphicLayer : MonoBehaviour
	{
		private Dictionary<string, AdvGraphicObject> currentGraphics = new Dictionary<string, AdvGraphicObject>();

		private Transform rootObjects;

		private const int Version = 0;

		public AdvEngine Engine => Manager.Engine;

		public AdvGraphicManager Manager { get; private set; }

		public AdvLayerSettingData SettingData { get; private set; }

		public AdvGraphicObject DefaultObject { get; private set; }

		public Dictionary<string, AdvGraphicObject> CurrentGraphics => currentGraphics;

		public Camera Camera { get; private set; }

		public LetterBoxCamera LetterBoxCamera { get; private set; }

		public Canvas Canvas { get; private set; }

		public Vector2 GameScreenSize => LetterBoxCamera.CurrentSize;

		internal bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
				{
					if (currentGraphic.Value.Loader.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Init(AdvGraphicManager manager, AdvLayerSettingData settingData)
		{
			Manager = manager;
			SettingData = settingData;
			Canvas = GetComponent<Canvas>();
			Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
			if (!string.IsNullOrEmpty(SettingData.LayerMask))
			{
				Canvas.gameObject.layer = LayerMask.NameToLayer(SettingData.LayerMask);
			}
			Canvas.sortingOrder = SettingData.Order;
			Camera = Engine.CameraManager.FindCameraByLayer(Canvas.gameObject.layer);
			if (Camera == null)
			{
				Debug.LogError("Cant find camera");
				Camera = Engine.CameraManager.FindCameraByLayer(0);
			}
			LetterBoxCamera = Camera.gameObject.GetComponent<LetterBoxCamera>();
			Canvas.worldCamera = Camera;
			Canvas.gameObject.AddComponent<GraphicRaycaster>().enabled = false;
			rootObjects = Canvas.transform;
			ResetCanvasRectTransform();
			if (Manager.DebugAutoResetCanvasPosition)
			{
				LetterBoxCamera.OnGameScreenSizeChange.AddListener(delegate
				{
					ResetCanvasRectTransform();
				});
			}
		}

		internal void ResetCanvasRectTransform()
		{
			RectTransform obj = Canvas.transform as RectTransform;
			SettingData.Horizontal.GetBorderdPositionAndSize(GameScreenSize.x, out var position, out var size);
			SettingData.Vertical.GetBorderdPositionAndSize(GameScreenSize.y, out var position2, out var size2);
			obj.localPosition = new Vector3(position, position2, SettingData.Z) / Manager.PixelsToUnits;
			obj.SetSize(size, size2);
			obj.localScale = SettingData.Scale / Manager.PixelsToUnits;
		}

		internal void Remove(AdvGraphicObject obj)
		{
			if (currentGraphics.ContainsValue(obj))
			{
				currentGraphics.Remove(obj.name);
			}
			if (DefaultObject == obj)
			{
				DefaultObject = null;
			}
		}

		internal AdvGraphicObject Draw(string name, AdvGraphicOperaitonArg arg)
		{
		#if DEBUG
            var index = arg.Graphic.File.FileName.IndexOf("file:///");
            if (index >= 0)
            {
				var file = arg.Graphic.File.FileName.Substring(index + 8);
                CHSPatch.Logger.OutMessage($"[AdvGraphicLayer::Draw] {name} {file}");
            }
		#endif
			AdvGraphicObject obj = GetObjectCreateIfMissing(name, arg.Graphic);
			obj.Loader.LoadGraphic(arg.Graphic, delegate
			{
				obj.Draw(arg, arg.GetSkippedFadeTime(Engine));
			});
			return obj;
		}

		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperaitonArg arg)
		{
			if (CheckChangeDafaultObject(name, arg))
			{
				if (SettingData.Type == AdvLayerSettingData.LayerType.Bg)
				{
					DelayOut(DefaultObject.name, arg.GetSkippedFadeTime(Engine));
				}
				else
				{
					FadeOut(DefaultObject.name, arg.GetSkippedFadeTime(Engine));
				}
			}
			DefaultObject = Draw(name, arg);
			return DefaultObject;
		}

		private bool CheckChangeDafaultObject(string name, AdvGraphicOperaitonArg arg)
		{
			if (DefaultObject == null)
			{
				return false;
			}
			if (DefaultObject.name != name)
			{
				return true;
			}
			if (DefaultObject.LastResource == null)
			{
				return false;
			}
			if (arg.Graphic.FileType != DefaultObject.LastResource.FileType)
			{
				return true;
			}
			return DefaultObject.TargetObject.CheckFailedCrossFade(arg.Graphic);
		}

		internal AdvGraphicObject GetObjectCreateIfMissing(string name, AdvGraphicInfo grapic)
		{
			if (grapic == null)
			{
				Debug.LogError(name + " grapic is null");
				return null;
			}
			if (!currentGraphics.TryGetValue(name, out var value))
			{
				return CreateObject(name, grapic);
			}
			return value;
		}

		private AdvGraphicObject CreateObject(string name, AdvGraphicInfo grapic)
		{
			AdvGraphicObject advGraphicObject;
			if (grapic.TryGetAdvGraphicObjectPrefab(out var prefab))
			{
				GameObject obj = Object.Instantiate(prefab);
				obj.name = name;
				advGraphicObject = obj.GetComponent<AdvGraphicObject>();
				rootObjects.AddChild(advGraphicObject.gameObject);
			}
			else
			{
				advGraphicObject = rootObjects.AddChildGameObjectComponent<AdvGraphicObject>(name);
			}
			advGraphicObject.Init(this, grapic);
			if (currentGraphics.Count == 0)
			{
				ResetCanvasRectTransform();
			}
			currentGraphics.Add(advGraphicObject.name, advGraphicObject);
			return advGraphicObject;
		}

		internal void FadeOut(string name, float fadeTime)
		{
			if (currentGraphics.TryGetValue(name, out var value))
			{
				value.FadeOut(fadeTime);
				Remove(value);
			}
		}

		internal void DelayOut(string name, float delay)
		{
			if (currentGraphics.TryGetValue(name, out var value))
			{
				Remove(value);
				StartCoroutine(CoDelayOut(value, delay));
			}
		}

		private IEnumerator CoDelayOut(AdvGraphicObject obj, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (obj != null)
			{
				obj.Clear();
			}
		}

		internal void FadeOutAll(float fadeTime)
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				item.FadeOut(fadeTime);
			}
			currentGraphics.Clear();
			DefaultObject = null;
		}

		internal void FadeOutParticle(string name)
		{
			if (currentGraphics.TryGetValue(name, out var value) && value.TargetObject is AdvGraphicObjectParticle)
			{
				value.FadeOut(0f);
				Remove(value);
			}
		}

		internal void FadeOutAllParticle()
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				if (item.TargetObject is AdvGraphicObjectParticle)
				{
					item.FadeOut(0f);
					Remove(item);
				}
			}
		}

		internal void Clear()
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				item.Clear();
			}
			currentGraphics.Clear();
			DefaultObject = null;
		}

		internal bool IsEqualDefaultGraphicName(string name)
		{
			if (DefaultObject != null)
			{
				return DefaultObject.name == name;
			}
			return false;
		}

		internal bool Contains(string name)
		{
			return currentGraphics.ContainsKey(name);
		}

		internal AdvGraphicObject Find(string name)
		{
			if (currentGraphics.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}

		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			graphics.AddRange(currentGraphics.Values);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.WriteLocalTransform(base.transform);
			int num = 0;
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
			{
				if (currentGraphic.Value.LastResource.DataType == "Capture")
				{
					Debug.LogError("Caputure image not support on save");
				}
				else
				{
					num++;
				}
			}
			writer.Write(num);
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic2 in currentGraphics)
			{
				if (!(currentGraphic2.Value.LastResource.DataType == "Capture"))
				{
					writer.Write(currentGraphic2.Key);
					writer.WriteBuffer(currentGraphic2.Value.LastResource.OnWrite);
					writer.WriteBuffer(currentGraphic2.Value.Write);
				}
			}
			writer.Write((DefaultObject == null) ? "" : DefaultObject.name);
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
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string text = reader.ReadString();
				AdvGraphicInfo graphic = null;
				reader.ReadBuffer(delegate(BinaryReader x)
				{
					graphic = AdvGraphicInfo.ReadGraphicInfo(Engine, x);
				});
				byte[] buffer = reader.ReadBuffer();
				CreateObject(text, graphic).Read(buffer, graphic);
			}
			string text2 = reader.ReadString();
			DefaultObject = Find(text2);
		}
	}
}
