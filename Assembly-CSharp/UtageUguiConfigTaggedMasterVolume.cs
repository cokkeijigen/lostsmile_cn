using UnityEngine;

[AddComponentMenu("Utage/TemplateUI/ConfigTaggedMasterVolume")]
public class UtageUguiConfigTaggedMasterVolume : MonoBehaviour
{
	public string volumeTag = "";

	public UtageUguiConfig config;

	public virtual void OnValugeChanged(float value)
	{
		if (!string.IsNullOrEmpty(volumeTag))
		{
			config.OnValugeChangedTaggedMasterVolume(volumeTag, value);
		}
	}
}
