using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/TweenPlayer")]
	internal class AdvITweenPlayer : MonoBehaviour
	{
		private iTweenData data;

		private Hashtable hashTbl;

		private Action<AdvITweenPlayer> callbackComplete;

		private bool isColorSprite;

		private int count;

		private string tweenName;

		private bool isPlaying;

		private AdvEffectColor target;

		private List<AdvITweenPlayer> oldTweenPlayers = new List<AdvITweenPlayer>();

		public bool IsEndlessLoop => data.IsEndlessLoop;

		public bool IsPlaying => isPlaying;

		public bool IsAddType
		{
			get
			{
				iTweenType type = data.Type;
				if (type == iTweenType.MoveAdd || type == iTweenType.RotateAdd || type == iTweenType.ScaleAdd)
				{
					return true;
				}
				return false;
			}
		}

		public void Init(iTweenData data, bool isUnder2DSpace, float pixelsToUnits, float skipSpeed, Action<AdvITweenPlayer> callbackComplete)
		{
			this.data = data;
			if (data.Type == iTweenType.Stop)
			{
				return;
			}
			this.callbackComplete = callbackComplete;
			data.ReInit();
			hashTbl = iTween.Hash(data.MakeHashArray());
			if (iTweenData.IsPostionType(data.Type) && (!isUnder2DSpace || !hashTbl.ContainsKey("islocal") || !(bool)hashTbl["islocal"]))
			{
				if (hashTbl.ContainsKey("x"))
				{
					hashTbl["x"] = (float)hashTbl["x"] / pixelsToUnits;
				}
				if (hashTbl.ContainsKey("y"))
				{
					hashTbl["y"] = (float)hashTbl["y"] / pixelsToUnits;
				}
				if (hashTbl.ContainsKey("z"))
				{
					hashTbl["z"] = (float)hashTbl["z"] / pixelsToUnits;
				}
			}
			if (skipSpeed > 0f)
			{
				bool flag = hashTbl.ContainsKey("speed");
				if (flag)
				{
					hashTbl["speed"] = (float)hashTbl["speed"] * skipSpeed;
				}
				if (hashTbl.ContainsKey("time"))
				{
					hashTbl["time"] = (float)hashTbl["time"] / skipSpeed;
				}
				else if (!flag)
				{
					hashTbl["time"] = 1f / skipSpeed;
				}
			}
			if (data.Type == iTweenType.ColorTo || data.Type == iTweenType.ColorFrom)
			{
				target = base.gameObject.GetComponent<AdvEffectColor>();
				if (target != null)
				{
					Color tweenColor = target.TweenColor;
					if (data.Type == iTweenType.ColorTo)
					{
						hashTbl["from"] = tweenColor;
						hashTbl["to"] = ParaseTargetColor(hashTbl, tweenColor);
					}
					else if (data.Type == iTweenType.ColorFrom)
					{
						hashTbl["from"] = ParaseTargetColor(hashTbl, tweenColor);
						hashTbl["to"] = tweenColor;
					}
					hashTbl["onupdate"] = "OnColorUpdate";
					isColorSprite = true;
				}
			}
			hashTbl["oncomplete"] = "OnCompleteTween";
			hashTbl["oncompletetarget"] = base.gameObject;
			hashTbl["oncompleteparams"] = this;
			tweenName = GetHashCode().ToString();
			hashTbl["name"] = tweenName;
		}

		public void Play()
		{
			TryStoreOldTween();
			isPlaying = true;
			if (data.Type == iTweenType.Stop)
			{
				iTween.Stop(base.gameObject);
				return;
			}
			if (isColorSprite)
			{
				iTween.ValueTo(base.gameObject, hashTbl);
				return;
			}
			switch (data.Type)
			{
			case iTweenType.ColorFrom:
				iTween.ColorFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.ColorTo:
				iTween.ColorTo(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveAdd:
				iTween.MoveAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveBy:
				iTween.MoveBy(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveFrom:
				iTween.MoveFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.MoveTo:
				iTween.MoveTo(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchPosition:
				iTween.PunchPosition(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchRotation:
				iTween.PunchRotation(base.gameObject, hashTbl);
				return;
			case iTweenType.PunchScale:
				iTween.PunchScale(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateAdd:
				iTween.RotateAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateBy:
				iTween.RotateBy(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateFrom:
				iTween.RotateFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.RotateTo:
				iTween.RotateTo(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleAdd:
				iTween.ScaleAdd(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleBy:
				iTween.ScaleBy(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleFrom:
				iTween.ScaleFrom(base.gameObject, hashTbl);
				return;
			case iTweenType.ScaleTo:
				iTween.ScaleTo(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakePosition:
				iTween.ShakePosition(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakeRotation:
				iTween.ShakeRotation(base.gameObject, hashTbl);
				return;
			case iTweenType.ShakeScale:
				iTween.ShakeScale(base.gameObject, hashTbl);
				return;
			}
			isPlaying = false;
			Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownType, data.Type.ToString()));
		}

		private bool TryStoreOldTween()
		{
			bool result = false;
			AdvITweenPlayer[] components = GetComponents<AdvITweenPlayer>();
			foreach (AdvITweenPlayer advITweenPlayer in components)
			{
				if (!(advITweenPlayer == this) && advITweenPlayer.isPlaying)
				{
					result = true;
					oldTweenPlayers.Add(advITweenPlayer);
					oldTweenPlayers.AddRange(advITweenPlayer.oldTweenPlayers);
				}
			}
			return result;
		}

		private Color ParaseTargetColor(Hashtable hashTbl, Color color)
		{
			if (hashTbl.Contains("color"))
			{
				color = (Color)hashTbl["color"];
			}
			else
			{
				if (hashTbl.Contains("r"))
				{
					color.r = (float)hashTbl["r"];
				}
				if (hashTbl.Contains("g"))
				{
					color.g = (float)hashTbl["g"];
				}
				if (hashTbl.Contains("b"))
				{
					color.b = (float)hashTbl["b"];
				}
				if (hashTbl.Contains("a"))
				{
					color.a = (float)hashTbl["a"];
				}
			}
			if (hashTbl.Contains("alpha"))
			{
				color.a = (float)hashTbl["alpha"];
			}
			return color;
		}

		public void Cancel()
		{
			iTween.StopByName(base.gameObject, tweenName);
			isPlaying = false;
			UnityEngine.Object.Destroy(this);
		}

		private void OnDestroy()
		{
			foreach (AdvITweenPlayer oldTweenPlayer in oldTweenPlayers)
			{
				if (oldTweenPlayer != null)
				{
					UnityEngine.Object.Destroy(oldTweenPlayer);
				}
			}
			if (callbackComplete != null)
			{
				callbackComplete(this);
			}
			callbackComplete = null;
		}

		private void OnCompleteTween(AdvITweenPlayer arg)
		{
			if (!(arg != this))
			{
				count++;
				if (count >= data.LoopCount && !IsEndlessLoop)
				{
					Cancel();
				}
			}
		}

		private void OnColorUpdate(Color color)
		{
			if (target != null)
			{
				target.TweenColor = color;
			}
		}

		public void Write(BinaryWriter writer)
		{
			data.Write(writer);
		}

		public void Read(BinaryReader reader, bool isUnder2DSpace, float pixelsToUnits)
		{
			iTweenData iTweenData2 = new iTweenData(reader);
			Init(iTweenData2, isUnder2DSpace, pixelsToUnits, 1f, null);
		}

		internal static void WriteSaveData(BinaryWriter writer, GameObject go)
		{
			AdvITweenPlayer[] components = go.GetComponents<AdvITweenPlayer>();
			int num = 0;
			AdvITweenPlayer[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsEndlessLoop)
				{
					num++;
				}
			}
			writer.Write(num);
			array = components;
			foreach (AdvITweenPlayer advITweenPlayer in array)
			{
				if (advITweenPlayer.IsEndlessLoop)
				{
					advITweenPlayer.Write(writer);
				}
			}
		}

		internal static void ReadSaveData(BinaryReader reader, GameObject go, bool isUnder2DSpace, float pixelsToUnits)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				go.AddComponent<AdvITweenPlayer>().Read(reader, isUnder2DSpace, pixelsToUnits);
			}
		}
	}
}
