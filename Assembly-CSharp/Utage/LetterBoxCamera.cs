using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Camera/LetterBoxCamera")]
	public class LetterBoxCamera : MonoBehaviour
	{
		public enum AnchorType
		{
			UpperLeft,
			UpperCenter,
			UpperRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			LowerLeft,
			LowerCenter,
			LowerRight
		}

		[SerializeField]
		private int pixelsToUnits = 100;

		[SerializeField]
		private int width = 800;

		[SerializeField]
		private int height = 600;

		[SerializeField]
		private bool isFlexible;

		[SerializeField]
		private int maxWidth = 800;

		[SerializeField]
		private int maxHeight = 600;

		[SerializeField]
		private AnchorType anchor = AnchorType.MiddleCenter;

		public LetterBoxCameraEvent OnGameScreenSizeChange = new LetterBoxCameraEvent();

		private float screenAspectRatio;

		private Vector2 padding;

		private Vector2 currentSize;

		[SerializeField]
		public float zoom2D = 1f;

		[SerializeField]
		public Vector2 zoom2DCenter;

		private Camera cachedCamera;

		private bool hasChanged = true;

		private const int Version = 0;

		public int PixelsToUnits
		{
			get
			{
				return pixelsToUnits;
			}
			set
			{
				hasChanged = true;
				pixelsToUnits = value;
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				hasChanged = true;
				width = value;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				hasChanged = true;
				height = value;
			}
		}

		public bool IsFlexible
		{
			get
			{
				return isFlexible;
			}
			set
			{
				hasChanged = true;
				isFlexible = value;
			}
		}

		public int MaxWidth
		{
			get
			{
				return maxWidth;
			}
			set
			{
				hasChanged = true;
				maxWidth = value;
			}
		}

		public int MaxHeight
		{
			get
			{
				return maxHeight;
			}
			set
			{
				hasChanged = true;
				maxHeight = value;
			}
		}

		public int FlexibleMinWidth
		{
			get
			{
				if (!IsFlexible)
				{
					return Width;
				}
				return Mathf.Min(Width, Width, MaxWidth);
			}
		}

		public int FlexibleMinHeight
		{
			get
			{
				if (!IsFlexible)
				{
					return Height;
				}
				return Mathf.Min(Height, Height, MaxHeight);
			}
		}

		public int FlexibleMaxWidth
		{
			get
			{
				if (!IsFlexible)
				{
					return Width;
				}
				return Mathf.Max(Width, Width, MaxWidth);
			}
		}

		public int FlexibleMaxHeight
		{
			get
			{
				if (!IsFlexible)
				{
					return Height;
				}
				return Mathf.Max(Height, Height, MaxHeight);
			}
		}

		public Vector2 CurrentSize
		{
			get
			{
				if (hasChanged)
				{
					RefreshCurrentSize();
				}
				return currentSize;
			}
		}

		public Camera CachedCamera
		{
			get
			{
				if (cachedCamera == null)
				{
					cachedCamera = GetComponent<Camera>();
				}
				return cachedCamera;
			}
		}

		public float Zoom2D
		{
			get
			{
				return zoom2D;
			}
			set
			{
				zoom2D = value;
				hasChanged = true;
			}
		}

		public Vector2 Zoom2DCenter
		{
			get
			{
				return zoom2DCenter;
			}
			set
			{
				zoom2DCenter = value;
				hasChanged = true;
			}
		}

		internal void SetZoom2D(float zoom, Vector2 center)
		{
			Zoom2D = zoom;
			Zoom2DCenter = center;
		}

		private void Start()
		{
			hasChanged = true;
		}

		private void OnValidate()
		{
			hasChanged = true;
		}

		private void Update()
		{
			if (hasChanged || !Mathf.Approximately(screenAspectRatio, 1f * (float)Screen.width / (float)Screen.height))
			{
				Refresh();
			}
		}

		public void Refresh()
		{
			hasChanged = false;
			RefreshCurrentSize();
			RefreshCamera();
		}

		private void RefreshCurrentSize()
		{
			if (TryRefreshCurrentSize())
			{
				OnGameScreenSizeChange.Invoke(this);
			}
		}

		private bool TryRefreshCurrentSize()
		{
			screenAspectRatio = 1f * (float)Screen.width / (float)Screen.height;
			float b = (float)Width / (float)Height;
			float num = (float)FlexibleMaxWidth / (float)FlexibleMinHeight;
			float num2 = (float)FlexibleMinWidth / (float)FlexibleMaxHeight;
			int num3;
			int num4;
			if (screenAspectRatio > num)
			{
				padding.x = (1f - num / screenAspectRatio) / 2f;
				padding.y = 0f;
				num3 = FlexibleMaxWidth;
				num4 = FlexibleMinHeight;
			}
			else if (screenAspectRatio < num2)
			{
				padding.x = 0f;
				padding.y = (1f - screenAspectRatio / num2) / 2f;
				num3 = FlexibleMinWidth;
				num4 = FlexibleMaxHeight;
			}
			else
			{
				padding.x = 0f;
				padding.y = 0f;
				if (Mathf.Approximately(screenAspectRatio, b))
				{
					num3 = Width;
					num4 = Height;
				}
				else
				{
					num4 = FlexibleMinHeight;
					num3 = Mathf.FloorToInt(screenAspectRatio * (float)num4);
					if (num3 < FlexibleMinWidth)
					{
						num3 = FlexibleMinWidth;
						num4 = Mathf.FloorToInt((float)num3 / screenAspectRatio);
					}
				}
			}
			bool result = currentSize.x != (float)num3 || currentSize.y != (float)num4;
			currentSize = new Vector2(num3, num4);
			return result;
		}

		private void RefreshCamera()
		{
			float x = padding.x;
			float num = 1f - padding.x * 2f;
			float y = padding.y;
			float num2 = 1f - padding.y * 2f;
			switch (anchor)
			{
			case AnchorType.UpperLeft:
				x = 0f;
				y = padding.y * 2f;
				break;
			case AnchorType.UpperCenter:
				y = padding.y * 2f;
				break;
			case AnchorType.UpperRight:
				x = padding.x * 2f;
				y = padding.y * 2f;
				break;
			case AnchorType.MiddleLeft:
				x = 0f;
				break;
			case AnchorType.MiddleRight:
				x = padding.x * 2f;
				break;
			case AnchorType.LowerLeft:
				x = 0f;
				y = 0f;
				break;
			case AnchorType.LowerCenter:
				y = 0f;
				break;
			case AnchorType.LowerRight:
				x = padding.x * 2f;
				y = 0f;
				break;
			}
			Rect rect = new Rect(x, y, num, num2);
			CachedCamera.orthographicSize = CurrentSize.y / (float)(2 * pixelsToUnits) / Zoom2D;
			CachedCamera.rect = rect;
			Vector2 vector = (-1f / Zoom2D + 1f) * Zoom2DCenter / pixelsToUnits;
			CachedCamera.transform.localPosition = vector;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(Zoom2D);
			writer.Write(Zoom2DCenter);
		}

		public void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
			else
			{
				Zoom2D = reader.ReadSingle();
				Zoom2DCenter = reader.ReadVector2();
			}
		}

		internal void OnClear()
		{
			Zoom2D = 1f;
			Zoom2DCenter = Vector2.zero;
		}
	}
}
