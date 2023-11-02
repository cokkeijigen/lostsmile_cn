using UnityEngine;
using Utage;
using UtageExtensions;

public class UtageSampleLoadCaharacter : MonoBehaviour
{
	private AdvGraphicLoader loader;

	public AdvUguiLoadGraphicFile texture;

	[SerializeField]
	private AdvEngine engine;

	[SerializeField]
	private DicingImage dicingImage;

	[SerializeField]
	private string testName = "";

	[SerializeField]
	private string testPattern = "";

	public AdvGraphicLoader Loader => this.GetComponentCacheCreateIfMissing(ref loader);

	public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	private void Start()
	{
		Load(testName, testPattern);
	}

	public void Load(string name, string pattern)
	{
		AdvGraphicInfo graphicInfo = Engine.DataManager.SettingDataManager.CharacterSetting.KeyToGraphicInfo(AdvCharacterSetting.ToDataKey(name, pattern)).Main;
		Loader.LoadGraphic(graphicInfo, delegate
		{
			OnLoaded(graphicInfo);
		});
	}

	private void OnLoaded(AdvGraphicInfo graphic)
	{
		string fileType = graphic.FileType;
		if (fileType == "Dicing")
		{
			dicingImage.DicingData = graphic.File.UnityObject as DicingTextures;
			string subFileName = graphic.SubFileName;
			dicingImage.ChangePattern(subFileName);
		}
		else
		{
			Debug.LogError(graphic.FileType + " is not support ");
		}
	}
}
