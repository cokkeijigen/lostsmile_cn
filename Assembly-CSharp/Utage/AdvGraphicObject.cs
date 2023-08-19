using System;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject")]
	[RequireComponent(typeof(RectTransform))]
	public class AdvGraphicObject : MonoBehaviour, IAdvFade
	{
		private AdvGraphicLoader loader;

		protected AdvGraphicLayer layer;

		private AdvEffectColor effectColor;

		private const int Version = 1;

		private const int Version0 = 0;

		public AdvGraphicLoader Loader
		{
			get
			{
				return this.GetComponentCacheCreateIfMissing(ref loader);
			}
		}

		public AdvGraphicLayer Layer
		{
			get
			{
				return layer;
			}
		}

		public AdvEngine Engine
		{
			get
			{
				return Layer.Manager.Engine;
			}
		}

		public AdvGraphicInfo LastResource { get; private set; }

		public float PixelsToUnits
		{
			get
			{
				return Layer.Manager.PixelsToUnits;
			}
		}

		public bool EnableRenderTexture
		{
			get
			{
				if (LastResource != null)
				{
					return LastResource.RenderTextureSetting.EnableRenderTexture;
				}
				return false;
			}
		}

		public AdvGraphicBase TargetObject { get; private set; }

		public AdvGraphicBase RenderObject { get; private set; }

		public AdvRenderTextureSpace RenderTextureSpace { get; private set; }

		private Timer FadeTimer { get; set; }

		public AdvEffectColor EffectColor
		{
			get
			{
				return this.GetComponentCacheCreateIfMissing(ref effectColor);
			}
		}

		public RectTransform rectTransform { get; private set; }

		public virtual void Init(AdvGraphicLayer layer, AdvGraphicInfo graphic)
		{
			this.layer = layer;
			rectTransform = base.transform as RectTransform;
			rectTransform.SetStretch();
			if (graphic.RenderTextureSetting.EnableRenderTexture)
			{
				InitRenderTextureImage(graphic);
			}
			else
			{
				GameObject gameObject = base.transform.AddChildGameObject(graphic.Key);
				AdvGraphicBase targetObject = (RenderObject = gameObject.AddComponent(graphic.GetComponentType()) as AdvGraphicBase);
				TargetObject = targetObject;
				TargetObject.Init(this);
			}
			LipSynchBase componentInChildren = TargetObject.GetComponentInChildren<LipSynchBase>();
			if (componentInChildren != null)
			{
				componentInChildren.CharacterLabel = base.gameObject.name;
				componentInChildren.OnCheckTextLipSync.AddListener(delegate(LipSynchBase x)
				{
					x.EnableTextLipSync = x.CharacterLabel == Engine.Page.CharacterLabel && Engine.Page.IsSendChar;
				});
			}
			FadeTimer = base.gameObject.AddComponent<Timer>();
			effectColor = this.GetComponentCreateIfMissing<AdvEffectColor>();
			effectColor.OnValueChanged.AddListener(RenderObject.OnEffectColorsChange);
		}

		private void InitRenderTextureImage(AdvGraphicInfo graphic)
		{
			AdvGraphicManager manager = Layer.Manager;
			RenderTextureSpace = manager.RenderTextureManager.CreateSpace();
			RenderTextureSpace.Init(graphic, manager.PixelsToUnits);
			AdvGraphicObjectRenderTextureImage advGraphicObjectRenderTextureImage = (AdvGraphicObjectRenderTextureImage)(RenderObject = base.transform.AddChildGameObject(graphic.Key).AddComponent<AdvGraphicObjectRenderTextureImage>());
			advGraphicObjectRenderTextureImage.Init(RenderTextureSpace);
			RenderObject.Init(this);
			TargetObject = RenderTextureSpace.RenderRoot.transform.AddChildGameObject(graphic.Key).AddComponent(graphic.GetComponentType()) as AdvGraphicBase;
			TargetObject.Init(this);
		}

		public virtual void Draw(AdvGraphicOperaitonArg arg, float fadeTime)
		{
			DrawSub(arg.Graphic, fadeTime);
		}

		private void DrawSub(AdvGraphicInfo graphic, float fadeTime)
		{
			TargetObject.name = graphic.File.FileName;
			TargetObject.ChangeResourceOnDraw(graphic, fadeTime);
			if (RenderObject != TargetObject)
			{
				RenderObject.ChangeResourceOnDraw(graphic, fadeTime);
				if (graphic.IsUguiComponentType)
				{
					RenderObject.Scale(graphic);
				}
			}
			else
			{
				TargetObject.Scale(graphic);
			}
			RenderObject.Alignment(Layer.SettingData.Alignment, graphic);
			RenderObject.Flip(Layer.SettingData.FlipX, Layer.SettingData.FlipY);
			LastResource = graphic;
		}

		internal virtual void SetCommandPostion(AdvCommand command)
		{
			bool flag = false;
			Vector3 localPosition = base.transform.localPosition;
			float val;
			if (command.TryParseCell<float>(AdvColumnName.Arg4, out val))
			{
				localPosition.x = val;
				flag = true;
			}
			float val2;
			if (command.TryParseCell<float>(AdvColumnName.Arg5, out val2))
			{
				localPosition.y = val2;
				flag = true;
			}
			if (flag)
			{
				base.transform.localPosition = localPosition;
			}
		}

		public virtual bool TryFadeIn(float time)
		{
			if (TargetObject != null)
			{
				FadeIn(time, null);
				return true;
			}
			return false;
		}

		public virtual void ChangePattern(string pattern)
		{
			if (TargetObject != null)
			{
				TargetObject.ChangePattern(pattern);
			}
		}

		public void FadeIn(float fadeTime, Action onComplete)
		{
			float begin = 0f;
			float end = 1f;
			FadeTimer.StartTimer(fadeTime, delegate(Timer x)
			{
				EffectColor.FadeAlpha = x.GetCurve(begin, end);
			}, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public virtual void FadeOut(float time)
		{
			FadeOut(time, Clear);
		}

		public void FadeOut(float time, Action onComplete)
		{
			if (TargetObject == null)
			{
				if (onComplete != null)
				{
					onComplete();
				}
				return;
			}
			float begin = EffectColor.FadeAlpha;
			float end = 0f;
			FadeTimer.StartTimer(time, delegate(Timer x)
			{
				EffectColor.FadeAlpha = x.GetCurve(begin, end);
			}, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			if (TargetObject == null)
			{
				if (onComplete != null)
				{
					onComplete();
				}
			}
			else
			{
				RenderObject.RuleFadeIn(engine, data, onComplete);
			}
		}

		public void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			if (TargetObject == null)
			{
				if (onComplete != null)
				{
					onComplete();
				}
				Clear();
				return;
			}
			RenderObject.RuleFadeOut(engine, data, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
				Clear();
			});
		}

		public virtual void Clear()
		{
			RemoveFromLayer();
			base.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		protected virtual void OnDestroy()
		{
			RemoveFromLayer();
			if ((bool)RenderTextureSpace)
			{
				UnityEngine.Object.Destroy(RenderTextureSpace.gameObject);
			}
		}

		public virtual void RemoveFromLayer()
		{
			if ((bool)Layer)
			{
				Layer.Remove(this);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(1);
			writer.WriteLocalTransform(base.transform);
			writer.WriteBuffer(EffectColor.Write);
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				AdvITweenPlayer.WriteSaveData(x, base.gameObject);
			});
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				AdvAnimationPlayer.WriteSaveData(x, base.gameObject);
			});
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				TargetObject.Write(x);
			});
		}

		public void Read(byte[] buffer, AdvGraphicInfo graphic)
		{
			TargetObject.gameObject.SetActive(false);
			Loader.LoadGraphic(graphic, delegate
			{
				TargetObject.gameObject.SetActive(true);
				SetGraphicOnSaveDataRead(graphic);
				BinaryUtil.BinaryRead(buffer, Read);
			});
		}

		private void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 1)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			reader.ReadLocalTransform(base.transform);
			reader.ReadBuffer(EffectColor.Read);
			reader.ReadBuffer(delegate(BinaryReader x)
			{
				AdvITweenPlayer.ReadSaveData(x, base.gameObject, true, PixelsToUnits);
			});
			reader.ReadBuffer(delegate(BinaryReader x)
			{
				AdvAnimationPlayer.ReadSaveData(x, base.gameObject, Engine);
			});
			if (num > 0)
			{
				reader.ReadBuffer(delegate(BinaryReader x)
				{
					TargetObject.Read(x);
				});
			}
		}

		internal void InitCaptureImage(AdvGraphicInfo grapic, Camera cachedCamera)
		{
			LastResource = grapic;
			base.gameObject.GetComponentInChildren<AdvGraphicObjectRawImage>().CaptureCamera(cachedCamera);
		}

		private void SetGraphicOnSaveDataRead(AdvGraphicInfo graphic)
		{
			DrawSub(graphic, 0f);
		}
	}
}
