namespace Utage
{
	public class AdvPageController
	{
		public bool IsKeepText { get; private set; }

		public bool IsWaitInput { get; private set; }

		public bool IsBr { get; private set; }

		public static bool IsPageEndType(AdvPageControllerType type)
		{
			if (type == AdvPageControllerType.InputBrPage || type == AdvPageControllerType.BrPage)
			{
				return true;
			}
			return false;
		}

		public static bool IsWaitInputType(AdvPageControllerType type)
		{
			if (type == AdvPageControllerType.InputBrPage || type == AdvPageControllerType.Input || type == AdvPageControllerType.InputBr)
			{
				return true;
			}
			return false;
		}

		public static bool IsBrType(AdvPageControllerType type)
		{
			if ((uint)(type - 4) <= 1u)
			{
				return true;
			}
			return false;
		}

		public void Update(AdvPageControllerType type)
		{
			IsKeepText = !IsPageEndType(type);
			IsWaitInput = IsWaitInputType(type);
			IsBr = IsBrType(type);
		}

		public void Clear()
		{
			IsKeepText = false;
			IsWaitInput = false;
			IsBr = false;
		}
	}
}
