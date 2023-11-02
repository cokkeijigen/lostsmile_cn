using UnityEngine;

namespace Utage
{
	public static class ShaderManager
	{
		public static string ColorFade = "Utage/ImageEffect/ColorFade";

		public static string BloomName = "Utage/ImageEffect/Bloom";

		public static string BlurName = "Utage/ImageEffect/Blur";

		public static string MosaicName = "Utage/ImageEffect/Mosaic";

		public static string ColorCorrectionRampName = "Utage/ImageEffect/ColorCorrectionRamp";

		public static string GrayScaleName = "Utage/ImageEffect/Grayscale";

		public static string MotionBlurName = "Utage/ImageEffect/MotionBlur";

		public static string NoiseAndGrainName = "Utage/ImageEffect/NoiseAndGrain";

		public static string BlendModesOverlayName = "Utage/ImageEffect/BlendModesOverlay";

		public static string SepiatoneName = "Utage/ImageEffect/Sepiatone";

		public static string NegaPosiName = "Utage/ImageEffect/NegaPosi";

		public static string FisheyeName = "Utage/ImageEffect/Fisheye";

		public static string TwirlName = "Utage/ImageEffect/Twirl";

		public static string VortexName = "Utage/ImageEffect/Vortex";

		public static Shader RuleFade => Shader.Find("Utage/UI/RuleFade");

		public static Shader CrossFade => Shader.Find("Utage/CrossFadeImage");

		public static Shader RenderTexture => Shader.Find("Utage/RenderTexture");

		public static Shader DrawByRenderTexture => Shader.Find("Utage/DrawByRenderTexture");
	}
}
