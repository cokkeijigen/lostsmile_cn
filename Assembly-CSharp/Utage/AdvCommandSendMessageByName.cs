using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvCommandSendMessageByName : AdvCommand
	{
		public bool IsWait { get; set; }

		public AdvEngine Engine { get; private set; }

		public AdvCommandSendMessageByName(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			Engine = engine;
			string text = ParseCell<string>(AdvColumnName.Arg1);
			string functionName = ParseCell<string>(AdvColumnName.Arg2);
			GameObject gameObject = GameObject.Find(text);
			if (gameObject == null)
			{
				Debug.LogError(text + " is not found in current scene");
			}
			else
			{
				gameObject.SafeSendMessage(functionName, this);
			}
		}

		public override bool Wait(AdvEngine engine)
		{
			return IsWait;
		}
	}
}
