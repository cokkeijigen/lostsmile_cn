using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/Animation/Alpha")]
	public class UguiAnimationAlpha : UguiAnimation
	{
		[SerializeField]
		private float from;

		[SerializeField]
		private float to;

		[SerializeField]
		private float by;

		private float lerpFrom;

		private float lerpTo;

		public float From
		{
			get
			{
				return from;
			}
			set
			{
				from = value;
			}
		}

		public float To
		{
			get
			{
				return to;
			}
			set
			{
				to = value;
			}
		}

		public float By
		{
			get
			{
				return by;
			}
			set
			{
				by = value;
			}
		}

		protected override void StartAnimation()
		{
			switch (base.Type)
			{
			case AnimationType.To:
				lerpFrom = base.TargetGraphic.color.a;
				lerpTo = To;
				break;
			case AnimationType.From:
				lerpFrom = From;
				lerpTo = base.TargetGraphic.color.a;
				break;
			case AnimationType.FromTo:
				lerpFrom = From;
				lerpTo = To;
				break;
			case AnimationType.By:
				lerpFrom = 0f;
				lerpTo = By;
				break;
			}
			Color color = base.TargetGraphic.color;
			color.a = lerpFrom;
			base.TargetGraphic.color = color;
		}

		protected override void UpdateAnimation(float value)
		{
			Color color = base.TargetGraphic.color;
			float num = LerpValue(lerpFrom, lerpTo);
			AnimationType type = base.Type;
			if (type == AnimationType.By)
			{
				color.a += num;
			}
			else
			{
				color.a = num;
			}
			base.TargetGraphic.color = color;
		}
	}
}
