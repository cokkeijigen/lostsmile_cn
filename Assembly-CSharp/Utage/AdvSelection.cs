using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvSelection
	{
		private string text;

		private string jumpLabel;

		private ExpressionParser exp;

		private string spriteName = "";

		private bool isPolygon;

		private StringGridRow row;

		private const int VERSION = 2;

		private const int VERSION_1 = 1;

		private const int VERSION_0 = 0;

		public string Label { get; private set; }

		public string Text
		{
			get
			{
				return text;
			}
		}

		public string JumpLabel
		{
			get
			{
				return jumpLabel;
			}
		}

		public ExpressionParser Exp
		{
			get
			{
				return exp;
			}
		}

		public string PrefabName { get; protected set; }

		public float? X { get; protected set; }

		public float? Y { get; protected set; }

		public string SpriteName
		{
			get
			{
				return spriteName;
			}
		}

		public bool IsPolygon
		{
			get
			{
				return isPolygon;
			}
		}

		public StringGridRow RowData
		{
			get
			{
				return row;
			}
		}

		public AdvSelection(string jumpLabel, string text, ExpressionParser exp, string prefabName, float? x, float? y, StringGridRow row)
		{
			Label = "";
			this.jumpLabel = jumpLabel;
			this.text = text;
			this.exp = exp;
			PrefabName = prefabName;
			X = x;
			Y = y;
			this.row = row;
		}

		public AdvSelection(string jumpLabel, string spriteName, bool isPolygon, ExpressionParser exp, StringGridRow row)
		{
			Label = "";
			this.jumpLabel = jumpLabel;
			text = "";
			this.exp = exp;
			this.spriteName = spriteName;
			this.isPolygon = isPolygon;
			this.row = row;
		}

		public AdvSelection(BinaryReader reader, AdvEngine engine)
		{
			Read(reader, engine);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(2);
			writer.Write(jumpLabel);
			writer.Write(text);
			if (Exp != null)
			{
				writer.Write(Exp.Exp);
			}
			else
			{
				writer.Write("");
			}
			writer.Write(spriteName);
			writer.Write(isPolygon);
		}

		private void Read(BinaryReader reader, AdvEngine engine)
		{
			int num = reader.ReadInt32();
			switch (num)
			{
			case 2:
			{
				jumpLabel = reader.ReadString();
				text = reader.ReadString();
				string value2 = reader.ReadString();
				if (!string.IsNullOrEmpty(value2))
				{
					exp = engine.DataManager.SettingDataManager.DefaultParam.CreateExpression(value2);
				}
				else
				{
					exp = null;
				}
				spriteName = reader.ReadString();
				isPolygon = reader.ReadBoolean();
				break;
			}
			case 1:
			{
				jumpLabel = reader.ReadString();
				text = reader.ReadString();
				string value = reader.ReadString();
				if (!string.IsNullOrEmpty(value))
				{
					exp = engine.DataManager.SettingDataManager.DefaultParam.CreateExpression(value);
				}
				else
				{
					exp = null;
				}
				break;
			}
			case 0:
				jumpLabel = reader.ReadString();
				text = reader.ReadString();
				break;
			default:
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				break;
			}
		}
	}
}
