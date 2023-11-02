using System.Collections.Generic;

namespace Utage
{
	internal class AdvWaitManager
	{
		private List<AdvCommandWaitBase> commandList = new List<AdvCommandWaitBase>();

		internal bool IsWaiting => commandList.Count > 0;

		internal bool IsWaitingAdd
		{
			get
			{
				foreach (AdvCommandWaitBase command in commandList)
				{
					AdvCommandWaitType waitType = command.WaitType;
					if (waitType == AdvCommandWaitType.ThisAndAdd || waitType == AdvCommandWaitType.Add)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal bool IsWaitingPageEndEffect
		{
			get
			{
				foreach (AdvCommandWaitBase command in commandList)
				{
					AdvCommandWaitType waitType = command.WaitType;
					if ((uint)(waitType - 1) <= 2u)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal bool IsWaitingInputEffect
		{
			get
			{
				foreach (AdvCommandWaitBase command in commandList)
				{
					AdvCommandWaitType waitType = command.WaitType;
					if ((uint)(waitType - 2) <= 1u)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal void Clear()
		{
			commandList.Clear();
		}

		internal void StartCommand(AdvCommandWaitBase command)
		{
			AdvCommandWaitType waitType = command.WaitType;
			if (waitType != AdvCommandWaitType.NoWait)
			{
				commandList.Add(command);
			}
		}

		internal void CompleteCommand(AdvCommandWaitBase command)
		{
			AdvCommandWaitType waitType = command.WaitType;
			if (waitType != AdvCommandWaitType.NoWait)
			{
				commandList.Remove(command);
			}
		}
	}
}
