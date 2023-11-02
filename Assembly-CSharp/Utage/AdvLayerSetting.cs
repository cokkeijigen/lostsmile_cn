namespace Utage
{
	public class AdvLayerSetting : AdvSettingDataDictinoayBase<AdvLayerSettingData>
	{
		public override void ParseGrid(StringGrid grid)
		{
			base.ParseGrid(grid);
			InitDefault(AdvLayerSettingData.LayerType.Bg, 0);
			InitDefault(AdvLayerSettingData.LayerType.Character, 100);
			InitDefault(AdvLayerSettingData.LayerType.Sprite, 200);
		}

		private void InitDefault(AdvLayerSettingData.LayerType type, int defaultOrder)
		{
			AdvLayerSettingData advLayerSettingData = base.List.Find((AdvLayerSettingData item) => item.Type == type);
			if (advLayerSettingData == null)
			{
				advLayerSettingData = new AdvLayerSettingData();
				advLayerSettingData.InitDefault(type.ToString() + " Default", type, defaultOrder);
				AddData(advLayerSettingData);
			}
			advLayerSettingData.IsDefault = true;
		}

		public bool Contains(string layerName, AdvLayerSettingData.LayerType type)
		{
			if (base.Dictionary.TryGetValue(layerName, out var value))
			{
				return value.Type == type;
			}
			return false;
		}

		public bool Contains(string layerName)
		{
			return base.Dictionary.ContainsKey(layerName);
		}

		public AdvLayerSettingData FindDefaultLayer(AdvLayerSettingData.LayerType type)
		{
			return base.List.Find((AdvLayerSettingData item) => item.Type == type && item.IsDefault);
		}
	}
}
