namespace Utage
{
	internal class AdvCommandPageControler : AdvCommand
	{
		private AdvPageControllerType pageCtrlType;

		public AdvCommandPageControler(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			if (row == null)
			{
				pageCtrlType = AdvPageControllerType.InputBrPage;
			}
			else
			{
				pageCtrlType = ParseCellOptional(AdvColumnName.PageCtrl, AdvPageControllerType.InputBrPage);
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.Page.UpdatePageTextData(pageCtrlType);
		}

		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitTextCommand;
		}

		public override bool IsTypePage()
		{
			return true;
		}

		public override bool IsTypePageEnd()
		{
			return AdvPageController.IsPageEndType(pageCtrlType);
		}
	}
}
