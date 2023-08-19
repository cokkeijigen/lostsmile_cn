using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Examples/CharacterGrayOutControllerRecieveMessage")]
	public class SampleCharacterGrayOutControllerRecieveMessage : MonoBehaviour
	{
		private void ChangeNoGrayoutCharancter(AdvCommandSendMessageByName message)
		{
			AdvCharacterGrayOutController component = GetComponent<AdvCharacterGrayOutController>();
			string[] collection = message.RowData.ParseCellOptionalArray("Arg3", new string[0]);
			component.NoGrayoutCharacters = new List<string>(collection);
		}
	}
}
