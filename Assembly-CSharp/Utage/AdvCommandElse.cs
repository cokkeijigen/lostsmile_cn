namespace Utage
{
	internal class AdvCommandElse : AdvCommand
	{
		public override bool IsIfCommand
		{
			get
			{
				return true;
			}
		}

		public AdvCommandElse(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.IfManager.Else();
		}
	}
}
