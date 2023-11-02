using System;
using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvAnimationData : IAdvSettingData
	{
		private enum PropertyType
		{
			Custom,
			X,
			Y,
			Z,
			Scale,
			ScaleX,
			ScaleY,
			ScaleZ,
			Angle,
			AngleX,
			AngleY,
			AngleZ,
			Alpha,
			Texture
		}

		public AnimationClip Clip { get; set; }

		public AdvAnimationData(StringGrid grid, ref int index, bool legacy)
		{
			Clip = new AnimationClip();
			Clip.legacy = legacy;
			ParseHeader(grid.Rows[index++]);
			List<float> timeTbl = ParseTimeTbl(grid.Rows[index++]);
			if (!Clip.legacy)
			{
				AddDummyCurve(timeTbl);
			}
			while (index < grid.Rows.Count)
			{
				StringGridRow stringGridRow = grid.Rows[index];
				try
				{
					if (stringGridRow.IsEmptyOrCommantOut)
					{
						index++;
						continue;
					}
					if (IsHeader(stringGridRow))
					{
						break;
					}
					if (!stringGridRow.TryParseCellTypeOptional(0, PropertyType.Custom, out var val))
					{
						stringGridRow.ParseCell<string>(0).Separate('.', false, out var str, out var str2);
						Type type = Type.GetType(str);
						if (type == null)
						{
							Debug.LogError(str + "is not class name");
						}
						Clip.SetCurve("", type, str2, ParseCurve(timeTbl, stringGridRow));
					}
					else if (IsEvent(val))
					{
						AddEvent(val, timeTbl, stringGridRow);
					}
					else
					{
						AddCurve(val, ParseCurve(timeTbl, stringGridRow));
					}
					index++;
				}
				catch (Exception ex)
				{
					Debug.LogError(stringGridRow.ToErrorString(ex.Message));
				}
			}
		}

		private bool IsHeader(StringGridRow row)
		{
			return row.ParseCell<string>(0)[0] == '*';
		}

		private void ParseHeader(StringGridRow row)
		{
			Clip.name = row.ParseCell<string>(0).Substring(1);
			Clip.wrapMode = row.ParseCellOptional(1, WrapMode.Default);
		}

		private List<float> ParseTimeTbl(StringGridRow row)
		{
			List<float> list = new List<float>();
			for (int i = 1; i < row.Strings.Length; i++)
			{
				if (!row.TryParseCell<float>(i, out var val))
				{
					Debug.LogError(row.ToErrorString("TimeTbl pase error"));
				}
				list.Add(val);
			}
			return list;
		}

		private bool IsEvent(PropertyType type)
		{
			if (type == PropertyType.Texture)
			{
				return true;
			}
			return false;
		}

		private bool IsCustomProperty(PropertyType type)
		{
			if (type == PropertyType.Custom)
			{
				return true;
			}
			return false;
		}

		private void AddEvent(PropertyType propertyType, List<float> timeTbl, StringGridRow row)
		{
			for (int i = 0; i < row.Strings.Length; i++)
			{
				if (i == 0 || row.IsEmptyCell(i))
				{
					continue;
				}
				AnimationEvent animationEvent = new AnimationEvent();
				if (propertyType == PropertyType.Texture)
				{
					if (!row.TryParseCell<string>(i, out var val))
					{
						continue;
					}
					animationEvent.functionName = "ChangePattern";
					animationEvent.stringParameter = val;
					animationEvent.time = timeTbl[i - 1];
				}
				if (Application.isPlaying)
				{
					Clip.AddEvent(animationEvent);
				}
			}
		}

		private AnimationCurve ParseCurve(List<float> timeTbl, StringGridRow row)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			for (int i = 0; i < row.Strings.Length; i++)
			{
				if (i != 0 && !row.IsEmptyCell(i) && row.TryParseCell<float>(i, out var val))
				{
					animationCurve.AddKey(new Keyframe(timeTbl[i - 1], val));
				}
			}
			_ = animationCurve.keys.Length;
			_ = 1;
			return animationCurve;
		}

		private void AddDummyCurve(List<float> timeTbl)
		{
			AnimationCurve curve = AnimationCurve.Linear(timeTbl[0], 0f, timeTbl[timeTbl.Count - 1], 1f);
			Clip.SetCurve("", typeof(UnityEngine.Object), "", curve);
		}

		private void AddCurve(PropertyType type, AnimationCurve curve)
		{
			if (curve.keys.Length != 0)
			{
				switch (type)
				{
				case PropertyType.X:
					Clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
					break;
				case PropertyType.Y:
					Clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
					break;
				case PropertyType.Z:
					Clip.SetCurve("", typeof(Transform), "localPosition.z", curve);
					break;
				case PropertyType.ScaleX:
					Clip.SetCurve("", typeof(Transform), "localScale.x", curve);
					break;
				case PropertyType.ScaleY:
					Clip.SetCurve("", typeof(Transform), "localScale.y", curve);
					break;
				case PropertyType.ScaleZ:
					Clip.SetCurve("", typeof(Transform), "localScale.z", curve);
					break;
				case PropertyType.Scale:
					Clip.SetCurve("", typeof(Transform), "localScale.x", curve);
					Clip.SetCurve("", typeof(Transform), "localScale.y", curve);
					Clip.SetCurve("", typeof(Transform), "localScale.z", curve);
					break;
				case PropertyType.AngleX:
					Clip.SetCurve("", typeof(Transform), "localEulerAngles.x", curve);
					break;
				case PropertyType.AngleY:
					Clip.SetCurve("", typeof(Transform), "localEulerAngles.y", curve);
					break;
				case PropertyType.Angle:
				case PropertyType.AngleZ:
					Clip.SetCurve("", typeof(Transform), "localEulerAngles.z", curve);
					break;
				case PropertyType.Alpha:
					Clip.SetCurve("", typeof(AdvEffectColor), "animationColor.a", curve);
					break;
				default:
					Debug.LogError("UnknownType");
					break;
				}
			}
		}
	}
}
