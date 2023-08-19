using UnityEngine;

namespace Utage
{
	internal class AdvCommandRuleFadeOut : AdvCommandEffectBase
	{
		private AdvTransitionArgs data;

		public AdvCommandRuleFadeOut(StringGridRow row)
			: base(row)
		{
			string textureName = ParseCell<string>(AdvColumnName.Arg2);
			float vague = ParseCellOptional(AdvColumnName.Arg3, 0.2f);
			float time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			data = new AdvTransitionArgs(textureName, vague, time);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			IAdvFade componentInChildren = target.GetComponentInChildren<IAdvFade>(true);
			if (componentInChildren == null)
			{
				Debug.LogError("Can't find [ " + base.TargetName + " ]");
				OnComplete(thread);
			}
			else
			{
				componentInChildren.RuleFadeOut(engine, data, delegate
				{
					OnComplete(thread);
				});
			}
		}
	}
}
