using System;
using System.Collections;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/RecieveMessageFromAdvComannd")]
public class UtageRecieveMessageFromAdvComannd : MonoBehaviour
{
	public GameObject root3d;

	public GameObject rotateRoot;

	public GameObject[] models;

	private float rotSpped;

	private void OnDoCommand(AdvCommandSendMessage command)
	{
		switch (command.Name)
		{
		case "3DOn":
			TreedOn(command);
			break;
		case "3DOff":
			TreedOff(command);
			break;
		case "RotateOn":
			RotateOn(command);
			break;
		case "RotateOff":
			RotateOff(command);
			break;
		case "Model":
			ModelOn(command);
			break;
		case "ModelOff":
			ModelOff(command);
			break;
		default:
			Debug.Log("Unknown Message:" + command.Name);
			break;
		}
	}

	private void OnWait(AdvCommandSendMessage command)
	{
		string name2 = command.Name;
		command.IsWait = false;
	}

	private void TreedOn(AdvCommandSendMessage command)
	{
		root3d.SetActive(true);
	}

	private void TreedOff(AdvCommandSendMessage command)
	{
		root3d.SetActive(false);
		StopAllCoroutines();
	}

	private void RotateOn(AdvCommandSendMessage command)
	{
		if (!WrapperUnityVersion.TryParseFloatGlobal(command.Arg2, out rotSpped))
		{
			rotSpped = 15f;
		}
		StartCoroutine("CoRotate3D");
	}

	private void RotateOff(AdvCommandSendMessage command)
	{
		StopCoroutine("CoRotate3D");
	}

	private IEnumerator CoRotate3D()
	{
		while (true)
		{
			rotateRoot.transform.Rotate(Vector3.up * rotSpped * Time.deltaTime);
			yield return null;
		}
	}

	private void ModelOn(AdvCommandSendMessage command)
	{
		GameObject gameObject = FindModel(command.Arg2);
		if (gameObject != null)
		{
			gameObject.SetActive(true);
			if (!string.IsNullOrEmpty(command.Arg3))
			{
				gameObject.GetComponent<Animation>().CrossFade(command.Arg3);
			}
		}
	}

	private void ModelOff(AdvCommandSendMessage command)
	{
		GameObject gameObject = FindModel(command.Arg2);
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	private GameObject FindModel(string name)
	{
		return Array.Find(models, (GameObject s) => s.name == name);
	}
}
