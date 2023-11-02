using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/CrossFadeRawImage")]
	public class UguiCrossFadeRawImage : MonoBehaviour, IMeshModifier, IMaterialModifier
	{
		[SerializeField]
		private Texture fadeTexture;

		[SerializeField]
		[Range(0f, 1f)]
		private float strengh = 1f;

		protected Graphic target;

		private Timer timer;

		private Material lastMaterial;

		private Material corssFadeMaterial;

		public Texture FadeTexture
		{
			get
			{
				return fadeTexture;
			}
			set
			{
				if (!(fadeTexture == value))
				{
					fadeTexture = value;
					Target.SetVerticesDirty();
					Target.SetMaterialDirty();
				}
			}
		}

		private float Strengh
		{
			get
			{
				return strengh;
			}
			set
			{
				strengh = value;
				Target.SetMaterialDirty();
			}
		}

		public virtual Graphic Target => target ?? (target = GetComponent<RawImage>());

		private Timer Timer
		{
			get
			{
				if (timer == null)
				{
					timer = base.gameObject.AddComponent<Timer>();
				}
				return timer;
			}
		}

		public Material Material
		{
			get
			{
				return Target.material;
			}
			set
			{
				Target.material = value;
			}
		}

		private void Awake()
		{
			lastMaterial = Target.material;
			corssFadeMaterial = new Material(ShaderManager.CrossFade);
			Material = corssFadeMaterial;
		}

		private void OnDestroy()
		{
			Material = lastMaterial;
			UnityEngine.Object.Destroy(corssFadeMaterial);
			UnityEngine.Object.Destroy(timer);
		}

		public void ModifyMesh(Mesh mesh)
		{
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				ModifyMesh(vertexHelper);
				vertexHelper.FillMesh(mesh);
			}
		}

		public void ModifyMesh(VertexHelper vh)
		{
			if (!(Target.mainTexture == null))
			{
				RebuildVertex(vh);
			}
		}

		public virtual void RebuildVertex(VertexHelper vh)
		{
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.uv1 = vertex.uv0;
				vh.SetUIVertex(vertex, i);
			}
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			baseMaterial.SetFloat("_Strength", Strengh);
			baseMaterial.SetTexture("_FadeTex", FadeTexture);
			return baseMaterial;
		}

		internal void CrossFade(Texture fadeTexture, float time, Action onComplete)
		{
			FadeTexture = fadeTexture;
			Target.material.EnableKeyword("CROSS_FADE");
			Timer.StartTimer(time, delegate(Timer x)
			{
				Strengh = x.Time01Inverse;
			}, delegate
			{
				Target.material.DisableKeyword("CROSS_FADE");
				onComplete();
			});
		}
	}
}
