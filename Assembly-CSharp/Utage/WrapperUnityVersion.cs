using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Utage
{
	public class WrapperUnityVersion
	{
		private const NumberStyles DefaultNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands;

		public static Vector2 GetBoxCollider2DOffset(BoxCollider2D col)
		{
			return col.offset;
		}

		public static void SetBoxCollider2DOffset(BoxCollider2D col, Vector2 offset)
		{
			col.offset = offset;
		}

		public static void SetCharacterInfoToVertex(UIVertex[] verts, UguiNovelTextCharacter character, ref CharacterInfo charInfo, Font font)
		{
			float num = 0.1f * (float)character.FontSize;
			float num2 = charInfo.minX;
			float num3 = charInfo.maxX;
			float num4 = charInfo.minY;
			float num5 = charInfo.maxY;
			if (!font.dynamic)
			{
				num2 *= character.BmpFontScale;
				num4 *= character.BmpFontScale;
				num3 *= character.BmpFontScale;
				num5 *= character.BmpFontScale;
			}
			Vector2 uvBottomLeft = charInfo.uvBottomLeft;
			Vector2 uvBottomRight = charInfo.uvBottomRight;
			Vector2 uvTopRight = charInfo.uvTopRight;
			Vector2 uvTopLeft = charInfo.uvTopLeft;
			verts[0].position.x = (verts[3].position.x = num2 + character.PositionX);
			verts[1].position.x = (verts[2].position.x = num3 + character.PositionX);
			verts[0].position.y = (verts[1].position.y = num4 + character.PositionY + num);
			verts[2].position.y = (verts[3].position.y = num5 + character.PositionY + num);
			verts[0].uv0 = uvBottomLeft;
			verts[1].uv0 = uvBottomRight;
			verts[2].uv0 = uvTopRight;
			verts[3].uv0 = uvTopLeft;
		}

		public static float GetCharacterInfoWidth(ref CharacterInfo charInfo)
		{
			return charInfo.advance;
		}

		public static float GetCharacterEndPointX(UguiNovelTextCharacter character)
		{
			return character.Verts[1].position.x;
		}

		public static void SetFontRenderInfo(char c, ref CharacterInfo info, float offsetY, float fontSize, out Vector3 offset, out float width, out float kerningWidth)
		{
			float x = info.minX + (info.maxX - info.minX) / 2;
			float y = (float)info.maxY - ((float)info.glyphHeight + fontSize) / 2f + offsetY + fontSize / 5f;
			offset = new Vector3(x, y, 0f);
			width = GetCharacterInfoWidth(ref info);
			kerningWidth = info.maxX - info.minX;
		}

		public static Rect GetUvRect(ref CharacterInfo info, Texture2D texture)
		{
			if (Mathf.Approximately(info.uvTopLeft.x, info.uvTopRight.x))
			{
				float x = info.uvBottomLeft.x;
				float num = info.uvTopLeft.x - x;
				float y = info.uvTopRight.y;
				float num2 = info.uvTopLeft.y - y;
				return new Rect(x * (float)texture.width, y * (float)texture.height, num * (float)texture.width, num2 * (float)texture.height);
			}
			float x2 = info.uvTopLeft.x;
			float num3 = info.uvTopRight.x - x2;
			float y2 = info.uvTopLeft.y;
			float num4 = info.uvBottomLeft.y - y2;
			return new Rect(x2 * (float)texture.width, y2 * (float)texture.height, num3 * (float)texture.width, num4 * (float)texture.height);
		}

		public static bool IsReadyPlayAudioClip(AudioClip clip)
		{
			return clip.loadState == AudioDataLoadState.Loaded;
		}

		public static AudioClip CreateAudioClip(string name, int lengthSamples, int channels, int frequency, bool is3D, bool stream)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream);
		}

		public static void SetNoBackupFlag(string path)
		{
		}

		public static void SetActivityIndicatorStyle()
		{
		}

		internal static void AddEntryToEventTrigger(EventTrigger eventTrigger, EventTrigger.Entry entry)
		{
			if (eventTrigger.triggers == null)
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			eventTrigger.triggers.Add(entry);
		}

		internal static Vector3 GetWorldPositionFromPointerEventData(PointerEventData data)
		{
			return data.pointerCurrentRaycast.worldPosition;
		}

		internal static void LoadScene(int index)
		{
			SceneManager.LoadScene(index);
		}

		internal static void LoadScene(string name)
		{
			SceneManager.LoadScene(name);
		}

		public static bool IsFinishedSplashScreen()
		{
			return SplashScreen.isFinished;
		}

		public static float UsedHeapMegaSize()
		{
			return 1f * (float)Profiler.usedHeapSizeLong / 1024f / 1024f;
		}

		public static float MonoHeapMegaSize()
		{
			return 1f * (float)Profiler.GetMonoHeapSizeLong() / 1024f / 1024f;
		}

		public static float MonoUsedMegaSize()
		{
			return 1f * (float)Profiler.GetMonoUsedSizeLong() / 1024f / 1024f;
		}

		public static void CleanCache()
		{
			Caching.ClearCache();
		}

		public static float ParseFloatGlobal(string str)
		{
			return float.Parse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
		}

		public static bool TryParseFloatGlobal(string str, out float val)
		{
			return float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out val);
		}

		public static double ParseDoubleGlobal(string str)
		{
			return double.Parse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
		}

		public static bool TryParseDoubleGlobal(string str, out double val)
		{
			return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out val);
		}
	}
}
