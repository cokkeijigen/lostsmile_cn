using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/ScenarioThread")]
	public class AdvScenarioThread : MonoBehaviour
	{
		[SerializeField]
		[NotEditable]
		private string threadName;

		private AdvIfManager ifManager = new AdvIfManager();

		private AdvJumpManager jumpManager = new AdvJumpManager();

		private AdvWaitManager waitManager = new AdvWaitManager();

		private List<AdvScenarioThread> subThreadList = new List<AdvScenarioThread>();

		private HashSet<AssetFile> preloadFileSet = new HashSet<AssetFile>();

		private AdvCommand currentCommand;

		public string ThreadName
		{
			get
			{
				return threadName;
			}
		}

		public bool IsMainThread { get; private set; }

		public bool IsLoading { get; private set; }

		public bool IsLoadingDeep
		{
			get
			{
				if (IsLoading)
				{
					return true;
				}
				foreach (AdvScenarioThread subThread in SubThreadList)
				{
					if (subThread.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsPlaying { get; set; }

		internal AdvIfManager IfManager
		{
			get
			{
				return ifManager;
			}
		}

		internal AdvJumpManager JumpManager
		{
			get
			{
				return jumpManager;
			}
		}

		internal AdvWaitManager WaitManager
		{
			get
			{
				return waitManager;
			}
		}

		private AdvScenarioThread ParenetThread { get; set; }

		private List<AdvScenarioThread> SubThreadList
		{
			get
			{
				return subThreadList;
			}
		}

		public AdvCommand CurrentCommand
		{
			get
			{
				return currentCommand;
			}
		}

		internal bool SkipPageHeaerOnSave { get; private set; }

		internal AdvScenarioPlayer ScenarioPlayer { get; private set; }

		internal AdvEngine Engine
		{
			get
			{
				return ScenarioPlayer.Engine;
			}
		}

		private bool IsBreakCommand
		{
			get
			{
				if (IsPlaying && !JumpManager.IsReserved)
				{
					if (IsMainThread)
					{
						return ScenarioPlayer.IsReservedEndScenario;
					}
					return false;
				}
				return true;
			}
		}

		public bool IsCurrentCommand(AdvCommand command)
		{
			if (command != null)
			{
				return currentCommand == command;
			}
			return false;
		}

		internal void Init(AdvScenarioPlayer scenarioPlayer, string name, AdvScenarioThread parent)
		{
			ScenarioPlayer = scenarioPlayer;
			threadName = name;
			ParenetThread = parent;
			IsMainThread = parent == null;
		}

		private void OnDestroy()
		{
			ClearPreload();
			CleaSubTreadList();
			if ((bool)ParenetThread)
			{
				ParenetThread.SubThreadList.Remove(this);
			}
		}

		internal void Clear()
		{
			IsPlaying = false;
			CleaSubTreadList();
			ResetOnJump();
			WaitManager.Clear();
			jumpManager.Clear();
			StopAllCoroutines();
		}

		internal void Cancel()
		{
			Clear();
			Object.Destroy(this);
		}

		private void ResetOnJump()
		{
			IsLoading = false;
			ifManager.Clear();
			jumpManager.ClearOnJump();
			ClearPreload();
		}

		internal void StartScenario(string label, int page, bool skipPageHeaer)
		{
			StartCoroutine(CoStartScenario(label, page, null, skipPageHeaer));
		}

		private IEnumerator CoStartScenario(string label, int page, AdvCommand returnToCommand, bool skipPageHeaer)
		{
			IsPlaying = true;
			SkipPageHeaerOnSave = false;
			if (ScenarioPlayer.DebugOutputLog)
			{
				Debug.Log("Jump : " + label + " :" + page);
			}
			while (Engine.IsLoading)
			{
				yield return null;
			}
			IsLoading = true;
			while (!Engine.DataManager.IsLoadEndScenarioLabel(label))
			{
				yield return null;
			}
			IsLoading = false;
			ResetOnJump();
			if (page < 0)
			{
				page = 0;
			}
			if (page != 0)
			{
				ifManager.IsLoadInit = true;
			}
			AdvScenarioLabelData currentLabelData = Engine.DataManager.FindScenarioLabelData(label);
			while (currentLabelData != null)
			{
				ScenarioPlayer.UpdateSceneGallery(currentLabelData.ScenarioLabel, Engine);
				AdvScenarioPageData pageData = currentLabelData.GetPageData(page);
				while (pageData != null)
				{
					UpdatePreLoadFiles(currentLabelData.ScenarioLabel, page);
					if (IsMainThread)
					{
						Engine.Page.BeginPage(pageData);
					}
					Coroutine coroutine = StartCoroutine(CoStartPage(currentLabelData, pageData, returnToCommand, skipPageHeaer));
					if (coroutine != null)
					{
						yield return coroutine;
					}
					currentCommand = null;
					returnToCommand = null;
					skipPageHeaer = false;
					if (IsMainThread)
					{
						Engine.Page.EndPage();
					}
					if (IsBreakCommand)
					{
						if (IsMainThread && ScenarioPlayer.IsReservedEndScenario)
						{
							ScenarioPlayer.EndScenario();
						}
						else if (JumpManager.IsReserved)
						{
							JumpToReserved();
						}
						else
						{
							OnEndThread();
						}
						yield break;
					}
					AdvScenarioLabelData advScenarioLabelData = currentLabelData;
					int num = page + 1;
					page = num;
					pageData = advScenarioLabelData.GetPageData(num);
				}
				IfManager.IsLoadInit = false;
				currentLabelData = Engine.DataManager.NextScenarioLabelData(currentLabelData.ScenarioLabel);
				page = 0;
			}
			OnEndThread();
		}

		private void OnEndThread()
		{
			IsPlaying = false;
			if (IsMainThread)
			{
				ScenarioPlayer.EndScenario();
			}
			else
			{
				Object.Destroy(this);
			}
		}

		private IEnumerator CoStartPage(AdvScenarioLabelData labelData, AdvScenarioPageData pageData, AdvCommand returnToCommand, bool skipPageHeaer)
		{
			int index = (skipPageHeaer ? pageData.IndexTextTopCommand : 0);
			AdvCommand command = pageData.GetCommand(index);
			if (returnToCommand != null)
			{
				while (command != returnToCommand)
				{
					int num = index + 1;
					index = num;
					command = pageData.GetCommand(num);
				}
			}
			if (IfManager.IsLoadInit)
			{
				index = pageData.GetIfSkipCommandIndex(index);
				command = pageData.GetCommand(index);
			}
			if (EnableSaveOnPageTop() && pageData.EnableSave)
			{
				SkipPageHeaerOnSave = false;
				Engine.SaveManager.UpdateAutoSaveData(Engine);
			}
			CheckSystemDataWriteIfChanged();
			while (command != null)
			{
				if (command.IsEntityType)
				{
					command = AdvEntityData.CreateEntityCommand(command, Engine, pageData);
				}
				int num;
				if (IfManager.CheckSkip(command))
				{
					if (ScenarioPlayer.DebugOutputLog)
					{
						Debug.Log(string.Concat("Command If Skip: ", command.GetType(), " ", labelData.ScenarioLabel, ":", pageData.PageNo));
					}
					num = index + 1;
					index = num;
					command = pageData.GetCommand(num);
					continue;
				}
				currentCommand = command;
				command.Load();
				if (EnableSaveTextTop() && pageData.EnableSaveTextTop(command))
				{
					SkipPageHeaerOnSave = true;
					Engine.SaveManager.UpdateAutoSaveData(Engine);
					CheckSystemDataWriteIfChanged();
				}
				while (!command.IsLoadEnd())
				{
					IsLoading = true;
					yield return null;
				}
				IsLoading = false;
				command.CurrentTread = this;
				if (ScenarioPlayer.DebugOutputLog)
				{
					Debug.Log(string.Concat("Command : ", command.GetType(), " ", labelData.ScenarioLabel, ":", pageData.PageNo));
				}
				ScenarioPlayer.OnBeginCommand.Invoke(command);
				command.DoCommand(Engine);
				command.Unload();
				command.CurrentTread = null;
				while (ScenarioPlayer.IsPausing)
				{
					yield return null;
				}
				while (true)
				{
					command.CurrentTread = this;
					ScenarioPlayer.OnUpdatePreWaitingCommand.Invoke(command);
					if (!command.Wait(Engine))
					{
						break;
					}
					if (ScenarioPlayer.DebugOutputWaiting)
					{
						Debug.Log("Wait..." + command.GetType());
					}
					ScenarioPlayer.OnUpdateWaitingCommand.Invoke(command);
					command.CurrentTread = null;
					yield return null;
				}
				command.CurrentTread = this;
				if (ScenarioPlayer.DebugOutputCommandEnd)
				{
					Debug.Log(string.Concat("End :", command.GetType(), " ", labelData.ScenarioLabel, ":", pageData.PageNo));
				}
				ScenarioPlayer.OnEndCommand.Invoke(command);
				command.CurrentTread = null;
				Engine.UiManager.IsInputTrig = false;
				Engine.UiManager.IsInputTrigCustom = false;
				if (IsBreakCommand)
				{
					break;
				}
				num = index + 1;
				index = num;
				command = pageData.GetCommand(num);
			}
		}

		private void CheckSystemDataWriteIfChanged()
		{
			if (Engine.Param.HasChangedSystemParam)
			{
				Engine.Param.HasChangedSystemParam = false;
				Engine.SystemSaveData.Write();
			}
		}

		internal bool EnableSaveOnPageTop()
		{
			if (!IsMainThread)
			{
				return false;
			}
			if (Engine.IsSceneGallery)
			{
				return false;
			}
			switch (Engine.SaveManager.Type)
			{
			case AdvSaveManager.SaveType.Default:
				return true;
			case AdvSaveManager.SaveType.SavePoint:
				if (Engine.Page.PageNo == 0)
				{
					return Engine.Page.CurrentData.ScenarioLabelData.IsSavePoint;
				}
				return false;
			default:
				return false;
			}
		}

		internal bool EnableSaveTextTop()
		{
			if (!IsMainThread)
			{
				return false;
			}
			if (Engine.IsSceneGallery)
			{
				return false;
			}
			if (WaitManager.IsWaiting)
			{
				return false;
			}
			int count = SubThreadList.Count;
			int num = 0;
			return false;
		}

		private void JumpToReserved()
		{
			StopAllCoroutines();
			if (JumpManager.SubRoutineReturnInfo != null)
			{
				SubRoutineInfo subRoutineReturnInfo = JumpManager.SubRoutineReturnInfo;
				StartCoroutine(CoStartScenario(subRoutineReturnInfo.ReturnLabel, subRoutineReturnInfo.ReturnPageNo, subRoutineReturnInfo.ReturnCommand, false));
			}
			else
			{
				StartCoroutine(CoStartScenario(JumpManager.Label, 0, null, false));
			}
		}

		internal void StartSubThread(string label, string name)
		{
			AdvScenarioThread advScenarioThread = base.gameObject.AddComponent<AdvScenarioThread>();
			advScenarioThread.Init(ScenarioPlayer, name, this);
			SubThreadList.Add(advScenarioThread);
			advScenarioThread.StartScenario(label, 0, false);
		}

		internal bool IsPlayingSubThread(string name)
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				if ((bool)subThread && subThread.ThreadName == name)
				{
					return subThread.IsPlaying;
				}
			}
			return false;
		}

		internal void CleaSubTreadList()
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				Object.Destroy(subThread);
			}
			SubThreadList.Clear();
		}

		internal void CancelSubThread(string name)
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				if ((bool)subThread && subThread.ThreadName == name)
				{
					subThread.Cancel();
				}
			}
		}

		private void ClearPreload()
		{
			foreach (AssetFile item in preloadFileSet)
			{
				item.Unuse(this);
			}
			preloadFileSet.Clear();
		}

		private void UpdatePreLoadFiles(string scenarioLabel, int page)
		{
			HashSet<AssetFile> hashSet = preloadFileSet;
			preloadFileSet = Engine.DataManager.MakePreloadFileList(scenarioLabel, page, ScenarioPlayer.MaxFilePreload, ScenarioPlayer.PreloadDeep);
			if (preloadFileSet == null)
			{
				preloadFileSet = new HashSet<AssetFile>();
			}
			foreach (AssetFile item in preloadFileSet)
			{
				AssetFileManager.Preload(item, this);
			}
			foreach (AssetFile item2 in hashSet)
			{
				if (!preloadFileSet.Contains(item2))
				{
					item2.Unuse(this);
				}
			}
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}
	}
}
