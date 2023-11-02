using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvImportScenarios : ScriptableObject
	{
		private const int Version = 3;

		[SerializeField]
		private int importVersion;

		[SerializeField]
		private List<AdvChapterData> chapters = new List<AdvChapterData>();

		public List<AdvChapterData> Chapters => chapters;

		public bool CheckVersion()
		{
			return importVersion == 3;
		}

		public void AddChapter(AdvChapterData chapterData)
		{
			Chapters.Add(chapterData);
		}

		public bool TryAddChapter(AdvChapterData chapterData)
		{
			if (Chapters.Exists((AdvChapterData x) => x.name == chapterData.name))
			{
				return false;
			}
			Chapters.Add(chapterData);
			return true;
		}
	}
}
