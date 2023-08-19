using UnityEngine;
using Utage;

[ExecuteInEditMode]
[AddComponentMenu("Utage/Lib/Other/BootCustomProjectSetting")]
public class BootCustomProjectSetting : MonoBehaviour
{
	[SerializeField]
	private CustomProjectSetting customProjectSetting;

	public CustomProjectSetting CustomProjectSetting
	{
		get
		{
			return customProjectSetting;
		}
		set
		{
			customProjectSetting = value;
		}
	}
}
