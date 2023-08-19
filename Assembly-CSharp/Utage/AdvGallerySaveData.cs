using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvGallerySaveData : IBinaryIO
	{
		private List<string> eventSceneLabels = new List<string>();

		private List<string> eventCGLabels = new List<string>();

		private const int VERSION = 0;

		public string SaveKey
		{
			get
			{
				return "AdvGallerySaveData";
			}
		}

		public void AddCgLabel(string label)
		{
			if (!CheckCgLabel(label))
			{
				eventCGLabels.Add(label);
			}
		}

		public void AddSceneLabel(string label)
		{
			if (!CheckSceneLabels(label))
			{
				eventSceneLabels.Add(label);
			}
		}

		public bool CheckSceneLabels(string label)
		{
			return eventSceneLabels.Contains(label);
		}

		public bool CheckCgLabel(string label)
		{
			return eventCGLabels.Contains(label);
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				eventSceneLabels.Clear();
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					eventSceneLabels.Add(reader.ReadString());
				}
				eventCGLabels.Clear();
				num2 = reader.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					eventCGLabels.Add(reader.ReadString());
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(eventSceneLabels.Count);
			foreach (string eventSceneLabel in eventSceneLabels)
			{
				writer.Write(eventSceneLabel);
			}
			writer.Write(eventCGLabels.Count);
			foreach (string eventCGLabel in eventCGLabels)
			{
				writer.Write(eventCGLabel);
			}
		}
	}
}
