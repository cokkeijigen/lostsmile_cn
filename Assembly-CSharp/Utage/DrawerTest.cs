using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Sample/DrawerTest")]
	public class DrawerTest : MonoBehaviour
	{
		[Serializable]
		public class DecoratorTest
		{
			[Flags]
			public enum Flags
			{
				Flag0 = 1,
				Flag1 = 2,
				Flag2 = 4
			}

			public enum LimitEnum
			{
				Type0,
				Type1,
				Type2
			}

			[HelpBox("HelpBoxは、ヘルプボックスを表示するデコレーターです。\n何行にもわたるような、長いテキストにも自動改行で対応します", HelpBoxAttribute.Type.Warning, 0)]
			public string helpBox;

			[HelpBox("Hide。非表示にします", HelpBoxAttribute.Type.Info, 0)]
			[Hide("")]
			public int hide;

			[HelpBox("NotEditable。表示のみで編集を不可能にします", HelpBoxAttribute.Type.Info, 0)]
			[NotEditable]
			public int notEditable;

			[HelpBox("EnumFlags。フラグタイプのenum表示です。マスク（チェックボックス）表示になります", HelpBoxAttribute.Type.Info, 0)]
			[EnumFlags]
			public Flags flags;

			[HelpBox("LimitEnum。enumのうち限られたものだけ表示します", HelpBoxAttribute.Type.Info, 0)]
			[LimitEnum(new string[] { "Type0", "Type2" })]
			public LimitEnum lmitEnum;

			[HelpBox("StringPopup。指定の文字列のポップアップリストを表示します", HelpBoxAttribute.Type.Info, 0)]
			[StringPopup(new string[] { "hoge", "hoge2" })]
			public string stringPopup;

			[HelpBox("StringPopupFunction。指定した名前の関数から取得できる、ポップアップリストを表示します", HelpBoxAttribute.Type.Info, 0)]
			[StringPopupFunction("GetStrings")]
			public string stringPopupFunction;

			[HelpBox("ボタンを表示します。", HelpBoxAttribute.Type.Info, 0)]
			[Button("OnPushButton", "Push!", 0)]
			public string pushButton = "HogeHoge!";

			[HelpBox("プロパティの横にボタンを追加します。", HelpBoxAttribute.Type.Info, 0)]
			[AddButton("OnPushAddButton", " Add Button!", 0)]
			public string addButton;

			[HelpBox("パスの文字列を設定するために、ファイルダイアログを開きます", HelpBoxAttribute.Type.Info, 0)]
			[PathDialog(PathDialogAttribute.DialogType.File)]
			public string path;

			[HelpBox("指定範囲のMinMax値を設定", HelpBoxAttribute.Type.Info, 0)]
			[SerializeField]
			[MinMax(0f, 10f, "min", "max")]
			private MinMaxFloat intervalTime = new MinMaxFloat
			{
				Min = 3f,
				Max = 5f
			};

			[SerializeField]
			[MinMax(0f, 10f, "min", "max")]
			private MinMaxInt intervalTimeInt = new MinMaxInt
			{
				Min = 3,
				Max = 5
			};

			[HelpBox("OverridePropertyDraw。プロパティドロワーを独自のメソッドで上書きします", HelpBoxAttribute.Type.Info, 0)]
			[SerializeField]
			[OverridePropertyDraw("OnGuiOverridePropertyDraw")]
			private int overridePropertyDraw;
		}

		[Header("ヘッダー表示")]
		[HelpBox("HelpBoxは、ヘルプボックスを表示するデコレーターです。\n何行にもわたるような、長いテキストにも自動改行で対応します", HelpBoxAttribute.Type.Info, 0)]
		public string helpBox;

		public DecoratorTest decoratorTest;

		[SerializeField]
		private bool isOverridePropertyDrawEditable;

		public List<string> GetStrings()
		{
			return new List<string> { "str0", "str1" };
		}

		public void OnPushButton()
		{
			Debug.Log("OnPushButton");
		}

		public void OnPushAddButton()
		{
			Debug.Log("OnPushAddButton");
		}
	}
}
