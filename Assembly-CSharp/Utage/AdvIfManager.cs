using UnityEngine;

namespace Utage
{
	internal class AdvIfManager
	{
		private bool isLoadInit;

		private AdvIfData current;

		public bool IsLoadInit
		{
			get
			{
				return isLoadInit;
			}
			set
			{
				isLoadInit = value;
			}
		}

		public void Clear()
		{
			current = null;
		}

		public void BeginIf(AdvParamManager param, ExpressionParser exp)
		{
			IsLoadInit = false;
			AdvIfData advIfData = new AdvIfData();
			if (current != null)
			{
				advIfData.Parent = current;
			}
			current = advIfData;
			current.BeginIf(param, exp);
		}

		public void ElseIf(AdvParamManager param, ExpressionParser exp)
		{
			if (current == null)
			{
				if (!IsLoadInit)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.ElseIf, exp));
				}
				current = new AdvIfData();
			}
			current.ElseIf(param, exp);
		}

		public void Else()
		{
			if (current == null)
			{
				if (!IsLoadInit)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.Else));
				}
				current = new AdvIfData();
			}
			current.Else();
		}

		public void EndIf()
		{
			if (current == null)
			{
				if (!IsLoadInit)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndIf));
				}
				current = new AdvIfData();
			}
			current.EndIf();
			current = current.Parent;
		}

		public bool CheckSkip(AdvCommand command)
		{
			if (command == null)
			{
				return false;
			}
			if (current == null)
			{
				return false;
			}
			if (current.IsSkpping)
			{
				return !command.IsIfCommand;
			}
			return false;
		}
	}
}
