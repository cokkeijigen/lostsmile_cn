using UtageExtensions;

namespace Utage
{
	public class AdvCommandSendMessage : AdvCommand
	{
		private bool isWait;

		private string name;

		private string arg2;

		private string arg3;

		private string arg4;

		private string arg5;

		private string text;

		private string voice;

		private int voiceVersion;

		public bool IsWait
		{
			get
			{
				return isWait;
			}
			set
			{
				isWait = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Arg2
		{
			get
			{
				return arg2;
			}
		}

		public string Arg3
		{
			get
			{
				return arg3;
			}
		}

		public string Arg4
		{
			get
			{
				return arg4;
			}
		}

		public string Arg5
		{
			get
			{
				return arg5;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
		}

		public string Voice
		{
			get
			{
				return voice;
			}
		}

		public int VoiceVersion
		{
			get
			{
				return voiceVersion;
			}
		}

		public AdvCommandSendMessage(StringGridRow row)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
			arg2 = ParseCellOptional(AdvColumnName.Arg2, "");
			arg3 = ParseCellOptional(AdvColumnName.Arg3, "");
			arg4 = ParseCellOptional(AdvColumnName.Arg4, "");
			arg5 = ParseCellOptional(AdvColumnName.Arg5, "");
			voice = ParseCellOptional(AdvColumnName.Voice, "");
			voiceVersion = ParseCellOptional(AdvColumnName.VoiceVersion, 0);
		}

		public override void DoCommand(AdvEngine engine)
		{
			text = ParseCellLocalizedText();
			engine.ScenarioPlayer.SendMessageTarget.SafeSendMessage("OnDoCommand", this);
		}

		public override bool Wait(AdvEngine engine)
		{
			engine.ScenarioPlayer.SendMessageTarget.SafeSendMessage("OnWait", this);
			return IsWait;
		}
	}
}
