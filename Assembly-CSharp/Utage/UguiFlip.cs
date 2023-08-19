using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/Flip")]
	public class UguiFlip : BaseMeshEffect
	{
		[SerializeField]
		private bool flipX;

		[SerializeField]
		private bool flipY;

		public bool FlipX
		{
			get
			{
				return flipX;
			}
			set
			{
				flipX = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public bool FlipY
		{
			get
			{
				return flipY;
			}
			set
			{
				flipY = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (!base.enabled || (!FlipX && !FlipY))
			{
				return;
			}
			Rect rect = base.graphic.rectTransform.rect;
			Vector2 pivot = base.graphic.rectTransform.pivot;
			float num = (0f - (pivot.x - 0.5f)) * rect.width * 2f;
			float num2 = (0f - (pivot.y - 0.5f)) * rect.height * 2f;
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				if (FlipX)
				{
					vertex.position.x = 0f - vertex.position.x + num;
				}
				if (FlipY)
				{
					vertex.position.y = 0f - vertex.position.y + num2;
				}
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
