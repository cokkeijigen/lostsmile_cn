using System;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvGuiBase
	{
		private RectTransform rectTransform;

		private Canvas canvas;

		private RectTransform canvasRectTransform;

		protected byte[] defaultData;

		private const int Version = 0;

		public string Name
		{
			get
			{
				return Target.name;
			}
		}

		public GameObject Target { get; private set; }

		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = Target.transform as RectTransform;
				}
				return rectTransform;
			}
		}

		public Canvas Canvas
		{
			get
			{
				return canvas ?? (canvas = Target.GetComponentInParent<Canvas>());
			}
		}

		public RectTransform CanvasRectTransform
		{
			get
			{
				if (canvasRectTransform == null)
				{
					canvasRectTransform = Canvas.transform as RectTransform;
				}
				return canvasRectTransform;
			}
		}

		public bool HasChanged { get; private set; }

		public AdvGuiBase(GameObject target)
		{
			Target = target;
			HasChanged = true;
			defaultData = ToBuffer();
			HasChanged = false;
		}

		public virtual byte[] ToBuffer()
		{
			return BinaryUtil.BinaryWrite(Write);
		}

		public virtual void ReadBuffer(byte[] buffer)
		{
			BinaryUtil.BinaryRead(buffer, Read);
		}

		protected virtual void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(HasChanged);
			if (HasChanged)
			{
				WriteChanged(writer);
			}
		}

		protected virtual void WriteChanged(BinaryWriter writer)
		{
			writer.Write(Target.activeSelf);
			writer.WriteRectTransfom(RectTransform);
		}

		protected virtual void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num >= 0)
			{
				bool flag = reader.ReadBoolean();
				if (flag)
				{
					HasChanged = flag;
					ReadChanged(reader);
				}
				else
				{
					Reset();
				}
				return;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
		}

		protected virtual void ReadChanged(BinaryReader reader)
		{
			Target.SetActive(reader.ReadBoolean());
			reader.ReadRectTransfom(RectTransform);
		}

		internal virtual void Reset()
		{
			if (HasChanged)
			{
				ReadBuffer(defaultData);
				HasChanged = false;
			}
		}

		public virtual void SetActive(bool isActive)
		{
			HasChanged = true;
			Target.SetActive(isActive);
		}

		public virtual void SetPosition(float? x, float? y)
		{
			HasChanged = true;
			Vector3 position = CanvasRectTransform.InverseTransformPoint(RectTransform.position);
			if (x.HasValue)
			{
				position.x = x.Value;
			}
			if (y.HasValue)
			{
				position.y = y.Value;
			}
			position = CanvasRectTransform.TransformPoint(position);
			RectTransform.position = position;
		}

		internal virtual void SetSize(float? x, float? y)
		{
			HasChanged = true;
			if (x.HasValue)
			{
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x.Value);
			}
			if (y.HasValue)
			{
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y.Value);
			}
		}
	}
}
