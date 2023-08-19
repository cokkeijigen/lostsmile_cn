using System;
using UnityEngine;

namespace Utage
{
	public class AdvRenderTextureSetting
	{
		public AdvRenderTextureMode RenderTextureType { get; protected set; }

		public Vector2 RenderTextureSize { get; protected set; }

		public Vector3 RenderTextureOffset { get; protected set; }

		public bool EnableRenderTexture
		{
			get
			{
				return RenderTextureType != AdvRenderTextureMode.None;
			}
		}

		public Material GetRenderMaterialIfEnable(Material material)
		{
			if (EnableRenderTexture && (material == null || material.shader != ShaderManager.RenderTexture))
			{
				return new Material(ShaderManager.RenderTexture);
			}
			return material;
		}

		public void Parse(StringGridRow row)
		{
			RenderTextureType = AdvParser.ParseCellOptional(row, AdvColumnName.RenderTexture, AdvRenderTextureMode.None);
			if (RenderTextureType == AdvRenderTextureMode.None)
			{
				return;
			}
			try
			{
				float[] array = row.ParseCellArray<float>(AdvColumnName.RenderRect.QuickToString());
				if (array.Length != 4)
				{
					Debug.LogError(row.ToErrorString("IconRect. Array size is not 4"));
					return;
				}
				RenderTextureOffset = new Vector3(0f - array[0], 0f - array[1], 1000f);
				RenderTextureSize = new Vector2(array[2], array[3]);
			}
			catch (Exception)
			{
			}
		}
	}
}
