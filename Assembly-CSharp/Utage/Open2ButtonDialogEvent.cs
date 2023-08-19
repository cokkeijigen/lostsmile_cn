using System;
using UnityEngine.Events;

namespace Utage
{
	[Serializable]
	public class Open2ButtonDialogEvent : UnityEvent<string, ButtonEventInfo, ButtonEventInfo>
	{
	}
}
