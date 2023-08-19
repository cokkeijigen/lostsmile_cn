using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/GraphicRenderTextureManager")]
	public class AdvGraphicRenderTextureManager : MonoBehaviour
	{
		public float offset = 10000f;

		private List<AdvRenderTextureSpace> spaceList = new List<AdvRenderTextureSpace>();

		internal AdvRenderTextureSpace CreateSpace()
		{
			AdvRenderTextureSpace advRenderTextureSpace = base.transform.AddChildGameObjectComponent<AdvRenderTextureSpace>("RenderTextureSpace");
			int i;
			for (i = 0; i < spaceList.Count; i++)
			{
				if (spaceList[i] == null)
				{
					spaceList[i] = advRenderTextureSpace;
					break;
				}
			}
			if (i >= spaceList.Count)
			{
				spaceList.Add(advRenderTextureSpace);
			}
			advRenderTextureSpace.transform.localPosition = new Vector3(0f, (float)(i + 1) * offset, 0f);
			return advRenderTextureSpace;
		}
	}
}
