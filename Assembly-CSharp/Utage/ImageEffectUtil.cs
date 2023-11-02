using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public static class ImageEffectUtil
	{
		private class ImageEffectPattern
		{
			public string type;

			public Type componentType;

			public Shader[] shaders;

			internal ImageEffectPattern(string type, Type componentType, Shader[] shaders)
			{
				this.type = type;
				this.componentType = componentType;
				this.shaders = shaders;
			}
		}

		private static List<ImageEffectPattern> patterns = new List<ImageEffectPattern>
		{
			new ImageEffectPattern(ImageEffectType.ColorFade.ToString(), typeof(ColorFade), new Shader[1] { Shader.Find(ShaderManager.ColorFade) }),
			new ImageEffectPattern(ImageEffectType.Bloom.ToString(), typeof(Bloom), new Shader[1] { Shader.Find(ShaderManager.BloomName) }),
			new ImageEffectPattern(ImageEffectType.Blur.ToString(), typeof(Blur), new Shader[1] { Shader.Find(ShaderManager.BlurName) }),
			new ImageEffectPattern(ImageEffectType.Mosaic.ToString(), typeof(Mosaic), new Shader[1] { Shader.Find(ShaderManager.MosaicName) }),
			new ImageEffectPattern(ImageEffectType.GrayScale.ToString(), typeof(Grayscale), new Shader[1] { Shader.Find(ShaderManager.GrayScaleName) }),
			new ImageEffectPattern(ImageEffectType.MotionBlur.ToString(), typeof(MotionBlur), new Shader[1] { Shader.Find(ShaderManager.MotionBlurName) }),
			new ImageEffectPattern(ImageEffectType.ScreenOverlay.ToString(), typeof(ScreenOverlay), new Shader[1] { Shader.Find(ShaderManager.BlendModesOverlayName) }),
			new ImageEffectPattern(ImageEffectType.Sepia.ToString(), typeof(SepiaTone), new Shader[1] { Shader.Find(ShaderManager.SepiatoneName) }),
			new ImageEffectPattern(ImageEffectType.NegaPosi.ToString(), typeof(NegaPosi), new Shader[1] { Shader.Find(ShaderManager.NegaPosiName) }),
			new ImageEffectPattern(ImageEffectType.FishEye.ToString(), typeof(FishEye), new Shader[1] { Shader.Find(ShaderManager.FisheyeName) }),
			new ImageEffectPattern(ImageEffectType.Twirl.ToString(), typeof(Twirl), new Shader[1] { Shader.Find(ShaderManager.TwirlName) }),
			new ImageEffectPattern(ImageEffectType.Vortex.ToString(), typeof(Vortex), new Shader[1] { Shader.Find(ShaderManager.VortexName) })
		};

		public static bool SupportsImageEffects => SystemInfo.supportsImageEffects;

		public static bool SupportsRenderTextures => true;

		public static bool SupportsDepth => SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);

		public static bool SupportsHDRTextures => SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);

		public static bool SupportDX11
		{
			get
			{
				if (SystemInfo.graphicsShaderLevel >= 50)
				{
					return SystemInfo.supportsComputeShaders;
				}
				return false;
			}
		}

		internal static bool TryParse(string type, out Type ComponentType, out Shader[] Shaders)
		{
			ImageEffectPattern imageEffectPattern = patterns.Find((ImageEffectPattern x) => x.type == type);
			if (imageEffectPattern == null)
			{
				ComponentType = null;
				Shaders = null;
				return false;
			}
			ComponentType = imageEffectPattern.componentType;
			Shaders = imageEffectPattern.shaders;
			return true;
		}

		internal static string ToImageEffectType(Type ComponentType)
		{
			ImageEffectPattern imageEffectPattern = patterns.Find((ImageEffectPattern x) => x.componentType == ComponentType);
			if (imageEffectPattern == null)
			{
				return "";
			}
			return imageEffectPattern.type;
		}

		internal static bool TryGetComonentCreateIfMissing(string type, out ImageEffectBase component, out bool alreadyEnabled, GameObject target)
		{
			alreadyEnabled = false;
			if (!TryParse(type, out var ComponentType, out var Shaders))
			{
				Debug.LogError(type + " is not find in Image effect patterns");
				component = null;
				return false;
			}
			component = target.GetComponent(ComponentType) as ImageEffectBase;
			if (component == null)
			{
				component = target.gameObject.AddComponent(ComponentType) as ImageEffectBase;
				component.SetShaders(Shaders);
			}
			else
			{
				alreadyEnabled = component.enabled;
			}
			return true;
		}

		public static void RenderDistortion(Material material, RenderTexture source, RenderTexture destination, float angle, Vector2 center, Vector2 radius)
		{
			if (source.texelSize.y < 0f)
			{
				center.y = 1f - center.y;
				angle = 0f - angle;
			}
			Matrix4x4 value = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
			material.SetMatrix("_RotationMatrix", value);
			material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
			material.SetFloat("_Angle", angle * ((float)Math.PI / 180f));
			Graphics.Blit(source, destination, material);
		}
	}
}
