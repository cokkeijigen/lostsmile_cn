using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/RichOutline")]
	public class UguiRichOutline : Outline
	{
		public int copyCount = 16;

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = ListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				ModifyVerticesSub(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				ListPool<UIVertex>.Release(list);
			}
		}

		private void ModifyVerticesSub(List<UIVertex> verts)
		{
			int start = 0;
			int count = verts.Count;
			for (int i = 0; i < copyCount; i++)
			{
				float x = Mathf.Sin((float)Math.PI * 2f * (float)i / (float)copyCount) * base.effectDistance.x;
				float y = Mathf.Cos((float)Math.PI * 2f * (float)i / (float)copyCount) * base.effectDistance.y;
				ApplyShadow(verts, base.effectColor, start, verts.Count, x, y);
				start = count;
				count = verts.Count;
			}
		}
	}
}
