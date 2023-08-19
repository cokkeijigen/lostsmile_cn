using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/GuiManager")]
	public class AdvGuiManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		[SerializeField]
		protected List<GameObject> guiObjects = new List<GameObject>();

		protected Dictionary<string, AdvGuiBase> objects = new Dictionary<string, AdvGuiBase>();

		private const int Version = 0;

		public Dictionary<string, AdvGuiBase> Objects
		{
			get
			{
				return objects;
			}
		}

		public virtual string SaveKey
		{
			get
			{
				return "GuiManager";
			}
		}

		protected virtual void Awake()
		{
			foreach (GameObject guiObject in guiObjects)
			{
				if (!objects.ContainsKey(guiObject.name))
				{
					objects.Add(guiObject.name, new AdvGuiBase(guiObject));
				}
			}
		}

		internal virtual bool TryGet(string name, out AdvGuiBase gui)
		{
			return objects.TryGetValue(name, out gui);
		}

		public virtual void OnClear()
		{
			foreach (AdvGuiBase value in objects.Values)
			{
				value.Reset();
			}
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(objects.Count);
			foreach (string key in objects.Keys)
			{
				writer.Write(key);
				byte[] bytes = objects[key].ToBuffer();
				writer.WriteBuffer(bytes);
			}
		}

		public virtual void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					string text = reader.ReadString();
					int count = reader.ReadInt32();
					byte[] buffer = reader.ReadBytes(count);
					AdvGuiBase value;
					if (objects.TryGetValue(text, out value))
					{
						value.ReadBuffer(buffer);
					}
					else
					{
						Debug.LogError(text + " is not found in GuiManager");
					}
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
