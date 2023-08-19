using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utage
{
	public class AdvBacklog
	{
		private class AdvBacklogDataInPage
		{
			public string LogText { get; private set; }

			public string CharacterLabel { get; private set; }

			public string CharacterNameText { get; private set; }

			public string VoiceFileName { get; private set; }

			public AdvBacklogDataInPage()
			{
				LogText = "";
				CharacterLabel = "";
				CharacterNameText = "";
				VoiceFileName = "";
			}

			public AdvBacklogDataInPage(AdvCommandText dataInPage, AdvCharacterInfo characterInfo)
			{
				LogText = "";
				VoiceFileName = "";
				if (characterInfo != null)
				{
					CharacterLabel = characterInfo.Label;
					CharacterNameText = characterInfo.LocalizeNameText;
				}
				else
				{
					CharacterLabel = "";
					CharacterNameText = "";
				}
				LogText = TextParser.MakeLogText(dataInPage.ParseCellLocalizedText());
				if (dataInPage.VoiceFile != null)
				{
					VoiceFileName = dataInPage.VoiceFile.FileName;
					LogText = TextParser.AddTag(LogText, "sound", dataInPage.VoiceFile.FileName);
				}
				else
				{
					VoiceFileName = "";
				}
				if (dataInPage.IsNextBr)
				{
					LogText += "\n";
				}
			}

			internal void Write(BinaryWriter writer)
			{
				writer.Write(LogText);
				writer.Write(CharacterLabel);
				writer.Write(CharacterNameText);
				writer.Write(VoiceFileName);
			}

			internal void Read(BinaryReader reader, int version)
			{
				LogText = reader.ReadString();
				CharacterLabel = reader.ReadString();
				CharacterNameText = reader.ReadString();
				VoiceFileName = reader.ReadString();
			}
		}

		private List<AdvBacklogDataInPage> dataList = new List<AdvBacklogDataInPage>();

		private const int Version = 0;

		public bool IsEmpty
		{
			get
			{
				return dataList.Count <= 0;
			}
		}

		public string Text
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (AdvBacklogDataInPage data in dataList)
				{
					stringBuilder.Append(data.LogText);
				}
				return stringBuilder.ToString().TrimEnd('\n');
			}
		}

		public string MainCharacterNameText
		{
			get
			{
				foreach (AdvBacklogDataInPage data in dataList)
				{
					if (!string.IsNullOrEmpty(data.CharacterNameText))
					{
						return data.CharacterNameText;
					}
				}
				return "";
			}
		}

		public string MainVoiceFileName
		{
			get
			{
				foreach (AdvBacklogDataInPage data in dataList)
				{
					if (!string.IsNullOrEmpty(data.VoiceFileName))
					{
						return data.VoiceFileName;
					}
				}
				return "";
			}
		}

		internal int CountVoice
		{
			get
			{
				int num = 0;
				foreach (AdvBacklogDataInPage data in dataList)
				{
					if (!string.IsNullOrEmpty(data.VoiceFileName))
					{
						num++;
					}
				}
				return num;
			}
		}

		internal void AddData(AdvCommandText log, AdvCharacterInfo characterInfo)
		{
			dataList.Add(new AdvBacklogDataInPage(log, characterInfo));
		}

		public string FindCharacerLabel(string voiceFileName)
		{
			foreach (AdvBacklogDataInPage data in dataList)
			{
				if (data.VoiceFileName == voiceFileName)
				{
					return data.CharacterLabel;
				}
			}
			return "";
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(dataList.Count);
			foreach (AdvBacklogDataInPage data in dataList)
			{
				writer.Write(data.LogText);
				writer.Write(data.CharacterLabel);
				writer.Write(data.CharacterNameText);
				writer.Write(data.VoiceFileName);
			}
		}

		internal void Read(BinaryReader reader)
		{
			dataList.Clear();
			int num = reader.ReadInt32();
			if (num == 0)
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					AdvBacklogDataInPage advBacklogDataInPage = new AdvBacklogDataInPage();
					advBacklogDataInPage.Read(reader, num);
					dataList.Add(advBacklogDataInPage);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
