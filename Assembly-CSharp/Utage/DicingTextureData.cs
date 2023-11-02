using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class DicingTextureData
	{
		public class QuadVerts
		{
			public Vector4 v;

			public Rect uvRect;

			public bool isAllTransparent;
		}

		[SerializeField]
		private string name = "";

		[SerializeField]
		private string atlasName = "";

		[SerializeField]
		private int width;

		[SerializeField]
		private int height;

		[SerializeField]
		private List<int> cellIndexList = new List<int>();

		[SerializeField]
		private int transparentIndex;

		[NonSerialized]
		private List<QuadVerts> verts;

		public string Name => name;

		public string AtlasName => atlasName;

		public int Width => width;

		public int Height => height;

		internal List<QuadVerts> GetVerts(DicingTextures textures)
		{
			if (verts == null)
			{
				InitVerts(textures);
			}
			return verts;
		}

		private void InitVerts(DicingTextures atlas)
		{
			if (atlas == null)
			{
				return;
			}
			verts = new List<QuadVerts>();
			int cellSize = atlas.CellSize;
			int num = cellSize - atlas.Padding * 2;
			int num2 = Mathf.CeilToInt(1f * (float)Width / (float)num);
			int num3 = Mathf.CeilToInt(1f * (float)Height / (float)num);
			int num4 = atlas.GetTexture(AtlasName).width;
			int num5 = atlas.GetTexture(AtlasName).height;
			int num6 = Mathf.CeilToInt(1f * (float)num4 / (float)cellSize);
			int num7 = 0;
			for (int i = 0; i < num3; i++)
			{
				float num8 = i * num;
				float num9 = Mathf.Min(num8 + (float)num, Height);
				for (int j = 0; j < num2; j++)
				{
					QuadVerts quadVerts = new QuadVerts();
					float num10 = j * num;
					float num11 = Mathf.Min(num10 + (float)num, Width);
					quadVerts.v = new Vector4(num10, num8, num11, num9);
					int num12 = cellIndexList[num7];
					quadVerts.isAllTransparent = num12 == transparentIndex;
					float num13 = num12 % num6 * cellSize;
					float num14 = num12 / num6 * cellSize;
					float x = 1f * (num13 + (float)atlas.Padding) / (float)num4;
					float y = 1f * (num14 + (float)atlas.Padding) / (float)num5;
					float num15 = 1f * (num11 - num10) / (float)num4;
					float num16 = 1f * (num9 - num8) / (float)num5;
					quadVerts.uvRect = new Rect(x, y, num15, num16);
					verts.Add(quadVerts);
					num7++;
				}
			}
		}

		public void ForeachVertexList(Rect position, Rect uvRect, bool skipTransParentCell, DicingTextures textures, Action<Rect, Rect> function)
		{
			Vector2 scale = new Vector2(position.width / (float)Width, position.height / (float)Height);
			ForeachVertexList(uvRect, skipTransParentCell, textures, delegate(Rect r1, Rect r2)
			{
				r1.xMin *= scale.x;
				r1.xMax *= scale.x;
				r1.x += position.x;
				r1.yMin *= scale.y;
				r1.yMax *= scale.y;
				r1.y += position.y;
				function(r1, r2);
			});
		}

		public void ForeachVertexList(Rect uvRect, bool skipTransParentCell, DicingTextures textures, Action<Rect, Rect> function)
		{
			if (uvRect.width == 0f || uvRect.height == 0f)
			{
				return;
			}
			if (uvRect.xMin < 0f)
			{
				uvRect.x += Mathf.CeilToInt(0f - uvRect.xMin);
			}
			if (uvRect.yMin < 0f)
			{
				uvRect.y += Mathf.CeilToInt(0f - uvRect.yMin);
			}
			bool flag = false;
			if (uvRect.width < 0f)
			{
				uvRect.width *= -1f;
				flag = true;
			}
			bool flag2 = false;
			if (uvRect.height < 0f)
			{
				uvRect.height *= -1f;
				flag2 = true;
			}
			float scaleX = 1f / uvRect.width;
			float fipOffsetX = 0f;
			if (flag)
			{
				scaleX *= -1f;
				fipOffsetX = Width;
			}
			float scaleY = 1f / uvRect.height;
			float fipOffsetY = 0f;
			if (flag2)
			{
				scaleY *= -1f;
				fipOffsetY = Height;
			}
			float num = uvRect.yMin % 1f;
			float num2 = uvRect.yMax % 1f;
			if (num2 == 0f)
			{
				num2 = 1f;
			}
			float offsetY = 0f;
			bool flag3 = true;
			bool flag4 = false;
			Rect rect = default(Rect);
			do
			{
				rect.yMin = (flag3 ? num : 0f);
				flag4 = offsetY + 1f - rect.yMin >= uvRect.height;
				rect.yMax = (flag4 ? num2 : 1f);
				float num3 = uvRect.xMin % 1f;
				float num4 = uvRect.xMax % 1f;
				if (num4 == 0f)
				{
					num4 = 1f;
				}
				float offsetX = 0f;
				bool flag5 = true;
				bool flag6 = false;
				do
				{
					rect.xMin = (flag5 ? num3 : 0f);
					flag6 = offsetX + 1f - rect.xMin >= uvRect.width;
					rect.xMax = (flag6 ? num4 : 1f);
					ForeachVertexListSub(rect, skipTransParentCell, textures, delegate(Rect r1, Rect r2)
					{
						r1.xMin *= scaleX;
						r1.xMax *= scaleX;
						r1.x += (offsetX - rect.xMin) * scaleX * (float)Width + fipOffsetX;
						r1.yMin *= scaleY;
						r1.yMax *= scaleY;
						r1.y += (offsetY - rect.yMin) * scaleY * (float)Height + fipOffsetY;
						function(r1, r2);
					});
					offsetX += rect.width;
					flag5 = false;
				}
				while (!flag6);
				offsetY += rect.height;
				flag3 = false;
			}
			while (!flag4);
		}

		private void ForeachVertexListSub(Rect uvRect, bool skipTransParentCell, DicingTextures textures, Action<Rect, Rect> function)
		{
			Texture2D texture = textures.GetTexture(AtlasName);
			float num = texture.width;
			float num2 = texture.height;
			List<QuadVerts> list = GetVerts(textures);
			Rect rect = new Rect(uvRect.x * (float)Width, uvRect.y * (float)Height, uvRect.width * (float)Width, uvRect.height * (float)Height);
			for (int i = 0; i < list.Count; i++)
			{
				QuadVerts quadVerts = list[i];
				if (skipTransParentCell && quadVerts.isAllTransparent)
				{
					continue;
				}
				float x = quadVerts.v.x;
				float num3 = quadVerts.v.z;
				float y = quadVerts.v.y;
				float num4 = quadVerts.v.w;
				Rect uvRect2 = quadVerts.uvRect;
				if (!(x > rect.xMax) && !(y > rect.yMax) && !(num3 < rect.x) && !(num4 < rect.y))
				{
					if (x < rect.x)
					{
						uvRect2.xMin += (rect.x - x) / num;
						x = rect.x;
					}
					if (num3 > rect.xMax)
					{
						uvRect2.xMax += (rect.xMax - num3) / num;
						num3 = rect.xMax;
					}
					if (y < rect.y)
					{
						uvRect2.yMin += (rect.y - y) / num2;
						y = rect.y;
					}
					if (num4 > rect.yMax)
					{
						uvRect2.yMax += (rect.yMax - num4) / num2;
						num4 = rect.yMax;
					}
					function(new Rect(x, y, num3 - x, num4 - y), uvRect2);
				}
			}
		}
	}
}
