using UnityEngine;

namespace Utage
{
	public class CharData
	{
		public enum HitEventType
		{
			Sound,
			Link,
			Tips
		}

		public class CustomCharaInfo
		{
			public Color color;

			public string colorStr;

			public int size;

			public string rubyStr;

			public float speed;

			public float Interval;

			public bool IsColor { get; set; }

			public bool IsSize { get; set; }

			public bool IsBold { get; set; }

			public bool IsItalic { get; set; }

			public bool IsDoubleWord { get; set; }

			public bool IsRubyTop { get; set; }

			public bool IsRuby { get; set; }

			public bool IsEmphasisMark { get; set; }

			public bool IsSuperScript { get; set; }

			public bool IsSubScript { get; set; }

			public bool IsSuperOrSubScript
			{
				get
				{
					if (!IsSuperScript)
					{
						return IsSubScript;
					}
					return true;
				}
			}

			public bool IsUnderLineTop { get; set; }

			public bool IsUnderLine { get; set; }

			public bool IsStrikeTop { get; set; }

			public bool IsStrike { get; set; }

			public bool IsGroupTop { get; set; }

			public bool IsGroup { get; set; }

			public bool IsEmoji { get; set; }

			public string EmojiKey { get; set; }

			public bool IsDash { get; set; }

			public int DashSize { get; set; }

			public bool IsSpace { get; set; }

			public int SpaceSize { get; set; }

			public bool IsSpeed { get; set; }

			public bool IsInterval { get; set; }

			public bool IsHitEventTop { get; set; }

			public bool IsHitEvent { get; set; }

			public string HitEventArg { get; set; }

			public HitEventType HitEventType { get; set; }

			public CustomCharaInfo Clone()
			{
				return (CustomCharaInfo)MemberwiseClone();
			}

			public bool TryParseBold(string arg)
			{
				return IsBold = true;
			}

			public void ResetBold()
			{
				IsBold = false;
			}

			public bool TryParseItalic(string arg)
			{
				return IsItalic = true;
			}

			public void ResetItalic()
			{
				IsItalic = false;
			}

			public bool TryParseSize(string arg)
			{
				return IsSize = int.TryParse(arg, out size);
			}

			public void ResetSize()
			{
				IsSize = false;
				size = 0;
			}

			public bool TryParseColor(string arg)
			{
				IsColor = ColorUtil.TryParseColor(arg, ref color);
				if (IsColor)
				{
					colorStr = arg;
				}
				return IsColor;
			}

			public void ResetColor()
			{
				IsColor = false;
				color = Color.white;
			}

			public bool TryParseRuby(string arg)
			{
				if (string.IsNullOrEmpty(arg))
				{
					return false;
				}
				bool isRubyTop = (IsRuby = true);
				IsRubyTop = isRubyTop;
				rubyStr = arg;
				return true;
			}

			public void ResetRuby()
			{
				IsRuby = false;
				rubyStr = "";
			}

			public bool TryParseEmphasisMark(string arg)
			{
				if (string.IsNullOrEmpty(arg))
				{
					return false;
				}
				rubyStr = arg;
				if (rubyStr.Length != 1)
				{
					return false;
				}
				bool flag2 = (IsEmphasisMark = true);
				bool isRubyTop = (IsRuby = flag2);
				IsRubyTop = isRubyTop;
				return true;
			}

			public void ResetEmphasisMark()
			{
				bool isRuby = (IsEmphasisMark = false);
				IsRuby = isRuby;
				rubyStr = "";
			}

			public bool TryParseSuperScript(string arg)
			{
				IsSuperScript = true;
				return true;
			}

			public void ResetSuperScript()
			{
				IsSuperScript = false;
			}

			public bool TryParseSubScript(string arg)
			{
				IsSubScript = true;
				return true;
			}

			public void ResetSubScript()
			{
				IsSubScript = false;
			}

			public bool TryParseUnderLine(string arg)
			{
				bool isUnderLineTop = (IsUnderLine = true);
				IsUnderLineTop = isUnderLineTop;
				return true;
			}

			public void ResetUnderLine()
			{
				IsUnderLine = false;
			}

			public bool TryParseStrike(string arg)
			{
				bool isStrikeTop = (IsStrike = true);
				IsStrikeTop = isStrikeTop;
				return true;
			}

			public void ResetStrike()
			{
				IsStrike = false;
			}

			public bool TryParseGroup(string arg)
			{
				bool isGroupTop = (IsGroup = true);
				IsGroupTop = isGroupTop;
				return true;
			}

			public void ResetGroup()
			{
				IsGroup = false;
			}

			public bool TryParseLink(string arg)
			{
				bool isHitEventTop = (IsHitEvent = true);
				IsHitEventTop = isHitEventTop;
				HitEventArg = arg;
				HitEventType = HitEventType.Link;
				return true;
			}

