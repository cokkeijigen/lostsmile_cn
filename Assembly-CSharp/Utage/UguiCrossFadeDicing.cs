using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/CrossFadeDicing")]
	public class UguiCrossFadeDicing : UguiCrossFadeRawImage
	{
		private DicingTextureData fadePatternData;

		public override Graphic Target
		{
			get
			{
				return target ?? (target = GetComponent<DicingImage>());
			}
		}

		internal void CrossFade(DicingTextureData fadePatternData, Texture fadeTexture, float time, Action onComplete)
		{
			this.fadePatternData = fadePatternData;
			Target.SetAllDirty();
			CrossFade(fadeTexture, time, onComplete);
		}

		public override void RebuildVertex(VertexHelper vh)
		{
			if (fadePatternData == null)
			{
				return;
			}
			vh.Clear();
			Rect pixelAdjustedRect = Target.GetPixelAdjustedRect();
			Color color = Target.color;
			DicingImage dicingImage = Target as DicingImage;
			float num = pixelAdjustedRect.width / (float)fadePatternData.Width;
			float num2 = pixelAdjustedRect.height / (float)fadePatternData.Height;
			int num3 = 0;
			List<DicingTextureData.QuadVerts> verts = dicingImage.GetVerts(dicingImage.PatternData);
			List<DicingTextureData.QuadVerts> verts2 = dicingImage.GetVerts(fadePatternData);
			int num4 = verts.Count;
			if (num4 != verts2.Count)
			{
				num4 = Mathf.Min(num4, verts2.Count);
				Debug.LogError(string.Format("Not equal texture size {0} and {1}", dicingImage.PatternData.Name, fadePatternData.Name));
			}
			for (int i = 0; i < num4; i++)
			{
				DicingTextureData.QuadVerts quadVerts = verts[i];
				DicingTextureData.QuadVerts quadVerts2 = verts2[i];
				if (!dicingImage.SkipTransParentCell || !quadVerts.isAllTransparent || !quadVerts2.isAllTransparent)
				{
					Vector4 vector = new Vector4(pixelAdjustedRect.x + num * quadVerts.v.x, pixelAdjustedRect.y + num2 * quadVerts.v.y, pixelAdjustedRect.x + num * quadVerts.v.z, pixelAdjustedRect.y + num2 * quadVerts.v.w);
					Rect uvRect = quadVerts.uvRect;
					Rect uvRect2 = quadVerts2.uvRect;
					vh.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(uvRect.xMin, uvRect.yMin), new Vector2(uvRect2.xMin, uvRect2.yMin), Vector3.zero, Vector4.zero);
					vh.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(uvRect.xMin, uvRect.yMax), new Vector2(uvRect2.xMin, uvRect2.yMax), Vector3.zero, Vector4.zero);
					vh.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(uvRect.xMax, uvRect.yMax), new Vector2(uvRect2.xMax, uvRect2.yMax), Vector3.zero, Vector4.zero);
					vh.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(uvRect.xMax, uvRect.yMin), new Vector2(uvRect2.xMax, uvRect2.yMin), Vector3.zero, Vector4.zero);
					vh.AddTriangle(num3, num3 + 1, num3 + 2);
					vh.AddTriangle(num3 + 2, num3 + 3, num3);
					num3 += 4;
				}
			}
		}
	}
}
