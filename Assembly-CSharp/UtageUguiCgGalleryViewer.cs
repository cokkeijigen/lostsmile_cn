using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/CgGalleryViewer")]
public class UtageUguiCgGalleryViewer : UguiView, IPointerClickHandler, IEventSystemHandler, IDragHandler, IPointerDownHandler
{
	public UtageUguiGallery gallery;

	public AdvUguiLoadGraphicFile texture;

	[SerializeField]
	private AdvEngine engine;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private bool applyPosition;

	protected Vector3 startContentPosition;

	protected bool isEnableClick;

	protected bool isLoadEnd;

	protected AdvCgGalleryData data;

	protected int currentIndex;

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	public virtual ScrollRect ScrollRect
	{
		get
		{
			if (scrollRect == null)
			{
				scrollRect = GetComponent<ScrollRect>();
				if (scrollRect == null)
				{
					scrollRect = base.gameObject.AddComponent<ScrollRect>();
					scrollRect.movementType = ScrollRect.MovementType.Clamped;
				}
				if (scrollRect.content == null)
				{
					scrollRect.content = texture.transform as RectTransform;
				}
			}
			return scrollRect;
		}
	}

	protected virtual void Awake()
	{
		texture.OnLoadEnd.AddListener(OnLoadEnd);
	}

	public void Open(AdvCgGalleryData data)
	{
		gallery.Sleep();
		Open();
		this.data = data;
		currentIndex = 0;
		startContentPosition = ScrollRect.content.localPosition;
		LoadCurrentTexture();
	}

	protected virtual void OnClose()
	{
		ScrollRect.content.localPosition = startContentPosition;
		texture.ClearFile();
		gallery.WakeUp();
	}

	protected virtual void Update()
	{
		if (InputUtil.IsMouseRightButtonDown())
		{
			Back();
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (isLoadEnd)
		{
			isEnableClick = true;
		}
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (isEnableClick)
		{
			currentIndex++;
			if (currentIndex >= data.NumOpen)
			{
				Back();
			}
			else
			{
				LoadCurrentTexture();
			}
		}
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		isEnableClick = false;
	}

	protected virtual void LoadCurrentTexture()
	{
		isLoadEnd = false;
		isEnableClick = false;
		ScrollRect.enabled = false;
		ScrollRect.content.localPosition = startContentPosition;
		AdvTextureSettingData dataOpened = data.GetDataOpened(currentIndex);
		texture.LoadFile(Engine.DataManager.SettingDataManager.TextureSetting.LabelToGraphic(dataOpened.Key).Main);
	}

	protected virtual void OnLoadEnd()
	{
		isLoadEnd = true;
		isEnableClick = false;
		ScrollRect.enabled = true;
		if (applyPosition)
		{
			AdvGraphicInfo main = data.GetDataOpened(currentIndex).Graphic.Main;
			texture.transform.localPosition = main.Position;
		}
	}
}
