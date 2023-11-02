using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvImportBook : ScriptableObject
	{
		private const int Version = 0;

		[SerializeField]
		private int importVersion;

		[SerializeField]
		private List<AdvImportScenarioSheet> importGridList = new List<AdvImportScenarioSheet>();

		public List<AdvImportScenarioSheet> ImportGridList => importGridList;

		public bool CheckVersion()
		{
			return importVersion == 0;
		}

		public void BootInit()
		{
			foreach (AdvImportScenarioSheet importGrid in ImportGridList)
			{
				importGrid.InitLink();
			}
		}
	}
}
