namespace Utage
{
	internal class AdvCommandEndIf : AdvCommand
	{
		public override bool IsIfCommand => true;

		public AdvCommandEndIf(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.IfManager.EndIf();
		}
	}
}
