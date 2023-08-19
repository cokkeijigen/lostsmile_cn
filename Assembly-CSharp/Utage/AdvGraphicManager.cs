using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/GraphicManager")]
	public class AdvGraphicManager : MonoBehaviour, IBinaryIO
	{
		[SerializeField]
		private float pixelsToUnits = 100f;

		[SerializeField]
		private float sortOderToZUnits = 100f;

		[SerializeField]
		private string bgSpriteName = "BG";

		[SerializeField]
		private bool resetCharacterTransformOnChangeLayer = true;

		[SerializeField]
		private AdvGraphicRenderTextureManager renderTextureManager;

		[SerializeField]
		private AdvVideoManager videoManager;

		private bool isEventMode;

		private Dictionary<AdvLayerSettingData.LayerType, AdvGraphicGroup> Groups = new Dictionary<AdvLayerSettingData.LayerType, AdvGraphicGroup>();

		private AdvEngine engine;

		private const int Version = 0;

		public float PixelsToUnits
		{
			get
			{
				return pixelsToUnits;
			}
		}

		public float SortOderToZUnits
		{
			get
			{
				return sortOderToZUnits;
			}
		}

		public string BgSpriteName
		{
			get
			{
				return bgSpriteName;
			}
		}

		public bool ResetCharacterTransformOnChangeLayer
		{
			get
			{
				return resetCharacterTransformOnChangeLayer;
			}
		}

		public bool DebugAutoResetCanvasPosition
		{
			get
			{
				return false;
			}
		}

		public AdvGraphicRenderTextureManager RenderTextureManager
		{
			get
			{
				if (renderTextureManager == null)
				{
					renderTextureManager = base.transform.parent.AddChildGameObjectComponent<AdvGraphicRenderTextureManager>("GraphicRenderTextureManager");
				}
				return renderTextureManager;
			}
		}

		public AdvVideoManager VideoManager
		{
			get
			{
				if (videoManager == null)
				{
					videoManager = base.transform.parent.AddChildGameObjectComponent<AdvVideoManager>("VideoManager");
				}
				return videoManager;
			}
		}

		public bool IsEventMode
		{
			get
			{
				return isEventMode;
			}
			set
			{
				isEventMode = value;
			}
		}

		public AdvGraphicGroup CharacterManager
		{
			get
			{
				return Groups[AdvLayerSettingData.LayerType.Character];
			}
		}

		public AdvGraphicGroup SpriteManager
		{
			get
			{
				return Groups[AdvLayerSettingData.LayerType.Sprite];
			}
		}

		public AdvGraphicGroup BgManager
		{
			get
			{
				return Groups[AdvLayerSettingData.LayerType.Bg];
			}
		}

		internal AdvEngine Engine
		{
			get
			{
				return engine;
			}
		}

		internal bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
				{
					if (group.Value.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string SaveKey
		{
			get
			{
				return "AdvGraphicManager";
			}
		}

		public void BootInit(AdvEngine engine, AdvLayerSetting setting)
		{
			this.engine = engine;
			Groups.Clear();
			foreach (AdvLayerSettingData.LayerType value2 in Enum.GetValues(typeof(AdvLayerSettingData.LayerType)))
			{
				AdvGraphicGroup value = new AdvGraphicGroup(value2, setting, this);
				Groups.Add(value2, value);
			}
		}

		public void Remake(AdvLayerSetting setting)
		{
			foreach (AdvGraphicGroup value2 in Groups.Values)
			{
				value2.DestroyAll();
			}
			Groups.Clear();
			foreach (AdvLayerSettingData.LayerType value3 in Enum.GetValues(typeof(AdvLayerSettingData.LayerType)))
			{
				AdvGraphicGroup value = new AdvGraphicGroup(value3, setting, this);
				Groups.Add(value3, value);
			}
		}

		internal void Clear()
		{
			foreach (AdvGraphicGroup value in Groups.Values)
			{
				value.Clear();
			}
		}

		internal AdvGraphicLayer FindLayer(string layerName)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicLayer advGraphicLayer = group.Value.FindLayer(layerName);
				if (advGraphicLayer != null)
				{
					return advGraphicLayer;
				}
			}
			return null;
		}

		internal AdvGraphicLayer FindLayerByObjectName(string name)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicLayer advGraphicLayer = group.Value.FindLayerFromObjectName(name);
				if (advGraphicLayer != null)
				{
					return advGraphicLayer;
				}
			}
			return null;
		}

		internal AdvGraphicObject FindObject(string name)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicObject advGraphicObject = group.Value.FindObject(name);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal GameObject FindObjectOrLayer(string name)
		{
			AdvGraphicObject advGraphicObject = FindObject(name);
			if (advGraphicObject != null)
			{
				return advGraphicObject.gameObject;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(name);
			if (advGraphicLayer != null)
			{
				return advGraphicLayer.gameObject;
			}
			return null;
		}

		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> list = new List<AdvGraphicObject>();
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.AddAllGraphics(list);
			}
			return list;
		}

		internal void DrawObject(string layerName, string label, AdvGraphicOperaitonArg graphicOperaitonArg)
		{
			FindLayer(layerName).Draw(label, graphicOperaitonArg);
		}

		internal void FadeOutParticle(string name)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.FadeOutParticle(name);
			}
		}

		internal void FadeOutAllParticle()
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.FadeOutAllParticle();
			}
		}

		internal void CreateCaptureImageObject(string name, string cameraName, string layerName)
		{
			AdvGraphicLayer advGraphicLayer = FindLayer(layerName);
			if (advGraphicLayer == null)
			{
				Debug.LogError(layerName + " is not layer name");
				return;
			}
			CameraRoot cameraRoot = Engine.CameraManager.FindCameraRoot(cameraName);
			if (cameraRoot == null)
			{
				Debug.LogError(cameraName + " is not camera name");
				return;
			}
			AdvGraphicInfo grapic = new AdvGraphicInfo("Capture", name, "2D");
			advGraphicLayer.GetObjectCreateIfMissing(name, grapic).InitCaptureImage(grapic, cameraRoot.LetterBoxCamera.CachedCamera);
		}

		internal void RemoveClickEvent(string name)
		{
			AdvGraphicObject advGraphicObject = FindObject(name);
			if (!(advGraphicObject == null))
			{
				IAdvClickEvent componentInChildren = advGraphicObject.gameObject.GetComponentInChildren<IAdvClickEvent>();
				if (componentInChildren != null)
				{
					componentInChildren.RemoveClickEvent();
				}
			}
		}

		internal void AddClickEvent(string name, bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			AdvGraphicObject advGraphicObject = FindObject(name);
			if (advGraphicObject == null)
			{
				Debug.LogError("can't find Graphic object" + name);
				return;
			}
			IAdvClickEvent componentInChildren = advGraphicObject.gameObject.GetComponentInChildren<IAdvClickEvent>();
			if (componentInChildren == null)
			{
				Debug.LogError("can't find IAdvClickEvent Interface in " + name);
			}
			else
			{
				componentInChildren.AddClickEvent(isPolygon, row, action);
			}
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(isEventMode);
			writer.Write(Groups.Count);
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				writer.Write((int)group.Key);
				writer.WriteBuffer(group.Value.Write);
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
			isEventMode = reader.ReadBoolean();
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				AdvLayerSettingData.LayerType key = (AdvLayerSettingData.LayerType)reader.ReadInt32();
				reader.ReadBuffer(Groups[key].Read);
			}
		}
	}
}
