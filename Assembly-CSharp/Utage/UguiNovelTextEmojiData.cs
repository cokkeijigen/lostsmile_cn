using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextEmojiData : ScriptableObject
	{
		[SerializeField]
		private int size;

		[SerializeField]
		private List<Sprite> spriteTbl = new List<Sprite>();

		private Dictionary<char, Sprite> spriteDictionary;

		private Dictionary<string, Sprite> spriteDictionaryStringKey;

		public int Size
		{
			get
			{
				if (size == 0)
				{
					Debug.LogError("EmojiData size is zero", this);
					return 8;
				}
				return size;
			}
		}

		private Dictionary<char, Sprite> SpriteDictionary
		{
			get
			{
				if (spriteDictionary == null)
				{
					MakeDictionary();
				}
				return spriteDictionary;
			}
		}

		private Dictionary<string, Sprite> SpriteDictionaryStringKey
		{
			get
			{
				if (spriteDictionaryStringKey == null)
				{
					MakeDictionary();
				}
				return spriteDictionaryStringKey;
			}
		}

		private void MakeDictionary()
		{
			spriteDictionary = new Dictionary<char, Sprite>();
			spriteDictionaryStringKey = new Dictionary<string, Sprite>();
			foreach (Sprite item in spriteTbl)
			{
				if (!(item == null))
				{
					spriteDictionaryStringKey.Add(item.name, item);
					try
					{
						char key = Convert.ToChar(Convert.ToInt32(item.name, 16));
						spriteDictionary.Add(key, item);
					}
					catch
					{
					}
				}
			}
		}

		public Sprite GetSprite(string key)
		{
			Sprite value;
			if (SpriteDictionaryStringKey.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		public bool Contains(char c)
		{
			return SpriteDictionary.ContainsKey(c);
		}

		public Sprite GetSprite(char c)
		{
			Sprite value;
			if (SpriteDictionary.TryGetValue(c, out value))
			{
				return value;
			}
			return null;
		}
	}
}
