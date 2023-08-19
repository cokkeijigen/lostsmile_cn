using System;
using System.IO;
using UnityEngine;

namespace UtageExtensions
{
	public static class BinaryIOExtensions
	{
		public static void WriteBuffer(this BinaryWriter writer, byte[] bytes)
		{
			writer.Write(bytes.Length);
			writer.Write(bytes);
		}

		public static void WriteBuffer(this BinaryWriter writer, Action<BinaryWriter> onWrite)
		{
			long position = writer.BaseStream.Position;
			writer.BaseStream.Position += 4L;
			onWrite(writer);
			long position2 = writer.BaseStream.Position;
			int value = (int)(position2 - position - 4);
			writer.BaseStream.Position = position;
			writer.Write(value);
			writer.BaseStream.Position = position2;
		}

		public static byte[] ReadBuffer(this BinaryReader reader)
		{
			return reader.ReadBytes(reader.ReadInt32());
		}

		public static void SkipBuffer(this BinaryReader reader)
		{
			int num = reader.ReadInt32();
			reader.BaseStream.Position += num;
		}

		public static void ReadBuffer(this BinaryReader reader, Action<BinaryReader> onRead)
		{
			long position = reader.BaseStream.Position;
			int num = reader.ReadInt32();
			long num2 = position + 4 + num;
			bool flag = false;
			try
			{
				onRead(reader);
				flag = reader.BaseStream.Position != num2;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				flag = true;
			}
			if (flag)
			{
				Debug.LogError("Read Buffer Size Error");
				reader.BaseStream.Position = num2;
			}
		}

		public static void WriteJson(this BinaryWriter writer, object obj)
		{
			writer.Write(JsonUtility.ToJson(obj));
		}

		public static void ReadJson(this BinaryReader reader, object obj)
		{
			JsonUtility.FromJsonOverwrite(reader.ReadString(), obj);
		}

		public static void Write(this BinaryWriter writer, Vector2 vector)
		{
			writer.Write(vector.x);
			writer.Write(vector.y);
		}

		public static Vector2 ReadVector2(this BinaryReader reader)
		{
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Vector3 vector)
		{
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}

		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Vector4 vector)
		{
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
			writer.Write(vector.w);
		}

		public static Vector4 ReadVector4(this BinaryReader reader)
		{
			return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Quaternion quaternion)
		{
			writer.Write(quaternion.x);
			writer.Write(quaternion.y);
			writer.Write(quaternion.z);
			writer.Write(quaternion.w);
		}

		public static Quaternion ReadQuaternion(this BinaryReader reader)
		{
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void WriteLocalTransform(this BinaryWriter writer, Transform transform)
		{
			writer.Write(transform.localPosition);
			writer.Write(transform.localEulerAngles);
			writer.Write(transform.localScale);
		}

		public static void ReadLocalTransform(this BinaryReader reader, Transform transform)
		{
			transform.localPosition = reader.ReadVector3();
			transform.localEulerAngles = reader.ReadVector3();
			transform.localScale = reader.ReadVector3();
		}

		public static void Write(this BinaryWriter writer, Color color)
		{
			writer.Write(color.r);
			writer.Write(color.g);
			writer.Write(color.b);
			writer.Write(color.a);
		}

		public static Color ReadColor(this BinaryReader reader)
		{
			return new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Rect rect)
		{
			writer.Write(rect.xMin);
			writer.Write(rect.yMin);
			writer.Write(rect.width);
			writer.Write(rect.height);
		}

		public static Rect ReadRect(this BinaryReader reader)
		{
			return new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this BinaryWriter writer, Bounds bounds)
		{
			writer.Write(bounds.center);
			writer.Write(bounds.size);
		}

		public static Bounds ReadBounds(this BinaryReader reader)
		{
			return new Bounds(reader.ReadVector3(), reader.ReadVector3());
		}

		public static void Write(this BinaryWriter writer, AnimationCurve animationCurve)
		{
			throw new NotImplementedException();
		}

		public static void WriteRectTransfom(this BinaryWriter writer, RectTransform rectTransform)
		{
			writer.WriteLocalTransform(rectTransform);
			writer.Write(rectTransform.anchoredPosition3D);
			writer.Write(rectTransform.anchorMin);
			writer.Write(rectTransform.anchorMax);
			writer.Write(rectTransform.sizeDelta);
			writer.Write(rectTransform.pivot);
		}

		internal static void ReadRectTransfom(this BinaryReader reader, RectTransform rectTransform)
		{
			reader.ReadLocalTransform(rectTransform);
			rectTransform.anchoredPosition3D = reader.ReadVector3();
			rectTransform.anchorMin = reader.ReadVector2();
			rectTransform.anchorMax = reader.ReadVector2();
			rectTransform.sizeDelta = reader.ReadVector2();
			rectTransform.pivot = reader.ReadVector2();
		}
	}
}
