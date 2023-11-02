using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class iTweenData
	{
		public const string Time = "time";

		public const string Delay = "delay";

		public const string Speed = "speed";

		public const string X = "x";

		public const string Y = "y";

		public const string Z = "z";

		public const string Color = "color";

		public const string R = "r";

		public const string G = "g";

		public const string B = "b";

		public const string A = "a";

		public const string Alpha = "alpha";

		public const string Islocal = "islocal";

		public const string EaseType = "easeType";

		public const string LoopType = "loopType";

		private iTweenType type;

		private iTween.LoopType loopType;

		private int loopCount;

		private Dictionary<string, object> hashObjects = new Dictionary<string, object>();

		private string errorMsg = "";

		private string strType;

		private string strArg;

		private string strEaseType;

		private string strLoopType;

		public static Func<string, object> CallbackGetValue;

		private bool isDynamic;

		private static readonly string[][] ArgTbl = new string[21][]
		{
			new string[8] { "time", "delay", "color", "alpha", "r", "g", "b", "a" },
			new string[8] { "time", "delay", "color", "alpha", "r", "g", "b", "a" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[7] { "time", "delay", "x", "y", "z", "speed", "islocal" },
			new string[7] { "time", "delay", "x", "y", "z", "speed", "islocal" },
			new string[5] { "time", "delay", "x", "y", "z" },
			new string[5] { "time", "delay", "x", "y", "z" },
			new string[5] { "time", "delay", "x", "y", "z" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[7] { "time", "delay", "x", "y", "z", "speed", "islocal" },
			new string[7] { "time", "delay", "x", "y", "z", "speed", "islocal" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "speed" },
			new string[6] { "time", "delay", "x", "y", "z", "islocal" },
			new string[5] { "time", "delay", "x", "y", "z" },
			new string[5] { "time", "delay", "x", "y", "z" },
			new string[0]
		};

		public iTweenType Type => type;

		public iTween.LoopType Loop => loopType;

		public int LoopCount => loopCount;

		public Dictionary<string, object> HashObjects => hashObjects;

		public string ErrorMsg => errorMsg;

		public bool IsDynamic => isDynamic;

		public bool IsEndlessLoop
		{
			get
			{
				if (loopType != 0)
				{
					return loopCount <= 0;
				}
				return false;
			}
		}

		public bool IsSupportLocal
		{
			get
			{
				string[] array = ArgTbl[(int)Type];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == "islocal")
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsLocal
		{
			get
			{
				if (HashObjects.ContainsKey("islocal"))
				{
					return (bool)HashObjects["islocal"];
				}
				if (IsSupportLocal)
				{
					return false;
				}
				Debug.LogError("Not Support Local type");
				return false;
			}
			set
			{
				HashObjects["islocal"] = value;
			}
		}

		public object[] MakeHashArray()
		{
			List<object> list = new List<object>();
			foreach (KeyValuePair<string, object> hashObject in HashObjects)
			{
				list.Add(hashObject.Key);
				list.Add(hashObject.Value);
			}
			return list.ToArray();
		}

		public iTweenData(string type, string arg, string easeType, string loopType)
		{
			Init(type, arg, easeType, loopType);
		}

		public iTweenData(iTweenType type, string arg)
		{
			Init(type.ToString(), arg, "", "");
		}

		public void ReInit()
		{
			if (isDynamic)
			{
				HashObjects.Clear();
				Init(strType, strArg, strEaseType, strLoopType);
			}
		}

		private void Init(string type, string arg, string easeType, string loopType)
		{
			strType = type;
			strArg = arg;
			strEaseType = easeType;
			strLoopType = loopType;
			ParseParameters(type, arg);
			if (!string.IsNullOrEmpty(easeType))
			{
				HashObjects.Add("easeType", easeType);
			}
			if (!string.IsNullOrEmpty(loopType))
			{
				try
				{
					ParseLoopType(loopType);
					HashObjects.Add("loopType", this.loopType);
				}
				catch (Exception ex)
				{
					AddErrorMsg(loopType + "は、LoopTypeとして解析できません。");
					AddErrorMsg(ex.Message);
				}
			}
		}

		private void ParseParameters(string type, string arg)
		{
			try
			{
				this.type = (iTweenType)Enum.Parse(typeof(iTweenType), type);
				if (this.type == iTweenType.Stop)
				{
					return;
				}
				char[] separator = new char[2] { ' ', '=' };
				string[] array = arg.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length % 2 != 0 || array.Length == 0)
				{
					AddErrorMsg(arg + "内が、「パラメーター名=値」 の形式で書かれていません。");
					return;
				}
				for (int i = 0; i < array.Length / 2; i++)
				{
					string text = array[i * 2];
					HashObjects.Add(text, ParseValue(this.type, text, array[i * 2 + 1], ref isDynamic));
				}
			}
			catch (Exception ex)
			{
				AddErrorMsg(arg + "内が、「パラメーター名=値」 の形式で書かれていません。");
				AddErrorMsg(ex.Message);
			}
		}

		private void AddErrorMsg(string msg)
		{
			if (!string.IsNullOrEmpty(errorMsg))
			{
				errorMsg += "\n";
			}
			errorMsg += msg;
		}

		private void ParseLoopType(string loopTypeStr)
		{
			loopType = iTween.LoopType.none;
			loopCount = 0;
			char[] separator = new char[2] { ' ', '=' };
			string[] array = loopTypeStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				loopType = (iTween.LoopType)Enum.Parse(typeof(iTween.LoopType), array[0]);
				loopCount = int.Parse(array[1]);
				return;
			}
			throw new Exception();
		}

		public void Write(BinaryWriter writer)
		{
			if (!IsEndlessLoop)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TweenWrite));
			}
			writer.Write(strType);
			writer.Write(strArg);
			writer.Write(strEaseType);
			writer.Write(strLoopType);
		}

		public iTweenData(BinaryReader reader)
		{
			string text = reader.ReadString();
			string arg = reader.ReadString();
			string easeType = reader.ReadString();
			string text2 = reader.ReadString();
			Init(text, arg, easeType, text2);
		}

		private static object ParseValue(iTweenType type, string name, string valueString, ref bool isDynamic)
		{
			object obj = null;
			if (CallbackGetValue != null)
			{
				obj = CallbackGetValue(valueString);
				isDynamic = true;
			}
			if (CheckArg(type, name))
			{
				switch (name)
				{
				case "time":
				case "delay":
				case "speed":
				case "alpha":
				case "r":
				case "g":
				case "b":
				case "a":
				case "x":
				case "y":
				case "z":
					if (obj != null)
					{
						return (float)obj;
					}
					return WrapperUnityVersion.ParseFloatGlobal(valueString);
				case "islocal":
					if (obj != null)
					{
						return (bool)obj;
					}
					return bool.Parse(valueString);
				case "color":
					return ColorUtil.ParseColor(valueString);
				default:
					return null;
				}
			}
			return null;
		}

		private static bool CheckArg(iTweenType type, string name)
		{
			return Array.IndexOf(ArgTbl[(int)type], name) >= 0;
		}

		public static bool IsPostionType(iTweenType type)
		{
			if ((uint)(type - 2) <= 4u || type == iTweenType.ShakePosition)
			{
				return true;
			}
			return false;
		}
	}
}
