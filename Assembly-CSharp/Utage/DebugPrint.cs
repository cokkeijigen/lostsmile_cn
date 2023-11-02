using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/DebugPrint")]
	public class DebugPrint : MonoBehaviour
	{
		private static DebugPrint instance;

		private List<string> logList = new List<string>();

		private float oldTime;

		private int frame;

		private float frameRate;

		private const float INTERVAL = 1f;

		private float memSizeSystem;

		private float memSizeGraphic;

		private float memSizeUsedHeap;

		private float memSizeGC;

		private float memSizeMonoHeap;

		private float memSizeMonoUsedHeap;

		private static DebugPrint GetInstance()
		{
			if (null == instance)
			{
				instance = (DebugPrint)UnityEngine.Object.FindObjectOfType(typeof(DebugPrint));
			}
			return instance;
		}

		public static void Log(object message)
		{
			GetInstance().AddLog(message as string);
		}

		public static void LogError(object message)
		{
			GetInstance().AddLogError(message as string);
		}

		public static void LogException(Exception ex)
		{
			GetInstance().AddLogError(ex.Message);
		}

		public static void LogWarning(object message)
		{
			GetInstance().AddLogWarning(message as string);
		}

		public static string GetLogString()
		{
			string text = "";
			foreach (string log in GetInstance().logList)
			{
				text = text + log + "\n";
			}
			return text;
		}

		private List<string> GetLogList()
		{
			return GetInstance().logList;
		}

		public static string GetDebugString()
		{
			return GetInstance().VersionString() + GetInstance().FpsToString() + GetInstance().MemToString();
		}

		private string VersionString()
		{
			return string.Format("Version:{0}  Unity:{1}  UTAGE:{2} \n", Application.version, Application.unityVersion, "3.5.8");
		}

		private string FpsToString()
		{
			return $"FPS:{frameRate,3:#0.} Simple:{1f / Time.deltaTime,3:#0.00}\n";
		}

		private string MemToString()
		{
			return "Mem:\nSystem " + memSizeSystem + "\nGraphic " + memSizeGraphic + "\nGC " + memSizeGC + "\nUsedHeap " + memSizeUsedHeap + "\nMonoHeap " + memSizeMonoHeap + "\nMonoUsedHeap " + memSizeMonoUsedHeap + "\n";
		}

		private void Awake()
		{
			if (null == instance)
			{
				instance = this;
			}
		}

		private void Start()
		{
			oldTime = Time.realtimeSinceStartup;
			Debug.Log("Utage Ver 3.5.8 Start!");
		}

		private void Update()
		{
			UpdateFPS();
			UpdateMemSize();
		}

		private void UpdateFPS()
		{
			frame++;
			float num = Time.realtimeSinceStartup - oldTime;
			if (num >= 1f)
			{
				frameRate = (float)frame / num;
				oldTime = Time.realtimeSinceStartup;
				frame = 0;
			}
		}

		private void UpdateMemSize()
		{
			memSizeSystem = SystemInfo.systemMemorySize;
			memSizeGraphic = SystemInfo.graphicsMemorySize;
			memSizeGC = 1f * (float)GC.GetTotalMemory(false) / 1024f / 1024f;
			memSizeUsedHeap = WrapperUnityVersion.UsedHeapMegaSize();
			memSizeMonoHeap = WrapperUnityVersion.MonoHeapMegaSize();
			memSizeMonoUsedHeap = WrapperUnityVersion.MonoUsedMegaSize();
		}

		private void AddLog(string message)
		{
			AddLogSub(message);
		}

		private void AddLogError(string message)
		{
			AddLogSub(message);
		}

		private void AddLogWarning(string message)
		{
			AddLogSub(message);
		}

		private void AddLogSub(string message)
		{
			logList.Add(message);
		}
	}
}
