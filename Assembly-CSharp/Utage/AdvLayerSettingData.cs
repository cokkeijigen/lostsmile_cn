using UnityEngine;

namespace Utage
{
	public class AdvLayerSettingData : AdvSettingDictinoayItemBase
	{
		public enum LayerType
		{
			Bg,
			Character,
			Sprite,
			Max
		}

		internal enum BorderType
		{
			None,
			Streach,
			BorderMin,
			BorderMax
		}

		internal class RectSetting
		{
			public BorderType type;

			public float position;

			public float size;

			public float borderMin;

			public float borderMax;

			internal void GetBorderdPositionAndSize(float defaultSize, out float position, out float size)
			{
				switch (type)
				{
				case BorderType.BorderMin:
					size = ((this.size == 0f) ? defaultSize : this.size);
					position = (0f - defaultSize) / 2f + borderMin + size / 2f;
					break;
				case BorderType.BorderMax:
					size = ((this.size == 0f) ? defaultSize : this.size);
					position = defaultSize / 2f - borderMax - size / 2f;
					break;
				case BorderType.Streach:
					size = defaultSize;
					size -= borderMin + borderMax;
					position = borderMin - borderMax;
					break;
				default:
					size = ((this.size == 0f) ? defaultSize : this.size);
					position = this.position;
					break;
				}
			}
		}

		private bool isDefault;

		public string Name
		{
			get
			{
				return base.Key;
			}
		}

		public LayerType Type { get; private set; }

		internal RectSetting Horizontal { get; private set; }

		internal RectSetting Vertical { get; private set; }

		public float Z { get; private set; }

		public Vector3 Scale { get; private set; }

		public Vector2 Pivot { get; private set; }

		public int Order { get; private set; }

		public string LayerMask { get; private set; }

		public Alignment Alignment { get; private set; }

		public bool FlipX { get; private set; }

		public bool FlipY { get; private set; }

		public bool IsDefault
		{
			get
			{
				return isDefault;
			}
			set
			{
				isDefault = value;
			}
		}

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			base.RowData = row;
			string value = AdvParser.ParseCell<string>(row, AdvColumnName.LayerName);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			InitKey(value);
			Type = AdvParser.ParseCell<LayerType>(row, AdvColumnName.Type);
			Order = AdvParser.ParseCell<int>(row, AdvColumnName.Order);
			LayerMask = AdvParser.ParseCellOptional(row, AdvColumnName.LayerMask, "");
			Horizontal = new RectSetting();
			bool num = !AdvParser.IsEmptyCell(row, AdvColumnName.BorderLeft);
			bool flag = !AdvParser.IsEmptyCell(row, AdvColumnName.BorderRight);
			if (num)
			{
				Horizontal.type = (flag ? BorderType.Streach : BorderType.BorderMin);
			}
			else
			{
				Horizontal.type = (flag ? BorderType.BorderMax : BorderType.None);
			}
			Horizontal.position = AdvParser.ParseCellOptional(row, AdvColumnName.X, 0f);
			Horizontal.size = AdvParser.ParseCellOptional(row, AdvColumnName.Width, 0f);
			Horizontal.borderMin = AdvParser.ParseCellOptional(row, AdvColumnName.BorderLeft, 0f);
			Horizontal.borderMax = AdvParser.ParseCellOptional(row, AdvColumnName.BorderRight, 0f);
			Vertical = new RectSetting();
			bool num2 = !AdvParser.IsEmptyCell(row, AdvColumnName.BorderTop);
			bool flag2 = !AdvParser.IsEmptyCell(row, AdvColumnName.BorderBottom);
			if (num2)
			{
				Vertical.type = (flag2 ? BorderType.Streach : BorderType.BorderMax);
			}
			else
			{
				Vertical.type = (flag2 ? BorderType.BorderMin : BorderType.None);
			}
			Vertical.position = AdvParser.ParseCellOptional(row, AdvColumnName.Y, 0f);
			Vertical.size = AdvParser.ParseCellOptional(row, AdvColumnName.Height, 0f);
			Vertical.borderMin = AdvParser.ParseCellOptional(row, AdvColumnName.BorderBottom, 0f);
			Vertical.borderMax = AdvParser.ParseCellOptional(row, AdvColumnName.BorderTop, 0f);
			Vector2 pivot = default(Vector2);
			pivot.x = AdvParser.ParseCellOptional(row, AdvColumnName.PivotX, 0.5f);
			pivot.y = AdvParser.ParseCellOptional(row, AdvColumnName.PivotY, 0.5f);
			Pivot = pivot;
			Vector3 scale = default(Vector3);
			scale.x = AdvParser.ParseCellOptional(row, AdvColumnName.ScaleX, 1f);
			scale.y = AdvParser.ParseCellOptional(row, AdvColumnName.ScaleY, 1f);
			scale.z = AdvParser.ParseCellOptional(row, AdvColumnName.ScaleZ, 1f);
			Scale = scale;
			Z = AdvParser.ParseCellOptional(row, AdvColumnName.Z, -0.01f * (float)Order);
			Alignment = AdvParser.ParseCellOptional(row, AdvColumnName.Align, Alignment.None);
			FlipX = AdvParser.ParseCellOptional(row, AdvColumnName.FlipX, false);
			FlipY = AdvParser.ParseCellOptional(row, AdvColumnName.FlipY, false);
			return true;
		}

		public void InitDefault(string name, LayerType type, int order)
		{
			InitKey(name);
			Type = type;
			Horizontal = new RectSetting();
			Vertical = new RectSetting();
			Pivot = Vector2.one * 0.5f;
			Order = order;
			Scale = Vector3.one;
			Z = -0.01f * (float)order;
			LayerMask = "";
			Alignment = Alignment.None;
			FlipX = false;
			FlipY = false;
		}
	}
}
