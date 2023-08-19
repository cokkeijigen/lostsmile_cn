using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Utage
{
	[Serializable]
	public class OpenDialogEvent : UnityEvent<string, List<ButtonEventInfo>>
	{
	}
}
