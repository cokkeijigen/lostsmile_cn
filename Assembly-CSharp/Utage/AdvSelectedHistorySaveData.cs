using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvSelectedHistorySaveData : IBinaryIO
	{
		private class AdvSelectedHistoryData
		{
			private string Label { get; set; }

			private string Text { get; set; }

			private string JumpLabel { get; set; }

			public AdvSelectedHistoryData(AdvSelection selection)
			{
				Label = selection.Label;
				Text = selection.Text;
				JumpLabel = selection.JumpLabel;
			}

			public bool Check(AdvSelection selection)
			{
				if (!string.IsNullOrEmpty(Label) || !string.IsNullOrEmpty(selection.Label))
				{
					return Label == selection.Label;
				}
				if (Text == selection.Text)
				{
					return JumpLabel == selection.JumpLabel;
				}
				return false;
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(Label);
				writer.Write(Text);
				writer.Write(JumpLabel);
			}

			public AdvSelectedHistoryData(BinaryReader reader, int version)
			{
				if (version == 0)
				{
					Label = reader.ReadString();
					Text = reader.ReadString();
					JumpLabel = reader.ReadString();
				}
				else
				{
					Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
				}
			}
		}

		private List<AdvSelectedHistoryData> dataList = new List<AdvSelectedHistoryData>();

		private const string Ignore = "Alaways";

		private const int VERSION = 0;

		public string SaveKey
		{
			get
			{
				return "AdvSelectedHistorySaveData";
			}
		}

		public void AddData(AdvSelection selection)
		{
			if (!(selection.Label == "Alaways") && !Check(selection))
			{
				dataList.Add(new AdvSelectedHistoryData(selection));
			}
		}

		public bool Check(AdvSelection selection)
		{
			if (selection.Label == "Alaways")
			{
				return false;
			}
			return dataList.Find((AdvSelectedHistoryData x) => x.Check(selection)) != null;
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(dataList.Count);
			foreach (AdvSelectedHistoryData data in dataList)
			{
				data.Write(writer);
			}
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				dataList.Clear();
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					dataList.Add(new AdvSelectedHistoryData(reader, num));
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
