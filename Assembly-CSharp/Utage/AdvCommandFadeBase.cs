using UnityEngine;
using UtageExtensions;

namespace Utage
{
	internal abstract class AdvCommandFadeBase : AdvCommandEffectBase
	{
		private float time;

		private bool inverse;

		private Color color;

		public AdvCommandFadeBase(StringGridRow row, bool inverse)
			: base(row)
		{
			this.inverse = inverse;
		}

		protected override void OnParse()
		{
			color = ParseCellOptional(AdvColumnName.Arg1, Color.white);
			if (IsEmptyCell(AdvColumnName.Arg2))
			{
				targetName = "SpriteCamera";
			}
			else
			{
				targetName = ParseCell<string>(AdvColumnName.Arg2);
			}
			time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			targetType = AdvEffectManager.TargetType.Camera;
			ParseWait(AdvColumnName.WaitType);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			Camera componentInChildren = target.GetComponentInChildren<Camera>(true);
			ImageEffectUtil.TryGetComonentCreateIfMissing(ImageEffectType.ColorFade.ToString(), out var imageEffect, out var alreadyEnabled, componentInChildren.gameObject);
			ColorFade colorFade = imageEffect as ColorFade;
			float start;
			float end;
			if (inverse)
			{
				start = colorFade.color.a;
				end = 0f;
			}
			else
			{
				start = (alreadyEnabled ? colorFade.Strength : 0f);
				end = color.a;
			}
			imageEffect.enabled = true;
			colorFade.color = color;
			Timer timer = componentInChildren.gameObject.AddComponent<Timer>();
			timer.AutoDestroy = true;
			timer.StartTimer(engine.Page.ToSkippedTime(time), delegate(Timer x)
			{
				colorFade.Strength = x.GetCurve(start, end);
			}, delegate
			{
				OnComplete(thread);
				if (inverse)
				{
					imageEffect.enabled = false;
					imageEffect.RemoveComponentMySelf();
				}
			});
		}
	}
}
