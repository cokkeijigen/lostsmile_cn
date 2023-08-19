namespace Utage
{
	public abstract class AdvCommandWaitBase : AdvCommand
	{
		public AdvCommandWaitType WaitType { get; protected set; }

		protected AdvCommandWaitBase(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.WaitManager.StartCommand(this);
			OnStart(engine, base.CurrentTread);
		}

		public override bool Wait(AdvEngine engine)
		{
			switch (WaitType)
			{
			case AdvCommandWaitType.ThisAndAdd:
				return base.CurrentTread.WaitManager.IsWaitingAdd;
			default:
				return false;
			}
		}

		protected abstract void OnStart(AdvEngine engine, AdvScenarioThread thread);

		internal virtual void OnComplete(AdvScenarioThread thread)
		{
			thread.WaitManager.CompleteCommand(this);
		}
	}
}
