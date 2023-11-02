using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/BacklogManager")]
	public class AdvBacklogManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		[SerializeField]
		private int maxLog = 10;

		[SerializeField]
		private bool ignoreLog;

		[SerializeField]
		private BacklogEvent onAddPage = new BacklogEvent();

		[SerializeField]
		private BacklogEvent onAddData = new BacklogEvent();

		private List<AdvBacklog> backlogs = new List<AdvBacklog>();

		private const int Version = 0;

		public int MaxLog => maxLog;

		public bool IgnoreLog
		{
			get
			{
				return ignoreLog;
			}
			set
			{
				ignoreLog = value;
			}
		}

		public BacklogEvent OnAddPage => onAddPage;

		public BacklogEvent OnAddData => onAddData;

		public List<AdvBacklog> Backlogs => backlogs;

		public AdvBacklog LastLog
		{
			get
			{
				if (Backlogs.Count <= 0)
				{
					return null;
				}
				return Backlogs[Backlogs.Count - 1];
			}
		}

		public string SaveKey => "BacklogManager";

		public void Clear()
		{
			backlogs.Clear();
		}

		internal void AddPage()
		{
			onAddPage.Invoke(this);
			if (!IgnoreLog)
			{
				AddLog(new AdvBacklog());
			}
		}

		private void AddLog(AdvBacklog log)
		{
			if (!IgnoreLog)
			{
				backlogs.Add(log);
				if (backlogs.Count > MaxLog)
				{
					backlogs.RemoveAt(0);
				}
			}
		}

		internal void AddCurrentPageLog(AdvCommandText dataInPage, AdvCharacterInfo characterInfo)
		{
			onAddData.Invoke(this);
			if (!IgnoreLog)
			{
				LastLog?.AddData(dataInPage, characterInfo);
			}
		}

		public void OnClear()
		{
			Clear();
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(Backlogs.Count);
			foreach (AdvBacklog backlog in Backlogs)
			{
				backlog.Write(writer);
			}
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					AdvBacklog advBacklog = new AdvBacklog();
					advBacklog.Read(reader);
					if (!advBacklog.IsEmpty)
					{
						AddLog(advBacklog);
					}
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
