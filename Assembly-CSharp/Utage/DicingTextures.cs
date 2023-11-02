using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[CreateAssetMenu(menuName = "Utage/DicingTextures")]
	public class DicingTextures : ScriptableObject
	{
		[HelpBox("ImportSetting", HelpBoxAttribute.Type.Info, 0)]
		[SerializeField]
		[IntPopup(new int[] { 32, 64, 128, 256 })]
		private int cellSize = 64;

		[SerializeField]
		[Min(1f)]
		private int padding = 3;

		[SerializeField]
		[NotEditable]
		private List<Texture2D> atlasTextures = new List<Texture2D>();

		[SerializeField]
		[NotEditable]
		private List<DicingTextureData> textureDataList = new List<DicingTextureData>();

		public int CellSize
		{
			get
			{
				return cellSize;
			}
			set
			{
				cellSize = value;
			}
		}

		public int Padding
		{
			get
			{
				return padding;
			}
			set
			{
				padding = value;
			}
		}

		public List<Texture2D> AtlasTextures => atlasTextures;

		public List<DicingTextureData> TextureDataList => textureDataList;

		internal DicingTextureData GetTextureData(string pattern)
		{
			foreach (DicingTextureData textureData in textureDataList)
			{
				if (textureData.Name == pattern)
				{
					return textureData;
				}
			}
			return null;
		}

		internal bool Exists(string pattern)
		{
			return textureDataList.Exists((DicingTextureData x) => x.Name == pattern);
		}

		internal List<DicingTextureData> GetTextureDataList(string topDirectory)
		{
			if (string.IsNullOrEmpty(topDirectory))
			{
				return TextureDataList;
			}
			if (!topDirectory.EndsWith("/"))
			{
				topDirectory += "/";
			}
			List<DicingTextureData> list = new List<DicingTextureData>();
			foreach (DicingTextureData textureData in TextureDataList)
			{
				if (textureData.Name.StartsWith(topDirectory))
				{
					list.Add(textureData);
				}
			}
			return list;
		}

		public Texture2D GetTexture(string name)
		{
			return atlasTextures.Find((Texture2D x) => x != null && x.name == name);
		}

		public List<string> GetPattenNameList()
		{
			List<string> list = new List<string>();
			foreach (DicingTextureData textureData in textureDataList)
			{
				list.Add(textureData.Name);
			}
			return list;
		}

		public List<DicingTextureData.QuadVerts> GetVerts(DicingTextureData data)
		{
			return data.GetVerts(this);
		}
	}
}
