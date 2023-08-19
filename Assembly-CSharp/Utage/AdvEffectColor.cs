using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/EffectColor")]
	public class AdvEffectColor : MonoBehaviour
	{
		[SerializeField]
		private Color animationColor = Color.white;

		[SerializeField]
		private Color tweenColor = Color.white;

		[SerializeField]
		private Color scriptColor = Color.white;

		[SerializeField]
		private Color customColor = Color.white;

		[SerializeField]
		private float fadeAlpha = 1f;

		public EventEffectColor OnValueChanged = new EventEffectColor();

		private Color lastColor = Color.white;

		private const int Version = 0;

		public Color AnimationColor
		{
			get
			{
				return animationColor;
			}
			set
			{
				animationColor = value;
				ChangedValue();
			}
		}

		public Color TweenColor
		{
			get
			{
				return tweenColor;
			}
			set
			{
				tweenColor = value;
				ChangedValue();
			}
		}

		public Color ScriptColor
		{
			get
			{
				return scriptColor;
			}
			set
			{
				scriptColor = value;
				ChangedValue();
			}
		}

		public Color CustomColor
		{
			get
			{
				return customColor;
			}
			set
			{
				customColor = value;
				ChangedValue();
			}
		}

		public float FadeAlpha
		{
			get
			{
				return fadeAlpha;
			}
			set
			{
				fadeAlpha = value;
				ChangedValue();
			}
		}

		public Color MulColor
		{
			get
			{
				Color result = AnimationColor * TweenColor * ScriptColor * CustomColor;
				result.a *= FadeAlpha;
				return result;
			}
		}

		private void OnValidate()
		{
			ChangedValue();
		}

		private void ChangedValue()
		{
			Color mulColor = MulColor;
			OnValueChanged.Invoke(this);
			lastColor = mulColor;
		}

		private void Update()
		{
			if (lastColor != MulColor)
			{
				ChangedValue();
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(AnimationColor);
			writer.Write(TweenColor);
			writer.Write(ScriptColor);
			writer.Write(CustomColor);
			writer.Write(FadeAlpha);
		}

		public void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			animationColor = reader.ReadColor();
			tweenColor = reader.ReadColor();
			scriptColor = reader.ReadColor();
			customColor = reader.ReadColor();
			fadeAlpha = reader.ReadSingle();
			fadeAlpha = 1f;
			ChangedValue();
		}
	}
}
