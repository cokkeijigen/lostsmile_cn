using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/Transition")]
	public class UguiTransition : MonoBehaviour, IMaterialModifier, IMeshModifier
	{
		private Graphic target;

		[SerializeField]
		private Texture ruleTexture;

		[SerializeField]
		[Range(0f, 1f)]
		private float strengh;

		[SerializeField]
		[Range(0.001f, 1f)]
		private float vague = 0.2f;

		public Graphic Target
		{
			get
			{
				if (target == null)
				{
					Target = GetComponent<Graphic>();
				}
				return target;
			}
			set
			{
				target = value;
				DefaultMaterial = target.material;
				Target.SetMaterialDirty();
			}
		}

		public Texture RuleTexture
		{
			get
			{
				return ruleTexture;
			}
			set
			{
				if (!(ruleTexture == value))
				{
					ruleTexture = value;
					Target.SetMaterialDirty();
				}
			}
		}

		public float Strengh
		{
			get
			{
				return strengh;
			}
			set
			{
				if (!Mathf.Approximately(strengh, value))
				{
					strengh = value;
					Target.SetMaterialDirty();
				}
			}
		}

		public float Vague
		{
			get
			{
				return vague;
			}
			set
			{
				if (!Mathf.Approximately(vague, value))
				{
					vague = value;
					Target.SetMaterialDirty();
				}
			}
		}

		public bool IsPremultipliedAlpha { get; set; }

		private Material DefaultMaterial { get; set; }

		private void Awake()
		{
			Target.SetAllDirty();
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
			if (base.enabled)
			{
				Rect rect = Target.rectTransform.rect;
				Vector2 pivot = Target.rectTransform.pivot;
				float width = rect.width;
				float height = rect.height;
				UIVertex vertex = default(UIVertex);
				for (int i = 0; i < vh.currentVertCount; i++)
				{
					vh.PopulateUIVertex(ref vertex, i);
					vertex.uv1 = new Vector2(vertex.position.x / width + pivot.x, vertex.position.y / height + pivot.y);
					vh.SetUIVertex(vertex, i);
				}
			}
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			baseMaterial.SetFloat("_Strength", Strengh);
			baseMaterial.SetFloat("_Vague", Vague);
			baseMaterial.SetTexture("_RuleTex", RuleTexture);
			if (IsPremultipliedAlpha)
			{
				baseMaterial.SetInt("_SrcBlend", 1);
				baseMaterial.SetInt("_DstBlend", 5);
				baseMaterial.EnableKeyword("PREMULTIPLIED_ALPHA");
			}
			else
			{
				baseMaterial.SetInt("_SrcBlend", 5);
				baseMaterial.SetInt("_DstBlend", 10);
				baseMaterial.DisableKeyword("PREMULTIPLIED_ALPHA");
			}
			return baseMaterial;
		}

		public void RuleFadeIn(Texture texture, float vague, bool isPremultipliedAlpha, float time, Action onComplete)
		{
			RuleTexture = texture;
			Vague = vague;
			IsPremultipliedAlpha = isPremultipliedAlpha;
			RuleFadeIn(time, onComplete);
		}

		public void RuleFadeIn(float time, Action onComplete)
		{
			Target.material = new Material(ShaderManager.RuleFade);
			Timer timer = base.gameObject.AddComponent<Timer>();
			timer.StartTimer(time, delegate(Timer x)
			{
				Strengh = x.Time01Inverse;
			}, delegate
			{
				Target.material = DefaultMaterial;
				UnityEngine.Object.Destroy(timer);
				onComplete();
			});
		}

		public void RuleFadeOut(Texture texture, float vague, bool isPremultipliedAlpha, float time, Action onComplete)
		{
			RuleTexture = texture;
			Vague = vague;
			IsPremultipliedAlpha = isPremultipliedAlpha;
			RuleFadeOut(time, onComplete);
		}

		public void RuleFadeOut(float time, Action onComplete)
		{
			Target.material = new Material(ShaderManager.RuleFade);
			Timer timer = base.gameObject.AddComponent<Timer>();
			timer.StartTimer(time, delegate(Timer x)
			{
				Strengh = x.Time01;
			}, delegate
			{
				Target.material = DefaultMaterial;
				UnityEngine.Object.Destroy(timer);
				onComplete();
			});
		}
	}
}
