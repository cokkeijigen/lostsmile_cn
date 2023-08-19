using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/LoadScene")]
	public class AdvLoadScene : MonoBehaviour
	{
		private void LoadScene(AdvCommandSendMessageByName command)
		{
			SceneManager.LoadScene(command.ParseCell<string>(AdvColumnName.Arg3));
		}
	}
}
