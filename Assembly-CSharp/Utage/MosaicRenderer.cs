using UnityEngine;
using UnityEngine.Rendering;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Effect/MosaicRenderer")]
	public class MosaicRenderer : MonoBehaviour
	{
		[Range(1f, 20f)]
		public float mosaicSize = 10f;

		public CompareFunction zTest = CompareFunction.Always;

		public string sortingLayerName = "";

		public int sortingOrder = 1000;

		public bool autoScale;

		[Hide("IgnoreAutoScale")]
		public int gameScreenWidth = 800;

		[Hide("IgnoreAutoScale")]
		public int gameScreenHeight = 600;

		public bool IgnoreAutoScale
		{
			get
			{
				return !autoScale;
			}
		}

		private void OnValidate()
		{
			Renderer component = GetComponent<Renderer>();
			if (!(component == null))
			{
				component.sortingLayerName = sortingLayerName;
				component.sortingOrder = sortingOrder;
			}
		}

		private void LateUpdate()
		{
			Renderer component = GetComponent<Renderer>();
			if (!(component == null))
			{
				component.sortingLayerName = sortingLayerName;
				component.sortingOrder = sortingOrder;
				component.enabled = true;
				float num = 1f;
				if (autoScale)
				{
					num = Mathf.Min(1f * (float)Screen.width / (float)gameScreenWidth, 1f * (float)Screen.height / (float)gameScreenHeight);
				}
				component.material.SetFloat("_Size", Mathf.CeilToInt(mosaicSize * num));
				component.material.SetInt("_ZTest", (int)zTest);
			}
		}
	}
}
