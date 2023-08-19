using System.IO;
using UnityEngine;

namespace Utage
{
	public class SubRoutineInfo
	{
		private BinaryReader reader;

		private const int Version = 0;

		public string ReturnLabel { get; set; }

		public int ReturnPageNo { get; set; }

		public AdvCommand ReturnCommand { get; set; }

		internal string JumpLabel { get; private set; }

		internal string CalledLabel { get; private set; }

		internal int CalledSubroutineCommandIndex { get; private set; }

		public SubRoutineInfo(AdvEngine engine, string jumpLabel, string calledLabel, int calledSubroutineCommandIndex)
		{
			JumpLabel = jumpLabel;
			CalledLabel = calledLabel;
			CalledSubroutineCommandIndex = calledSubroutineCommandIndex;
			InitReturnInfo(engine);
		}

		public SubRoutineInfo(AdvEngine engine, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				JumpLabel = reader.ReadString();
				CalledLabel = reader.ReadString();
				CalledSubroutineCommandIndex = reader.ReadInt32();
				InitReturnInfo(engine);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(JumpLabel);
			writer.Write(CalledLabel);
			writer.Write(CalledSubroutineCommandIndex);
		}

		private void InitReturnInfo(AdvEngine engine)
		{
			if (!string.IsNullOrEmpty(JumpLabel))
			{
				ReturnLabel = JumpLabel;
				ReturnPageNo = 0;
				ReturnCommand = null;
			}
			else
			{
				engine.DataManager.SetSubroutineRetunInfo(CalledLabel, CalledSubroutineCommandIndex, this);
			}
		}
	}
}
