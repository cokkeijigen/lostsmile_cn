using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UguiMessageWindowFaceIcon")]
	public class AdvUguiMessageWindowFaceIcon : MonoBehaviour
	{
		protected enum IconGraphicType
		{
			Default,
			Dicing,
			RenderTexture,
			NotSupport
		}

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected bool autoHideIconCharacterOff;

		protected AdvGraphicObject targetObject;

		protected GameObject iconRoot;

		public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

		public bool AutoHideIconCharacterOff
		{
			get
			{
				return autoHideIconCharacterOff;
			}
			set
			{
				autoHideIconCharacterOff = value;
			}
		}

		protected virtual void Awake()
		{
			Engine.Page.OnChangeText.AddListener(OnChangeText);
			Engine.MessageWindowManager.OnReset.AddListener(OnReset);
		}

		protected virtual void Update()
		{
			if (targetObject == null && autoHideIconCharacterOff)
			{
				HideIcon();
			}
		}

		protected virtual void OnReset(AdvMessageWindowManager window)
		{
			if (iconRoot != null)
			{
				Object.Destroy(iconRoot);
				iconRoot = null;
			}
		}

		protected virtual void OnChangeText(AdvPage page)
		{
			if (!TyrSetIcon(page))
			{
				targetObject = null;
				HideIcon();
			}
		}

		protected virtual void HideIcon()
		{
			if (iconRoot != null && iconRoot.activeSelf)
			{
				iconRoot.SetActive(false);
			}
		}

		protected virtual bool TyrSetIcon(AdvPage page)
		{
			targetObject = null;
			AdvCharacterInfo characterInfo = page.CharacterInfo;
			if (characterInfo == null || characterInfo.Graphic == null || characterInfo.Graphic.Main == null)
			{
				return false;
			}
			AdvGraphicInfo main = characterInfo.Graphic.Main;
			if (!(main.SettingData is AdvCharacterSettingData advCharacterSettingData))
			{
				return false;
			}
			AdvCharacterSettingData.IconInfo icon = advCharacterSettingData.Icon;
			if (icon.IconType == AdvCharacterSettingData.IconInfo.Type.None)
			{
				return false;
			}
			targetObject = Engine.GraphicManager.FindObject(characterInfo.Label);
			switch (icon.IconType)
			{
			case AdvCharacterSettingData.IconInfo.Type.IconImage:
				SetIconImage(icon.File);
				return true;
			case AdvCharacterSettingData.IconInfo.Type.DicingPattern:
				SetIconDicingPattern(icon.File, icon.IconSubFileName);
				return true;
			case AdvCharacterSettingData.IconInfo.Type.RectImage:
				switch (ParseIconGraphicType(main, characterInfo.Label))
				{
				case IconGraphicType.Default:
					SetIconRectImage(main, icon.IconRect);
					return true;
				case IconGraphicType.Dicing:
					SetIconDicing(main, icon.IconRect);
					return true;
				case IconGraphicType.RenderTexture:
					SetIconRenderTexture(icon.IconRect);
					return true;
				default:
					return false;
				}
			default:
				return false;
			}
		}

		protected virtual void SetIconImage(AssetFile file)
		{
			AssetFileManager.Load(file, delegate(AssetFile x)
			{
				RawImage rawImage = ChangeIconComponent<RawImage>();
				rawImage.material = null;
				Texture2D texture = x.Texture;
				rawImage.texture = texture;
				rawImage.uvRect = new Rect(0f, 0f, 1f, 1f);
				ChangeReference(file, rawImage.gameObject);
			});
		}

		protected virtual void SetIconDicingPattern(AssetFile file, string pattern)
		{
			DicingImage dicingImage = ChangeIconComponent<DicingImage>();
			DicingTextures dicingData = file.UnityObject as DicingTextures;
			dicingImage.DicingData = dicingData;
			dicingImage.ChangePattern(pattern);
			dicingImage.UvRect = new Rect(0f, 0f, 1f, 1f);
			ChangeReference(file, dicingImage.gameObject);
		}

		protected virtual IconGraphicType ParseIconGraphicType(AdvGraphicInfo info, string characterLabel)
		{
			string fileType = info.FileType;
			switch (fileType)
			{
			default:
				if (fileType.Length != 0)
				{
					break;
				}
				goto case "2D";
			case "Dicing":
				return IconGraphicType.Dicing;
			case "2D":
				return IconGraphicType.Default;
			case null:
				break;
			}
			AdvGraphicObject advGraphicObject = Engine.GraphicManager.FindObject(characterLabel);
			if (advGraphicObject != null && advGraphicObject.EnableRenderTexture)
			{
				return IconGraphicType.RenderTexture;
			}
			return IconGraphicType.NotSupport;
		}

		protected virtual void SetIconRectImage(AdvGraphicInfo graphic, Rect rect)
		{
			RawImage rawImage = ChangeIconComponent<RawImage>();
			rawImage.material = null;
			Texture2D texture2D = (Texture2D)(rawImage.texture = graphic.File.Texture);
			float w = texture2D.width;
			float h = texture2D.height;
			rawImage.uvRect = rect.ToUvRect(w, h);
			ChangeReference(graphic.File, rawImage.gameObject);
		}

		protected virtual void SetIconDicing(AdvGraphicInfo graphic, Rect rect)
		{
			DicingImage dicingImage = ChangeIconComponent<DicingImage>();
			DicingTextures dicingData = graphic.File.UnityObject as DicingTextures;
			string subFileName = graphic.SubFileName;
			dicingImage.DicingData = dicingData;
			dicingImage.ChangePattern(subFileName);
			float w = dicingImage.PatternData.Width;
			float h = dicingImage.PatternData.Height;
			dicingImage.UvRect = rect.ToUvRect(w, h);
			ChangeReference(graphic.File, dicingImage.gameObject);
		}

		protected virtual void SetIconRenderTexture(Rect rect)
		{
			AdvGraphicObject advGraphicObject = targetObject;
			if (!(advGraphicObject.RenderTextureSpace == null))
			{
				RawImage rawImage = ChangeIconComponent<RawImage>();
				if (advGraphicObject.RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image)
				{
					rawImage.material = new Material(ShaderManager.DrawByRenderTexture);
				}
				Texture texture = (rawImage.texture = advGraphicObject.RenderTextureSpace.RenderTexture);
				float w = texture.width;
				float h = texture.height;
				Transform obj = advGraphicObject.TargetObject.transform;
				float x = obj.localScale.x;
				float y = obj.localScale.y;
				rect.position = new Vector2(rect.position.x * x, rect.position.y * y);
				rect.size = new Vector2(rect.size.x * x, rect.size.y * y);
				rawImage.uvRect = rect.ToUvRect(w, h);
			}
		}

		protected virtual T ChangeIconComponent<T>() where T : Component
		{
			T val = null;
			if (iconRoot != null)
			{
				val = iconRoot.GetComponent<T>();
				if ((Object)val != (Object)null)
				{
					iconRoot.SetActive(true);
					return val;
				}
			}
			if (iconRoot != null)
			{
				Object.Destroy(iconRoot);
			}
			val = base.transform.AddChildGameObjectComponent<T>("Icon");
			iconRoot = val.gameObject;
			RectTransform rectTransform = iconRoot.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetStretch();
			}
			return val;
		}

		protected virtual void ChangeReference(AssetFile file, GameObject go)
		{
			go.RemoveComponents<AssetFileReference>();
			file.AddReferenceComponent(go);
		}
	}
}
