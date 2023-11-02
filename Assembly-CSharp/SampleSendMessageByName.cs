using System.Collections;
using System.IO;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SendMessageByName")]
public class SampleSendMessageByName : MonoBehaviour, IAdvSaveData, IBinaryIO
{
	public bool isAdOpen;

	private const int Version = 0;

	public string SaveKey => "SampleSendMessageByName";

	private void Test(AdvCommandSendMessageByName command)
	{
		Debug.Log("SendMessageByName");
	}

	private void TestArg(AdvCommandSendMessageByName command)
	{
		Debug.Log(command.ParseCellOptional(AdvColumnName.Arg3, "arg3"));
	}

	private void TestWait(AdvCommandSendMessageByName command)
	{
		StartCoroutine(CoWait(command));
	}

	private IEnumerator CoWait(AdvCommandSendMessageByName command)
	{
		command.IsWait = true;
		float time = command.ParseCellOptional(AdvColumnName.Arg3, 0f);
		while (true)
		{
			Debug.Log(time);
			time -= Time.deltaTime;
			if (time <= 0f)
			{
				break;
			}
			yield return null;
		}
		command.IsWait = false;
	}

	private void SetEnableAdvertise(AdvCommandSendMessageByName command)
	{
		isAdOpen = command.ParseCellOptional(AdvColumnName.Arg3, false);
	}

	public void OnClear()
	{
		isAdOpen = false;
	}

	public void OnWrite(BinaryWriter writer)
	{
		writer.Write(0);
		writer.Write(isAdOpen);
	}

	public void OnRead(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		if (num == 0)
		{
			isAdOpen = reader.ReadBoolean();
			return;
		}
		Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
	}
}
