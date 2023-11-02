using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Effect/ParticleScaler")]
	public class ParticleScaler : MonoBehaviour
	{
		[SerializeField]
		private bool useLocalScale;

		[SerializeField]
		[Hide("UseLocalScale")]
		private float scale = 1f;

		[SerializeField]
		private bool changeRenderMode = true;

		[SerializeField]
		private bool changeGravity = true;

		private Dictionary<ParticleSystem, float> defaultGravities = new Dictionary<ParticleSystem, float>();

		public bool UseLocalScale
		{
			get
			{
				return useLocalScale;
			}
			set
			{
				useLocalScale = value;
				HasChanged = true;
			}
		}

		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				HasChanged = true;
			}
		}

		public bool ChangeRenderMode
		{
			get
			{
				return changeRenderMode;
			}
			set
			{
				changeRenderMode = value;
				HasChanged = true;
			}
		}

		public bool ChangeGravity
		{
			get
			{
				return changeGravity;
			}
			set
			{
				changeGravity = value;
				HasChanged = true;
			}
		}

		private bool HasChanged { get; set; }

		private bool IsInit { get; set; }

		private void Start()
		{
			HasChanged = true;
		}

		private void OnValidate()
		{
			HasChanged = true;
		}

		private void Update()
		{
			if (HasChanged)
			{
				if (!useLocalScale)
				{
					base.transform.localScale = Scale * Vector3.one;
				}
				ChangeSetting();
			}
		}

		private void ChangeSetting()
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
			foreach (ParticleSystem particle in componentsInChildren)
			{
				ChangeSetting(particle);
			}
		}

		private void ChangeSetting(ParticleSystem particle)
		{
			ParticleSystem.MainModule main = particle.main;
			main.scalingMode = ParticleSystemScalingMode.Hierarchy;
			if (particle.velocityOverLifetime.enabled)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particle.velocityOverLifetime;
				velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
			}
			if (particle.forceOverLifetime.enabled)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = particle.forceOverLifetime;
				forceOverLifetime.space = ParticleSystemSimulationSpace.Local;
			}
			if (ChangeGravity)
			{
				if (!defaultGravities.TryGetValue(particle, out var value))
				{
					value = main.gravityModifier.constant;
					defaultGravities.Add(particle, value);
				}
				main.gravityModifier = value * base.transform.lossyScale.y;
			}
			if (ChangeRenderMode)
			{
				ParticleSystemRenderer component = particle.GetComponent<ParticleSystemRenderer>();
				if (component != null && component.renderMode == ParticleSystemRenderMode.Stretch)
				{
					component.renderMode = ParticleSystemRenderMode.Billboard;
				}
			}
		}
	}
}
