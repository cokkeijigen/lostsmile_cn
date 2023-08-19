using System;
using UnityEngine.Events;

namespace Utage
{
	[Serializable]
	public class Open3ButtonDialogEvent : UnityEvent<string, ButtonEventInfo, ButtonEventInfo, ButtonEventInfo>
	{
	}
}
