using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/MessageWindowManager")]
	public class AdvMessageWindowManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		[SerializeField]
		private MessageWindowEvent onReset = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onChangeActiveWindows = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onChangeCurrentWindow = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onTextChange = new MessageWindowEvent();

		private AdvEngine engine;

		private bool isInit;

		private Dictionary<string, AdvMessageWindow> allWindows = new Dictionary<string, AdvMessageWindow>();

		private List<string> defaultActiveWindowNameList = new List<string>();

		private Dictionary<string, AdvMessageWindow> activeWindows = new Dictionary<string, AdvMessageWindow>();

		private const int Version = 0;

		public MessageWindowEvent OnReset
		{
			get
			{
				return onReset;
			}
		}

		public MessageWindowEvent OnChangeActiveWindows
		{
			get
			{
				return onChangeActiveWindows;
			}
		}

		public MessageWindowEvent OnChangeCurrentWindow
		{
			get
			{
				return onChangeCurrentWindow;
			}
		}

		public MessageWindowEvent OnTextChange
		{
			get
			{
				return onTextChange;
			}
		}

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = GetComponent<AdvEngine>());
			}
		}

		public Dictionary<string, AdvMessageWindow> AllWindows
		{
			get
			{
				if (!isInit)
				{
					InitWindows();
				}
				return allWindows;
			}
		}

		private List<string> DefaultActiveWindowNameList
		{
			get
			{
				if (!isInit)
				{
					InitWindows();
				}
				return defaultActiveWindowNameList;
			}
		}

		public Dictionary<string, AdvMessageWindow> ActiveWindows
		{
			get
			{
				return activeWindows;
			}
		}

		public AdvMessageWindow CurrentWindow { get; private set; }

		public AdvMessageWindow LastWindow { get; private set; }

		public string SaveKey
		{
			get
			{
				return "MessageWindowManager";
			}
		}

		public bool IsCurrent(string name)
		{
			return CurrentWindow.Name == name;
		}

		public bool IsActiveWindow(string name)
		{
			return ActiveWindows.ContainsKey(name);
		}

		private void InitWindows()
		{
			IAdvMessageWindow[] componentsInChildren = GetComponentsInChildren<IAdvMessageWindow>(true);
			IAdvMessageWindow[] array = componentsInChildren;
			foreach (IAdvMessageWindow advMessageWindow in array)
			{
				string text = advMessageWindow.gameObject.name;
				if (allWindows.ContainsKey(text))
				{
					Debug.LogError(text + ". The same name already exists. Please change to a different name.");
				}
				allWindows.Add(advMessageWindow.gameObject.name, new AdvMessageWindow(advMessageWindow));
			}
			array = componentsInChildren;
			foreach (IAdvMessageWindow advMessageWindow2 in array)
			{
				if (advMessageWindow2.gameObject.activeSelf)
				{
					defaultActiveWindowNameList.Add(advMessageWindow2.gameObject.name);
				}
			}
			array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnInit(this);
			}
			isInit = true;
		}

		internal void ChangeActiveWindows(List<string> names)
		{
			ActiveWindows.Clear();
			foreach (string name in names)
			{
				AdvMessageWindow value;
				if (!AllWindows.TryGetValue(name, out value))
				{
					Debug.LogError(name + " is not found in message windows");
				}
				else
				{
					ActiveWindows.Add(name, value);
				}
			}
			CalllEventActiveWindows();
		}

		private void CalllEventActiveWindows()
		{
			foreach (AdvMessageWindow value in AllWindows.Values)
			{
				value.ChangeActive(IsActiveWindow(value.Name));
			}
			OnChangeActiveWindows.Invoke(this);
		}

		internal void ChangeCurrentWindow(string name)
		{
			if (string.IsNullOrEmpty(name) || (CurrentWindow != null && CurrentWindow.Name == name))
			{
				return;
			}
			AdvMessageWindow value;
			if (!ActiveWindows.TryGetValue(name, out value))
			{
				if (!AllWindows.TryGetValue(name, out value))
				{
					Debug.LogWarning(name + "is not found in window manager");
					name = DefaultActiveWindowNameList[0];
					value = AllWindows[name];
				}
				if (CurrentWindow != null)
				{
					ActiveWindows.Remove(CurrentWindow.Name);
				}
				ActiveWindows.Add(name, value);
				CalllEventActiveWindows();
			}
			LastWindow = CurrentWindow;
			CurrentWindow = value;
			if (LastWindow != null)
			{
				LastWindow.ChangeCurrent(false);
			}
			CurrentWindow.ChangeCurrent(true);
			OnChangeCurrentWindow.Invoke(this);
		}

		internal AdvMessageWindow FindWindow(string name)
		{
			AdvMessageWindow value = CurrentWindow;
			if (!string.IsNullOrEmpty(name) && !AllWindows.TryGetValue(name, out value))
			{
				Debug.LogError(name + "is not found in all message windows");
			}
			return value;
		}

		internal void OnPageTextChange(AdvPage page)
		{
			CurrentWindow.PageTextChange(page);
			OnTextChange.Invoke(this);
		}

		public virtual void OnClear()
		{
			if (DefaultActiveWindowNameList.Count <= 0)
			{
				Debug.LogWarning("defaultWindowNameList is zero");
				return;
			}
			ChangeActiveWindows(DefaultActiveWindowNameList);
			ChangeCurrentWindow(DefaultActiveWindowNameList[0]);
			foreach (AdvMessageWindow value in AllWindows.Values)
			{
				value.Reset();
			}
			OnReset.Invoke(this);
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(ActiveWindows.Count);
			foreach (KeyValuePair<string, AdvMessageWindow> activeWindow in ActiveWindows)
			{
				writer.Write(activeWindow.Key);
				writer.WriteBuffer(activeWindow.Value.WritePageData);
			}
			string value = ((CurrentWindow == null) ? "" : CurrentWindow.Name);
			writer.Write(value);
		}

		public virtual void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				List<string> list = new List<string>();
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					string item = reader.ReadString();
					byte[] bytes = reader.ReadBytes(reader.ReadInt32());
					list.Add(item);
					BinaryUtil.BinaryRead(bytes, FindWindow(item).ReadPageData);
				}
				string text = reader.ReadString();
				ChangeActiveWindows(list);
				ChangeCurrentWindow(text);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
