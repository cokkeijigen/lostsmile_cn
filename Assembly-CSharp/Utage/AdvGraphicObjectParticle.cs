using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Particle")]
	public class AdvGraphicObjectParticle : AdvGraphicObjectPrefabBase
	{
		protected ParticleSystem[] ParticleArray { get; set; }

		public override void Init(AdvGraphicObject parentObject)
		{
			base.Init(parentObject);
			parentObject.gameObject.AddComponent<ParticleAutomaticDestroyer>();
		}

		protected override void ChangeResourceOnDrawSub(AdvGraphicInfo grapic)
		{
			SetSortingOrder(base.Layer.Canvas.sortingOrder, base.Layer.Canvas.sortingLayerName);
		}

		protected void SetSortingOrder(int sortingOrder, string sortingLayerName)
		{
			ParticleArray = currentObject.GetComponentsInChildren<ParticleSystem>(true);
			ParticleSystem[] particleArray = ParticleArray;
			for (int i = 0; i < particleArray.Length; i++)
			{
				Renderer component = particleArray[i].GetComponent<Renderer>();
				component.sortingOrder += sortingOrder;
				component.sortingLayerName += sortingLayerName;
			}
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
			bool flag = (bool)currentObject;
		}

		private void FadInOut(ParticleSystem particle, float alpha)
		{
			ParticleSystem.MainModule main = particle.main;
			Color color = main.startColor.color;
			color.a = alpha;
			main.startColor = color;
		}
	}
}
