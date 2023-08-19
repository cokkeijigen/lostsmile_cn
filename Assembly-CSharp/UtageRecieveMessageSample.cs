using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/UtageRecieveMessageSample")]
public class UtageRecieveMessageSample : MonoBehaviour
{
	public AdvEngine engine;

	public InputField inputFiled;

	private void Awake()
	{
		if (inputFiled != null)
		{
			inputFiled.gameObject.SetActive(false);
		}
	}

	private void OnDoCommand(AdvCommandSendMessage command)
	{
		switch (command.Name)
		{
		case "DebugLog":
			DebugLog(command);
			break;
		case "InputFileld":
			InputFileld(command);
			break;
		case "AutoLoad":
			AutoLoad(command);
			break;
		default:
			Debug.Log("Unknown Message:" + command.Name);
			break;
		}
	}

	private void OnWait(AdvCommandSendMessage command)
	{
		string text = command.Name;
		if (!(text == "InputFileld"))
		{
			if (text == "AutoLoad")
			{
				command.IsWait = true;
			}
			else
			{
				command.IsWait = false;
			}
		}
		else
		{
			command.IsWait = inputFiled.gameObject.activeSelf;
		}
	}

	private void OnAgingInput(AdvCommandSendMessage command)
	{
		string text = command.Name;
		if (text == "InputFileld")
		{
			inputFiled.gameObject.SetActive(false);
		}
	}

	private void DebugLog(AdvCommandSendMessage command)
	{
		Debug.Log(command.Arg2);
	}

	private void InputFileld(AdvCommandSendMessage command)
	{
		inputFiled.gameObject.SetActive(true);
		inputFiled.onEndEdit.RemoveAllListeners();
		inputFiled.onEndEdit.AddListener(delegate(string text)
		{
			OnEndEditInputFiled(command.Arg2, text);
		});
	}

	private void OnEndEditInputFiled(string paramName, string text)
	{
		if (!engine.Param.TrySetParameter(paramName, text))
		{
			Debug.LogError(paramName + "is not found");
		}
		inputFiled.gameObject.SetActive(false);
	}

	private void AutoLoad(AdvCommandSendMessage command)
	{
		Debug.Log("AutoLoad");
		StartCoroutine(CoAutoLoadSub());
	}

	private IEnumerator CoAutoLoadSub()
	{
		engine.ScenarioPlayer.IsReservedEndScenario = true;
		yield return null;
		engine.SaveManager.ReadAutoSaveData();
		if (engine.SaveManager.AutoSaveData == null || !engine.SaveManager.AutoSaveData.IsSaved)
		{
			Debug.LogError("AutoLoad is not yet load");
		}
		else
		{
			engine.OpenLoadGame(engine.SaveManager.AutoSaveData);
		}
	}
}
