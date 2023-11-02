using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/LoadGraphicFile")]
	public class AdvUguiLoadGraphicFile : MonoBehaviour
	{
		public enum SizeSetting
		{
			RectSize,
			TextureSize,
			GraphicSize
		}

		private AdvGraphicLoader loader;

		[SerializeField]
		private SizeSetting sizeSetting;

		public UnityEvent OnLoadEnd;

		public AdvGraphicLoader Loader => this.GetComponentCacheCreateIfMissing(ref loader);

		protected virtual Graphic GraphicComponent { get; set; }

		protected virtual AssetFile File { get; set; }

		protected virtual AdvGraphicInfo GraphicInfo { get; set; }

		public SizeSetting RectSizeSetting
		{
			get
			{
				return sizeSetting;
			}
			set
			{
				sizeSetting = value;
			}
		}

		public virtual void LoadFile(AdvGraphicInfo graphic)
		{
			GraphicInfo = graphic;
			Loader.LoadGraphic(graphic, delegate
			{
				string fileType = graphic.FileType;
				if (!(fileType == "2D") && (fileType == null || fileType.Length != 0))
				{
					if (fileType == "Dicing")
					{
						DicingImage dicingImage = ChangeGraphicComponent<DicingImage>();
						dicingImage.DicingData = graphic.File.UnityObject as DicingTextures;
						string subFileName = graphic.SubFileName;
						dicingImage.ChangePattern(subFileName);
						InitSize(new Vector2(dicingImage.PatternData.Width, dicingImage.PatternData.Height));
					}
					else
					{
						Debug.LogError(graphic.FileType + " is not support ");
					}
				}
				else
				{
					RawImage rawImage = ChangeGraphicComponent<RawImage>();
					rawImage.texture = graphic.File.Texture;
					InitSize(new Vector2(rawImage.texture.width, rawImage.texture.height));
				}
				OnLoadEnd.Invoke();
			});
		}

		public virtual void LoadTextureFile(string path)
		{
			ClearFile();
			File = AssetFileManager.Load(path, this);
			File.AddReferenceComponent(base.gameObject);
			File.Unuse(this);
			StartCoroutine(CoWaitTextureFileLoading());
		}

		protected virtual IEnumerator CoWaitTextureFileLoading()
		{
			while (!File.IsLoadEnd)
			{
				yield return null;
			}
			if (File.Texture != null)
			{
				RawImage rawImage = ChangeGraphicComponent<RawImage>();
				rawImage.texture = File.Texture;
				InitSize(new Vector2(rawImage.texture.width, rawImage.texture.height));
			}
			OnLoadEnd.Invoke();
		}

		protected virtual T ChangeGraphicComponent<T>() where T : Graphic
		{
			if (GraphicComponent == null)
			{
				GraphicComponent = GetComponent<Graphic>();
			}
			if (GraphicComponent != null)
			{
				if (GraphicComponent is T)
				{
					return GraphicComponent as T;
				}
				Object.DestroyImmediate(GraphicComponent);
			}
			GraphicComponent = base.gameObject.AddComponent<T>();
			return GraphicComponent as T;
		}

		protected virtual void InitSize(Vector2 resouceSize)
		{
			switch (RectSizeSetting)
			{
			case SizeSetting.TextureSize:
				(GraphicComponent.transform as RectTransform).SetSize(resouceSize.x, resouceSize.y);
				break;
			case SizeSetting.GraphicSize:
			{
				if (GraphicInfo == null)
				{
					Debug.LogError("graphic is null");
					break;
				}
				float width = resouceSize.x * GraphicInfo.Scale.x;
				float height = resouceSize.y * GraphicInfo.Scale.y;
				(GraphicComponent.transform as RectTransform).SetSize(width, height);
				break;
			}
			case SizeSetting.RectSize:
				break;
			}
		}

		public virtual void ClearFile()
		{
			GraphicComponent.RemoveComponentMySelf();
			GraphicComponent = null;
			base.gameObject.RemoveComponent<AssetFileReference>();
			File = null;
			Loader.Unload();
			GraphicInfo = null;
		}
	}
}
