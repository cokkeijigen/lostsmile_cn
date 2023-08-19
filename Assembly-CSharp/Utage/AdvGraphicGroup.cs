using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvGraphicGroup
	{
		protected AdvLayerSettingData.LayerType type;

		protected AdvGraphicManager manager;

		private List<AdvGraphicLayer> layers = new List<AdvGraphicLayer>();

		private const int Version = 0;

		internal AdvGraphicLayer DefaultLayer { get; set; }

		internal bool IsLoading
		{
			get
			{
				foreach (AdvGraphicLayer layer in layers)
				{
					if (layer.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal AdvGraphicGroup(AdvLayerSettingData.LayerType type, AdvLayerSetting setting, AdvGraphicManager manager)
		{
			this.type = type;
			this.manager = manager;
			foreach (AdvLayerSettingData item in setting.List)
			{
				if (item.Type == type)
				{
					GameObject gameObject = new GameObject(item.Name, typeof(RectTransform), typeof(Canvas));
					manager.transform.AddChild(gameObject);
					AdvGraphicLayer advGraphicLayer = gameObject.AddComponent<AdvGraphicLayer>();
					advGraphicLayer.Init(manager, item);
					layers.Add(advGraphicLayer);
					if (item.IsDefault)
					{
						DefaultLayer = advGraphicLayer;
					}
				}
			}
		}

		internal virtual void Clear()
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.Clear();
			}
		}

		internal void DestroyAll()
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.Clear();
				Object.Destroy(layer.gameObject);
			}
			layers.Clear();
			DefaultLayer = null;
		}

		internal AdvGraphicObject Draw(string layerName, string name, AdvGraphicOperaitonArg arg)
		{
			return FindLayerOrDefault(layerName).Draw(name, arg);
		}

		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperaitonArg arg)
		{
			return DefaultLayer.DrawToDefault(name, arg);
		}

		internal AdvGraphicObject DrawCharacter(string layerName, string name, AdvGraphicOperaitonArg arg)
		{
			AdvGraphicLayer advGraphicLayer = layers.Find((AdvGraphicLayer item) => item.IsEqualDefaultGraphicName(name));
			AdvGraphicLayer advGraphicLayer2 = layers.Find((AdvGraphicLayer item) => item.SettingData.Name == layerName);
			if (advGraphicLayer2 == null)
			{
				advGraphicLayer2 = ((advGraphicLayer == null) ? DefaultLayer : advGraphicLayer);
			}
			if (!(advGraphicLayer != advGraphicLayer2) || !(advGraphicLayer != null))
			{
				return advGraphicLayer2.DrawToDefault(name, arg);
			}
			Vector3 localScale = Vector3.one;
			Vector3 localPosition = Vector3.zero;
			Quaternion localRotation = Quaternion.identity;
			AdvGraphicObject value;
			if (advGraphicLayer.CurrentGraphics.TryGetValue(name, out value))
			{
				localScale = value.rectTransform.localScale;
				localPosition = value.rectTransform.localPosition;
				localRotation = value.rectTransform.localRotation;
				advGraphicLayer.FadeOut(name, arg.GetSkippedFadeTime(manager.Engine));
			}
			AdvGraphicObject advGraphicObject = advGraphicLayer2.DrawToDefault(name, arg);
			if (!manager.ResetCharacterTransformOnChangeLayer)
			{
				advGraphicObject.rectTransform.localScale = localScale;
				advGraphicObject.rectTransform.localPosition = localPosition;
				advGraphicObject.rectTransform.localRotation = localRotation;
			}
			return advGraphicObject;
		}

		internal List<AdvGraphicLayer> AllGraphicsLayers()
		{
			List<AdvGraphicLayer> list = new List<AdvGraphicLayer>();
			foreach (AdvGraphicLayer layer in layers)
			{
				if (layer.CurrentGraphics.Count > 0)
				{
					list.Add(layer);
				}
			}
			return list;
		}

		internal virtual void FadeOut(string name, float fadeTime)
		{
			AdvGraphicLayer advGraphicLayer = FindLayerFromObjectName(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOut(name, fadeTime);
			}
		}

		internal virtual void FadeOutAll(float fadeTime)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.FadeOutAll(fadeTime);
			}
		}

		internal void FadeOutParticle(string name)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.FadeOutParticle(name);
			}
		}

		internal void FadeOutAllParticle()
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.FadeOutAllParticle();
			}
		}

		internal bool IsContians(string layerName, string name)
		{
			if (string.IsNullOrEmpty(layerName))
			{
				return FindObject(name) != null;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(layerName);
			if (advGraphicLayer != null)
			{
				return advGraphicLayer.Find(name) != null;
			}
			return false;
		}

		internal AdvGraphicLayer FindLayerFromObjectName(string name)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				if (layer.Contains(name))
				{
					return layer;
				}
			}
			return null;
		}

		internal AdvGraphicLayer FindLayer(string name)
		{
			return layers.Find((AdvGraphicLayer item) => item.name == name);
		}

		internal AdvGraphicLayer FindLayerOrDefault(string name)
		{
			return layers.Find((AdvGraphicLayer item) => item.SettingData.Name == name) ?? DefaultLayer;
		}

		internal AdvGraphicObject FindObject(string name)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				AdvGraphicObject advGraphicObject = layer.Find(name);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> list = new List<AdvGraphicObject>();
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.AddAllGraphics(list);
			}
			return list;
		}

		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.AddAllGraphics(graphics);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(layers.Count);
			foreach (AdvGraphicLayer layer in layers)
			{
				writer.Write(layer.name);
				writer.WriteBuffer(layer.Write);
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
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string name = reader.ReadString();
				AdvGraphicLayer advGraphicLayer = FindLayer(name);
				if (advGraphicLayer != null)
				{
					reader.ReadBuffer(advGraphicLayer.Read);
				}
				else
				{
					reader.SkipBuffer();
				}
			}
		}
	}
}
