using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Utage/Lib/Camera/CaptureCamera")]
	public class CaptureCamera : MonoBehaviour
	{
		public CaptureCameraEvent OnCaptured = new CaptureCameraEvent();

		public RenderTexture CaptureImage { get; set; }

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (base.enabled)
			{
				if (CaptureImage != null)
				{
					RenderTexture.ReleaseTemporary(CaptureImage);
				}
				CaptureImage = source.CreateCopyTemporary();
				Graphics.Blit(source, destination);
				OnCaptured.Invoke(this);
			}
		}
	}
}
