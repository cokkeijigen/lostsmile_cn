using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	internal class AdvJumpManager
	{
		private class RandomInfo
		{
			public AdvCommand command;

			public float rate;

			public RandomInfo(AdvCommand command, float rate)
			{
				this.command = command;
				this.rate = rate;
			}
		}

		private Stack<SubRoutineInfo> subRoutineCallStack = new Stack<SubRoutineInfo>();

		private List<RandomInfo> randomInfoList = new List<RandomInfo>();

		private const int Version = 0;

		internal string Label { get; private set; }

		internal SubRoutineInfo SubRoutineReturnInfo { get; private set; }

		internal Stack<SubRoutineInfo> SubRoutineCallStack
		{
			get
			{
				return subRoutineCallStack;
			}
		}

		internal bool IsReserved
		{
			get
			{
				if (string.IsNullOrEmpty(Label))
				{
					return SubRoutineReturnInfo != null;
				}
				return true;
			}
		}

		internal void RegistoreLabel(string jumpLabel)
		{
			Label = jumpLabel;
		}

		internal void RegistoreSubroutine(string label, SubRoutineInfo calledInfo)
		{
			Label = label;
			subRoutineCallStack.Push(calledInfo);
		}

		internal void EndSubroutine()
		{
			if (subRoutineCallStack.Count > 0)
			{
				SubRoutineReturnInfo = subRoutineCallStack.Pop();
			}
			else
			{
				Debug.LogErrorFormat("Failed to terminate the subroutine.Please call the subroutine with 'JumpSubRoutine'.");
			}
		}

		internal void AddRandom(AdvCommand command, float rate)
		{
			randomInfoList.Add(new RandomInfo(command, rate));
		}

		internal void ClearOnJump()
		{
			Label = "";
			SubRoutineReturnInfo = null;
			randomInfoList.Clear();
		}

		internal void Clear()
		{
			ClearOnJump();
			subRoutineCallStack.Clear();
		}

		internal AdvCommand GetRandomJumpCommand()
		{
			float sum = 0f;
			randomInfoList.ForEach(delegate(RandomInfo item)
			{
				sum += item.rate;
			});
			if (sum <= 0f)
			{
				return null;
			}
			float num = Random.Range(0f, sum);
			foreach (RandomInfo randomInfo in randomInfoList)
			{
				num -= randomInfo.rate;
				if (num <= 0f)
				{
					return randomInfo.command;
				}
			}
			return null;
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(subRoutineCallStack.Count);
			foreach (SubRoutineInfo item in subRoutineCallStack)
			{
				item.Write(writer);
			}
		}

		internal void Read(AdvEngine engine, BinaryReader reader)
		{
			Clear();
			if (reader.BaseStream.Length <= 0)
			{
				return;
			}
			int num = reader.ReadInt32();
			if (num == 0)
			{
				int num2 = reader.ReadInt32();
				SubRoutineInfo[] array = new SubRoutineInfo[num2];
				for (int i = 0; i < num2; i++)
				{
					array[i] = new SubRoutineInfo(engine, reader);
				}
				for (int num3 = num2 - 1; num3 >= 0; num3--)
				{
					subRoutineCallStack.Push(array[num3]);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
