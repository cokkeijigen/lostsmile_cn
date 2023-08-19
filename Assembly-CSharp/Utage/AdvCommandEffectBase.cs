using UnityEngine;

namespace Utage
{
	public abstract class AdvCommandEffectBase : AdvCommandWaitBase
	{
		protected AdvEffectManager.TargetType targetType;

		protected string targetName;

		public AdvEffectManager.TargetType Target
		{
			get
			{
				return targetType;
			}
		}

		public string TargetName
		{
			get
			{
				return targetName;
			}
		}

		protected AdvCommandEffectBase(StringGridRow row)
			: base(row)
		{
			OnParse();
		}

		protected virtual void OnParse()
		{
			ParseEffectTarget(AdvColumnName.Arg1);
			ParseWait(AdvColumnName.WaitType);
		}

		protected virtual void ParseWait(AdvColumnName columnName)
		{
			AdvCommandWaitType val;
			if (IsEmptyCell(columnName))
			{
				base.WaitType = AdvCommandWaitType.ThisAndAdd;
			}
			else if (!ParserUtil.TryParaseEnum<AdvCommandWaitType>(ParseCell<string>(columnName), out val))
			{
				base.WaitType = AdvCommandWaitType.NoWait;
				Debug.LogError(ToErrorString("UNKNOWN WaitType"));
			}
			else
			{
				base.WaitType = val;
			}
		}

		protected virtual void ParseEffectTarget(AdvColumnName columnName)
		{
			targetName = ParseCell<string>(columnName);
			if (!ParserUtil.TryParaseEnum<AdvEffectManager.TargetType>(targetName, out targetType))
			{
				targetType = AdvEffectManager.TargetType.Default;
			}
		}

		protected override void OnStart(AdvEngine engine, AdvScenarioThread thread)
		{
			GameObject gameObject = engine.EffectManager.FindTarget(this);
			if (gameObject == null)
			{
				Debug.LogError(base.RowData.ToErrorString(TargetName + " is not found"));
				OnComplete(thread);
			}
			else
			{
				OnStartEffect(gameObject, engine, thread);
			}
		}

		protected abstract void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread);
	}
}
