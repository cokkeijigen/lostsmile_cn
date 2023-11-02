using System;
using UnityEngine;

namespace Utage
{
	public class Compression
	{
		private class Node
		{
			public int mNext;

			public int mPrev;

			public int mPos;
		}

		private class Index
		{
			private Node[] mNodes = new Node[2304];

			private int[] mStack = new int[2048];

			private int mStackPos;

			public Index()
			{
				for (int i = 0; i < 2304; i++)
				{
					mNodes[i] = new Node();
				}
				for (int j = 2048; j < 2304; j++)
				{
					mNodes[j].mNext = (mNodes[j].mPrev = j);
				}
				for (int k = 0; k < 2048; k++)
				{
					mStack[k] = k;
				}
				mStackPos = 2048;
			}

			public int getFirst(byte c)
			{
				return mNodes[2048 + c].mNext;
			}

			public Node getNode(int i)
			{
				return mNodes[i];
			}

			public void add(byte c, int pos)
			{
				mStackPos--;
				int num = mStack[mStackPos];
				Node obj = mNodes[num];
				Node node = mNodes[2048 + c];
				obj.mNext = node.mNext;
				obj.mPrev = 2048 + c;
				obj.mPos = pos;
				mNodes[node.mNext].mPrev = num;
				node.mNext = num;
			}

			public void remove(byte c, int pos)
			{
				int mPrev = mNodes[2048 + c].mPrev;
				Node node = mNodes[mPrev];
				if (node.mPos != pos)
				{
					Debug.LogError("n.mPos != pos");
				}
				mStack[mStackPos] = mNodes[node.mPrev].mNext;
				mStackPos++;
				mNodes[node.mPrev].mNext = node.mNext;
				mNodes[node.mNext].mPrev = node.mPrev;
			}

			public bool isEnd(int idx)
			{
				return idx >= 2048;
			}
		}

		private const int DIC_BITS = 11;

		private const int LENGTH_BITS = 4;

		private const int DIC_MASK = 2047;

		private const int DIC_MASK_HIGH = 1792;

		private const int DIC_MASK_SHIFTED = 112;

		private const int LENGTH_MASK = 15;

		private const int DIC_SIZE = 2048;

		private const int MAX_LENGTH = 18;

		public static byte[] Compress(byte[] bytes)
		{
			int num = bytes.Length;
			byte[] bytes2 = BitConverter.GetBytes(num);
			int num2 = bytes2.Length;
			byte[] array = new byte[num + num / 128 + 1];
			Compress(array, out var oSize, bytes);
			byte[] array2 = new byte[oSize + 4];
			Buffer.BlockCopy(bytes2, 0, array2, 0, num2);
			Buffer.BlockCopy(array, 0, array2, num2, oSize);
			return array2;
		}

		public static byte[] Decompress(byte[] bytes)
		{
			int oSize = BitConverter.ToInt32(bytes, 0);
			byte[] array = new byte[oSize];
			Decompress(array, out oSize, bytes);
			return array;
		}

		private static int min(int a, int b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		private static int max(int a, int b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		private static void Compress(byte[] oData, out int oSize, byte[] iData)
		{
			int num = iData.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			Index index = new Index();
			while (num3 < num)
			{
				int num5 = 0;
				int num6 = 0;
				int num7 = min(18, num - num3);
				int num8 = index.getFirst(iData[num3]);
				while (!index.isEnd(num8))
				{
					Node node = index.getNode(num8);
					int mPos = node.mPos;
					int i;
					for (i = 1; i < num7 && iData[mPos + i] == iData[num3 + i]; i++)
					{
					}
					if (num5 < i)
					{
						num6 = mPos;
						num5 = i;
						if (num5 == num7)
						{
							break;
						}
					}
					num8 = node.mNext;
				}
				if (num5 >= 3)
				{
					for (int j = 0; j < num5; j++)
					{
						int num9 = num3 + j - 2048;
						if (num9 >= 0)
						{
							index.remove(iData[num9], num9);
						}
						index.add(iData[num3 + j], num3 + j);
					}
					if (num4 < num3)
					{
						oData[num2] = (byte)(num3 - num4 - 1);
						num2++;
						for (int k = num4; k < num3; k++)
						{
							oData[num2] = iData[k];
							num2++;
						}
					}
					int num10 = num5 - 3;
					int num11 = num3 - num6 - 1;
					int num12 = 0x80 | num10;
					num12 |= (num11 & 0x700) >> 4;
					oData[num2] = (byte)num12;
					oData[num2 + 1] = (byte)((uint)num11 & 0xFFu);
					num2 += 2;
					num3 += num5;
					num4 = num3;
					continue;
				}
				int num13 = num3 - 2048;
				if (num13 >= 0)
				{
					index.remove(iData[num13], num13);
				}
				index.add(iData[num3], num3);
				num3++;
				if (num3 - num4 == 128)
				{
					oData[num2] = (byte)(num3 - num4 - 1);
					num2++;
					for (int l = num4; l < num3; l++)
					{
						oData[num2] = iData[l];
						num2++;
					}
					num4 = num3;
				}
			}
			if (num4 < num3)
			{
				oData[num2] = (byte)(num3 - num4 - 1);
				num2++;
				for (int m = num4; m < num3; m++)
				{
					oData[num2] = iData[m];
					num2++;
				}
			}
			oSize = num2;
		}

		private static void Decompress(byte[] oData, out int oSize, byte[] iData)
		{
			int num = 0;
			int num2 = iData.Length;
			int num3;
			for (num3 = 4; num3 < num2; num3++)
			{
				int num4;
				if ((iData[num3] & 0x80u) != 0)
				{
					num4 = iData[num3] & 0xF;
					num4 += 3;
					int num5 = ((iData[num3] & 0x70) << 4) | iData[num3 + 1];
					num5++;
					for (int i = 0; i < num4; i++)
					{
						oData[num + i] = oData[num - num5 + i];
					}
					num3++;
				}
				else
				{
					num4 = iData[num3] + 1;
					for (int j = 0; j < num4; j++)
					{
						oData[num + j] = iData[num3 + 1 + j];
					}
					num3 += num4;
				}
				num += num4;
			}
			oSize = num;
		}
	}
}
