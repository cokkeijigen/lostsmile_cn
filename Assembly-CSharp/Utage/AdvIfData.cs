namespace Utage
{
	internal class AdvIfData
	{
		private AdvIfData parent;

		private bool isSkpping;

		private bool isIf;

		public AdvIfData Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		public bool IsSkpping
		{
			get
			{
				return isSkpping;
			}
			set
			{
				isSkpping = value;
			}
		}

		public void BeginIf(AdvParamManager param, ExpressionParser exp)
		{
			isIf = param.CalcExpressionBoolean(exp);
			isSkpping = !isIf;
		}

		public void ElseIf(AdvParamManager param, ExpressionParser exp)
		{
			if (!isIf)
			{
				isIf = param.CalcExpressionBoolean(exp);
				isSkpping = !isIf;
			}
			else
			{
				isSkpping = true;
			}
		}

		public void Else()
		{
			if (!isIf)
			{
				isIf = true;
				isSkpping = false;
			}
			else
			{
				isSkpping = true;
			}
		}

		public void EndIf()
		{
			isSkpping = false;
		}
	}
}
