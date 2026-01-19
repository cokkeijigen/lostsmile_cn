using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[CreateAssetMenu(menuName = "Utage/AvatarData")]
	public class AvatarData : ScriptableObject
	{
		[Serializable]
		public class Category
		{
			[SerializeField]
			private string name;

			[SerializeField]
			private int sortOrder;

			[SerializeField]
			private string tag;

			[SerializeField]
			private List<Sprite> sprites = new List<Sprite>();

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public int SortOrder
			{
				get
				{
					return sortOrder;
				}
				set
				{
					sortOrder = value;
				}
			}

			public string Tag
			{
				get
				{
					return tag;
				}
				set
				{
					tag = value;
				}
			}

			public List<Sprite> Sprites
			{
				get
				{
					return sprites;
				}
				set
				{
					sprites = value;
				}
			}

			public HashSet<string> GetAllPatternNames()
			{
				HashSet<string> set = new HashSet<string>();
				Sprites.ForEach(delegate(Sprite x)
				{
					set.Add(ToPatternName(x));
				});
				return set;
			}

			public Sprite GetSprite(string pattern)
			{
				Sprite sprite = Sprites.Find((Sprite x) => ToPatternName(x) == pattern);
				if (sprite == null)
				{
					sprite = Sprites.Find((Sprite x) => x != null && x.name == pattern);
				}
				if (sprite == null)
				{
					sprite = Sprites.Find((Sprite x) => x != null && x.name == ToPatternName(pattern));
				}
				return sprite;
			}

            // iTsukezigen++
            public Sprite GetSprite(string pattern, string fromName)
            {
                Sprite sprite = null;
                if (!CHSPatch.AssetPatchManager.GetSpriteIfExists($"{fromName}_{pattern}", out sprite))
                {
                    sprite = GetSprite(pattern);
                }
                return sprite;
            }
            // End++
        }

        [NotEditable]
		public List<Category> categories = new List<Category>();

		[SerializeField]
		private string optionTag = "accessory";

		[SerializeField]
		[NotEditable]
		private Vector2 size;

		public string OptionTag => optionTag;

		public Vector2 Size
		{
			get
			{
				return size;
			}
			internal set
			{
				size = value;
			}
		}

		public static string ToPatternName(Sprite sprite)
		{
			if (sprite == null)
			{
				return "";
			}
			return ToPatternName(sprite.name);
		}

		public static string ToPatternName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return "";
			}
			return name.Split('_')[0];
		}

		public List<Sprite> MakeSortedSprites(AvatarPattern pattern)
		{
			List<Sprite> list = new List<Sprite>();
			foreach (Category category in categories)
			{
				if (category.Tag != optionTag)
				{
					foreach (AvatarPattern.PartternData data in pattern.DataList)
					{
						if (!(category.Tag != data.tag))
						{
							list.Add(category.GetSprite(data.patternName, this.name)); // iTsukezigen++
						}
					}
					continue;
				}
				foreach (string optionPattern in pattern.OptionPatternNameList)
				{
					list.AddRange(category.Sprites.FindAll((Sprite x) => ToPatternName(x) == optionPattern));
				}
			}
			return list;
		}

		public void CheckPatternError(AvatarPattern pattern)
		{
			foreach (AvatarPattern.PartternData data in pattern.DataList)
			{
				if (CheckPatternError(pattern, data))
				{
					Debug.LogErrorFormat("Tag:{0} Pattern:{1} is not found in AvatorData {2}", data.tag, data.patternName, base.name);
				}
			}
		}

		private bool CheckPatternError(AvatarPattern pattern, AvatarPattern.PartternData patternData)
		{
			if (string.IsNullOrEmpty(patternData.patternName))
			{
				return false;
			}
			foreach (Category category in categories)
			{
				if (category.Tag != optionTag)
				{
					// iTsukezigen++
					if (!(category.Tag != patternData.tag) && category.GetSprite(patternData.patternName, this.name) != null)
					{
						return false;
					}
					continue;
				}
				foreach (string optionPattern in pattern.OptionPatternNameList)
				{
					if (category.Sprites.Exists((Sprite x) => ToPatternName(x) == optionPattern))
					{
						return false;
					}
				}
			}
			return true;
		}

		public List<string> GetAllOptionPatterns()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Category category in categories)
			{
				if (!(category.Tag != OptionTag))
				{
					hashSet.UnionWith(category.GetAllPatternNames());
				}
			}
			return new List<string>(hashSet);
		}
	}
}