			public void ResetLink()
			{
				IsHitEvent = false;
			}

			public bool TryParseTips(string arg)
			{
				bool isHitEventTop = (IsHitEvent = true);
				IsHitEventTop = isHitEventTop;
				HitEventArg = arg;
				HitEventType = HitEventType.Tips;
				return true;
			}

			public void ResetTips()
			{
				IsHitEvent = false;
			}

			internal bool TryParseSound(string arg)
			{
				bool isHitEventTop = (IsHitEvent = true);
				IsHitEventTop = isHitEventTop;
				HitEventArg = arg;
				HitEventType = HitEventType.Sound;
				return true;
			}

			internal void ResetSound()
			{
				IsHitEvent = false;
			}

			internal bool TryParseSpeed(string arg)
			{
				return IsSpeed = WrapperUnityVersion.TryParseFloatGlobal(arg, out speed);
			}

			internal void ResetSpeed()
			{
				IsSpeed = false;
				speed = 0f;
			}

			internal bool TryParseInterval(string arg)
			{
				return IsInterval = WrapperUnityVersion.TryParseFloatGlobal(arg, out Interval);
			}

			public bool IsEndBold(CustomCharaInfo lastCustomInfo)
			{
				if (!lastCustomInfo.IsBold)
				{
					return false;
				}
				return !IsBold;
			}

			public bool IsBeginBold(CustomCharaInfo lastCustomInfo)
			{
				if (!IsBold)
				{
					return false;
				}
				return !lastCustomInfo.IsBold;
			}

			public bool IsEndItalic(CustomCharaInfo lastCustomInfo)
			{
				if (!lastCustomInfo.IsItalic)
				{
					return false;
				}
				return !IsItalic;
			}

			public bool IsBeginItalic(CustomCharaInfo lastCustomInfo)
			{
				if (!IsItalic)
				{
					return false;
				}
				return !lastCustomInfo.IsItalic;
			}

			public bool IsEndSize(CustomCharaInfo lastCustomInfo)
			{
				if (!lastCustomInfo.IsSize)
				{
					return false;
				}
				if (!IsSize)
				{
					return true;
				}
				return lastCustomInfo.size != size;
			}

			public bool IsBeginSize(CustomCharaInfo lastCustomInfo)
			{
				if (!IsSize)
				{
					return false;
				}
				if (!lastCustomInfo.IsSize)
				{
					return true;
				}
				return lastCustomInfo.size != size;
			}

			public bool IsEndColor(CustomCharaInfo lastCustomInfo)
			{
				if (!lastCustomInfo.IsColor)
				{
					return false;
				}
				if (!IsColor)
				{
					return true;
				}
				return lastCustomInfo.color != color;
			}

			public bool IsBeginColor(CustomCharaInfo lastCustomInfo)
			{
				if (!IsColor)
				{
					return false;
				}
				if (!lastCustomInfo.IsColor)
				{
					return true;
				}
				return lastCustomInfo.color != color;
			}

			public int GetCustomedSize(int defaultSize)
			{
				if (!IsSize)
				{
					return defaultSize;
				}
				return size;
			}

			public FontStyle GetCustomedStyle(FontStyle defaultFontStyle)
			{
				if (IsItalic && IsBold)
				{
					return FontStyle.BoldAndItalic;
				}
				if (IsItalic)
				{
					return FontStyle.Italic;
				}
				if (IsBold)
				{
					return FontStyle.Bold;
				}
				return defaultFontStyle;
			}

			public Color GetCustomedColor(Color defaultColor)
			{
				if (!IsColor)
				{
					return defaultColor;
				}
				return color;
			}

			public void ClearOnNextChar()
			{
				IsRubyTop = false;
				IsUnderLineTop = false;
				IsStrikeTop = false;
				IsHitEventTop = false;
				IsGroupTop = false;
				rubyStr = "";
			}
		}

		public const char Dash = '—';

		private char c;

		private int unityRitchTextIndex = -1;

		private CustomCharaInfo customInfo;

		public char Char
		{
			get
			{
				return c;
			}
			set
			{
				c = value;
			}
		}

		public int UnityRitchTextIndex
		{
			get
			{
				return unityRitchTextIndex;
			}
			set
			{
				unityRitchTextIndex = value;
			}
		}

		public CustomCharaInfo CustomInfo
		{
			get
			{
				return customInfo;
			}
		}

		public bool IsBr
		{
			get
			{
				if (Char != '\n')
				{
					return Char == '\r';
				}
				return true;
			}
		}

		public CharData(char c, CustomCharaInfo customInfo)
		{
			this.c = c;
			this.customInfo = customInfo.Clone();
		}

		public CharData(char c)
		{
			this.c = c;
			customInfo = new CustomCharaInfo();
		}

		internal bool TryParseInterval(string arg)
		{
			return customInfo.TryParseInterval(arg);
		}
	}
}
