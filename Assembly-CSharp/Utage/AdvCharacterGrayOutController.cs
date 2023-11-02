using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/CharacterGrayOutContoller")]
	public class AdvCharacterGrayOutController : MonoBehaviour
	{
		[Flags]
		public enum LightingMask
		{
			Talking = 1,
			NewCharacerInPage = 2,
			NoChanageIfTextOnly = 4
		}

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		[EnumFlags]
		private LightingMask mask = LightingMask.Talking;

		[SerializeField]
		private Color mainColor = Color.white;

		[SerializeField]
		private Color subColor = Color.gray;

		[SerializeField]
		private float fadeTime = 0.2f;

		[SerializeField]
		private List<string> noGrayoutCharacters = new List<string>();

		private bool isChanged;

		private List<AdvGraphicLayer> pageBeginLayer;

		[SerializeField]
		private bool enableChangeOrder;

		[SerializeField]
		public int orderOffset = 100;

		private Dictionary<AdvGraphicLayer, int> defaultOrders = new Dictionary<AdvGraphicLayer, int>();

		public AdvEngine Engine => engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());

		public LightingMask Mask
		{
			get
			{
				return mask;
			}
			set
			{
				mask = value;
			}
		}

		public Color MainColor
		{
			get
			{
				return mainColor;
			}
			set
			{
				mainColor = value;
			}
		}

		public Color SubColor
		{
			get
			{
				return subColor;
			}
			set
			{
				subColor = value;
			}
		}

		public float FadeTime
		{
			get
			{
				return fadeTime;
			}
			set
			{
				fadeTime = value;
			}
		}

		public List<string> NoGrayoutCharacters
		{
			get
			{
				return noGrayoutCharacters;
			}
			set
			{
				noGrayoutCharacters = value;
			}
		}

		public bool EnableChangeOrder => enableChangeOrder;

		public int OrderOffset => orderOffset;

		private void Awake()
		{
			if (Engine != null)
			{
				Engine.Page.OnBeginPage.AddListener(OnBeginPage);
				Engine.Page.OnChangeText.AddListener(OnChangeText);
			}
		}

		private void OnBeginPage(AdvPage page)
		{
			pageBeginLayer = page.Engine.GraphicManager.CharacterManager.AllGraphicsLayers();
			if (mask != 0 || !isChanged)
			{
				return;
			}
			foreach (AdvGraphicLayer item in pageBeginLayer)
			{
				ChangeColor(item, MainColor);
			}
			isChanged = false;
		}

		private void OnChangeText(AdvPage page)
		{
			if (mask == (LightingMask)0)
			{
				return;
			}
			isChanged = true;
			AdvEngine advEngine = page.Engine;
			if (string.IsNullOrEmpty(page.CharacterLabel) && (Mask & LightingMask.NoChanageIfTextOnly) == LightingMask.NoChanageIfTextOnly)
			{
				return;
			}
			foreach (AdvGraphicLayer item in advEngine.GraphicManager.CharacterManager.AllGraphicsLayers())
			{
				bool flag = IsLightingCharacter(page, item);
				ChangeColor(item, flag ? MainColor : SubColor);
				ChangeOrder(item, flag);
			}
		}

		private void ChangeOrder(AdvGraphicLayer layer, bool isLighting)
		{
			if (EnableChangeOrder)
			{
				if (!defaultOrders.TryGetValue(layer, out var value))
				{
					value = layer.Canvas.sortingOrder;
					defaultOrders.Add(layer, layer.Canvas.sortingOrder);
				}
				layer.Canvas.sortingOrder = (isLighting ? (value + orderOffset) : value);
			}
		}

		private bool IsLightingCharacter(AdvPage page, AdvGraphicLayer layer)
		{
			if ((Mask & LightingMask.Talking) == LightingMask.Talking && layer.DefaultObject.name == page.CharacterLabel)
			{
				return true;
			}
			if ((Mask & LightingMask.NewCharacerInPage) == LightingMask.NewCharacerInPage && pageBeginLayer.Find((AdvGraphicLayer x) => x != null && x.DefaultObject != null && x.DefaultObject.name == layer.DefaultObject.name) == null)
			{
				return true;
			}
			if (NoGrayoutCharacters.Exists((string x) => x == layer.DefaultObject.name))
			{
				return true;
			}
			return false;
		}

		private void ChangeColor(AdvGraphicLayer layer, Color color)
		{
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in layer.CurrentGraphics)
			{
				AdvEffectColor component = currentGraphic.Value.gameObject.GetComponent<AdvEffectColor>();
				if (!(component == null))
				{
					if (FadeTime > 0f)
					{
						Color customColor = component.CustomColor;
						StartCoroutine(FadeColor(component, customColor, color));
					}
					else
					{
						component.CustomColor = color;
					}
				}
			}
		}

		private IEnumerator FadeColor(AdvEffectColor effect, Color from, Color to)
		{
			float elapsed = 0f;
			do
			{
				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
				if (elapsed >= fadeTime)
				{
					elapsed = fadeTime;
				}
				effect.CustomColor = Color.Lerp(from, to, elapsed / FadeTime);
			}
			while (!(elapsed >= fadeTime));
		}
	}
}
