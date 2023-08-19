using System.IO;

namespace Utage
{
	public class AdvMessageWindow
	{
		public string Name { get; protected set; }

		public TextData Text { get; protected set; }

		public string NameText { get; protected set; }

		public string CharacterLabel { get; protected set; }

		public int TextLength { get; protected set; }

		protected IAdvMessageWindow MessageWindow { get; set; }

		public AdvMessageWindow(IAdvMessageWindow messageWindow)
		{
			MessageWindow = messageWindow;
			Name = MessageWindow.gameObject.name;
			Clear();
		}

		private void Clear()
		{
			Text = new TextData("");
			NameText = "";
			CharacterLabel = "";
			TextLength = -1;
		}

		public virtual void ChangeActive(bool isActive)
		{
			if (!isActive)
			{
				Clear();
			}
			MessageWindow.OnChangeActive(isActive);
		}

		internal void Reset()
		{
			Clear();
			MessageWindow.OnReset();
		}

		internal void ChangeCurrent(bool isCurrent)
		{
			MessageWindow.OnChangeCurrent(isCurrent);
		}

		internal void PageTextChange(AdvPage page)
		{
			Text = page.TextData;
			NameText = page.NameText;
			CharacterLabel = page.CharacterLabel;
			TextLength = page.CurrentTextLength;
			MessageWindow.OnTextChanged(this);
		}

		internal void ReadPageData(BinaryReader reader)
		{
			Text = new TextData(reader.ReadString());
			NameText = reader.ReadString();
			CharacterLabel = reader.ReadString();
			TextLength = -1;
			MessageWindow.OnTextChanged(this);
		}

		internal void WritePageData(BinaryWriter writer)
		{
			writer.Write(Text.OriginalText);
			writer.Write(NameText);
			writer.Write(CharacterLabel);
		}
	}
}
