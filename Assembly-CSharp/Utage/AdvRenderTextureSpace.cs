using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/RenderTextureSpace")]
	public class AdvRenderTextureSpace : MonoBehaviour
	{
		public RenderTexture RenderTexture { get; private set; }

		private Camera RenderCamera { get; set; }

		private Canvas Canvas { get; set; }

		private CanvasScaler CanvasScaler { get; set; }

		public GameObject RenderRoot { get; private set; }

		private AdvRenderTextureSetting Setting { get; set; }

		public AdvRenderTextureMode RenderTextureType
		{
			get
			{
				return Setting.RenderTextureType;
			}
		}

		internal void Init(AdvGraphicInfo graphic, float pixelsToUnits)
		{
			Setting = graphic.RenderTextureSetting;
			CreateCamera(pixelsToUnits);
			CreateTexture();
			CreateRoot(graphic, pixelsToUnits);
		}

		private void OnDestroy()
		{
			if ((bool)RenderTexture)
			{
				RenderTexture.Release();
				Object.Destroy(RenderTexture);
			}
		}

		private void CreateCamera(float pixelsToUnits)
		{
			RenderCamera = base.gameObject.AddComponent<Camera>();
			RenderCamera.gameObject.layer = base.gameObject.layer;
			RenderCamera.cullingMask = 1 << base.gameObject.layer;
			RenderCamera.depth = -100f;
			RenderCamera.clearFlags = CameraClearFlags.Color;
			RenderCamera.backgroundColor = ((RenderTextureType == AdvRenderTextureMode.Image) ? new Color(0f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 0f));
			RenderCamera.orthographic = true;
			RenderCamera.orthographicSize = Setting.RenderTextureSize.y / pixelsToUnits / 2f;
		}

		private void CreateTexture()
		{
			int width = (int)Setting.RenderTextureSize.x;
			int height = (int)Setting.RenderTextureSize.y;
			RenderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
			RenderCamera.targetTexture = RenderTexture;
		}

		private void CreateRoot(AdvGraphicInfo graphic, float pixelsToUnits)
		{
			if (graphic.IsUguiComponentType)
			{
				CreateCanvas();
				return;
			}
			RenderRoot = RenderCamera.transform.AddChildGameObject("Root");
			RenderRoot.transform.localPosition = Setting.RenderTextureOffset / pixelsToUnits;
			RenderRoot.transform.localScale = graphic.Scale;
		}

		private void CreateCanvas()
		{
			GameObject gameObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
			RenderCamera.transform.AddChild(gameObject);
			Canvas = gameObject.GetComponent<Canvas>();
			Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
			RenderCamera.nearClipPlane = -1f;
			Canvas.renderMode = RenderMode.ScreenSpaceCamera;
			Canvas.worldCamera = RenderCamera;
			CanvasScaler = Canvas.gameObject.AddComponent<CanvasScaler>();
			CanvasScaler.referenceResolution = Setting.RenderTextureSize;
			CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			CanvasScaler.scaleFactor = 1f;
			CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			RenderRoot = Canvas.transform.AddChildGameObjectComponent<RectTransform>("Root").gameObject;
		}

		private void Update()
		{
			if (!RenderTexture.IsCreated())
			{
				RenderTexture.Create();
			}
		}
	}
}
