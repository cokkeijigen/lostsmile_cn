using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/ScenarioPlayer")]
	public class AdvScenarioPlayer : MonoBehaviour, IBinaryIO
	{
		[Flags]
		private enum DebugOutPut
		{
			Log = 1,
			Waiting = 2,
			CommandEnd = 4
		}

		[SerializeField]
		private GameObject sendMessageTarget;

		[SerializeField]
		[EnumFlags]
		private DebugOutPut debugOutPut;

		[SerializeField]
		private int maxFilePreload = 20;

		[SerializeField]
		private int preloadDeep = 5;

		[SerializeField]
		public AdvScenarioPlayerEvent onEndScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvScenarioPlayerEvent onPauseScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvScenarioPlayerEvent onEndOrPauseScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvCommandEvent onBeginCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onUpdatePreWaitingCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onUpdateWaitingCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onEndCommand = new AdvCommandEvent();

		private AdvEngine engine;

		private AdvScenarioThread mainThread;

		private const int Version = 0;

		public GameObject SendMessageTarget
		{
			get
			{
				return sendMessageTarget;
			}
		}

		internal bool DebugOutputLog
		{
			get
			{
				return (debugOutPut & DebugOutPut.Log) == DebugOutPut.Log;
			}
		}

		internal bool DebugOutputWaiting
		{
			get
			{
				return (debugOutPut & DebugOutPut.Waiting) == DebugOutPut.Waiting;
			}
		}

		internal bool DebugOutputCommandEnd
		{
			get
			{
				return (debugOutPut & DebugOutPut.CommandEnd) == DebugOutPut.CommandEnd;
			}
		}

		internal int MaxFilePreload
		{
			get
			{
				return maxFilePreload;
			}
		}

		internal int PreloadDeep
		{
			get
			{
				return preloadDeep;
			}
		}

		public AdvScenarioPlayerEvent OnEndScenario
		{
			get
			{
				return onEndScenario;
			}
		}

		public AdvScenarioPlayerEvent OnPauseScenario
		{
			get
			{
				return onPauseScenario;
			}
		}

		public AdvScenarioPlayerEvent OnEndOrPauseScenario
		{
			get
			{
				return onEndOrPauseScenario;
			}
		}

		public AdvCommandEvent OnBeginCommand
		{
			get
			{
				return onBeginCommand;
			}
		}

		public AdvCommandEvent OnUpdatePreWaitingCommand
		{
			get
			{
				return onUpdatePreWaitingCommand;
			}
		}

		public AdvCommandEvent OnUpdateWaitingCommand
		{
			get
			{
				return onUpdateWaitingCommand;
			}
		}

		public AdvCommandEvent OnEndCommand
		{
			get
			{
				return onEndCommand;
			}
		}

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = GetComponent<AdvEngine>());
			}
		}

		public AdvScenarioThread MainThread
		{
			get
			{
				if (mainThread == null)
				{
					mainThread = base.gameObject.GetComponentCreateIfMissing<AdvScenarioThread>();
					mainThread.Init(this, "MainThread", null);
				}
				return mainThread;
			}
		}

		public bool IsEndScenario { get; set; }

		public bool IsReservedEndScenario { get; set; }

		public bool IsPausing { get; set; }

		public string CurrentGallerySceneLabel { get; set; }

		public bool IsLoading
		{
			get
			{
				return MainThread.IsLoadingDeep;
			}
		}

		public string SaveKey
		{
			get
			{
				return "ScenarioPlayer";
			}
		}

		public virtual void StartScenario(string label, int page)
		{
			IsPausing = false;
			IsEndScenario = false;
			IsReservedEndScenario = false;
			CurrentGallerySceneLabel = "";
			MainThread.Clear();
			MainThread.StartScenario(label, page, false);
		}

		internal IEnumerator CoStartSaveData(AdvSaveData saveData)
		{
			IsPausing = false;
			IsEndScenario = false;
			IsReservedEndScenario = false;
			MainThread.Clear();
			saveData.LoadGameData(Engine, Engine.SaveManager.CustomSaveDataIOList, Engine.SaveManager.GetSaveIoListCreateIfMissing(Engine));
			yield return null;
			saveData.Buffer.Overrirde(this);
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			MainThread.JumpManager.Write(writer);
			writer.Write(Engine.Page.ScenarioLabel);
			writer.Write(Engine.Page.PageNo);
			writer.Write(CurrentGallerySceneLabel);
			writer.Write(MainThread.SkipPageHeaerOnSave);
		}

		public void OnRead(BinaryReader reader)
		{
			if (reader.ReadInt32() <= 0)
			{
				MainThread.JumpManager.Read(Engine, reader);
				string label = reader.ReadString();
				int page = reader.ReadInt32();
				string currentGallerySceneLabel = reader.ReadString();
				bool skipPageHeaer = reader.ReadBoolean();
				MainThread.ScenarioPlayer.CurrentGallerySceneLabel = currentGallerySceneLabel;
				MainThread.StartScenario(label, page, skipPageHeaer);
			}
		}

		public virtual void EndScenario()
		{
			OnEndScenario.Invoke(this);
			OnEndOrPauseScenario.Invoke(this);
			Engine.ClearOnEnd();
			MainThread.Clear();
			IsEndScenario = true;
		}

		public void Pause()
		{
			IsPausing = true;
			OnPauseScenario.Invoke(this);
			OnEndOrPauseScenario.Invoke(this);
		}

		public void Resume()
		{
			IsPausing = false;
		}

		public void Clear()
		{
			MainThread.Clear();
			CurrentGallerySceneLabel = "";
		}

		internal void UpdateSceneGallery(string label, AdvEngine engine)
		{
			if (engine.DataManager.SettingDataManager.SceneGallerySetting.Contains(label) && CurrentGallerySceneLabel != label)
			{
				if (!string.IsNullOrEmpty(CurrentGallerySceneLabel))
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.UpdateSceneLabel, CurrentGallerySceneLabel, label));
				}
				CurrentGallerySceneLabel = label;
			}
		}

		public void EndSceneGallery(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(CurrentGallerySceneLabel))
			{
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndSceneGallery));
				return;
			}
			engine.SystemSaveData.GalleryData.AddSceneLabel(CurrentGallerySceneLabel);
			CurrentGallerySceneLabel = "";
		}
	}
}
