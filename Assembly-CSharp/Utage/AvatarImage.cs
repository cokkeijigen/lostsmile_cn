using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/AvatarImage")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class AvatarImage : MonoBehaviour
	{
		[SerializeField]
		private AvatarData avatarData;

		[SerializeField]
		[NovelAvatarPattern("AvatarData")]
		private AvatarPattern avatarPattern = new AvatarPattern();

		private RectTransform cachedRectTransform;

		[SerializeField]
		private Material material;

		public UnityEvent OnPostRefresh = new UnityEvent();

		[SerializeField]
		private bool flipX;

		[SerializeField]
		private bool flipY;

		private RectTransform rootChildren;

		public AvatarData AvatarData
		{
			get
			{
				return avatarData;
			}
			set
			{
				avatarData = value;
				avatarPattern.Rebuild(AvatarData);
				HasChanged = true;
			}
		}

		public AvatarPattern AvatarPattern
		{
			get
			{
				return avatarPattern;
			}
			set
			{
				avatarPattern = value;
				HasChanged = true;
			}
		}

		public RectTransform CachedRectTransform
		{
			get
			{
				if (cachedRectTransform == null)
				{
					cachedRectTransform = GetComponent<RectTransform>();
				}
				return cachedRectTransform;
			}
		}

		public Material Material
		{
			get
			{
				return material;
			}
			set
			{
				material = value;
				HasChanged = true;
			}
		}

		public bool FlipX
		{
			get
			{
				return flipX;
			}
			set
			{
				flipX = value;
				HasChanged = true;
			}
		}

		public bool FlipY
		{
			get
			{
				return flipY;
			}
			set
			{
				flipY = value;
				HasChanged = true;
			}
		}

		private RectTransform RootChildren
		{
			get
			{
				if (rootChildren == null)
				{
					rootChildren = base.transform.AddChildGameObjectComponent<RectTransform>("childRoot");
					rootChildren.gameObject.hideFlags = HideFlags.DontSave;
				}
				return rootChildren;
			}
		}

		private bool HasChanged { get; set; }

		public void Flip(bool flipX, bool flipY)
		{
			FlipX = flipX;
			FlipY = flipY;
		}

		private void OnEnable()
		{
			HasChanged = true;
		}

		private void Update()
		{
			if (HasChanged)
			{
				Refresh();
				HasChanged = false;
			}
		}

		private void Refresh()
		{
			RootChildren.DestroyChildrenInEditorOrPlayer();
			avatarPattern.Rebuild(AvatarData);
			MakeImageFromAvartorData(AvatarData);
			OnPostRefresh.Invoke();
		}

		private void MakeImageFromAvartorData(AvatarData data)
		{
			if (AvatarData == null)
			{
				return;
			}
			data.CheckPatternError(avatarPattern);
			foreach (Sprite item in data.MakeSortedSprites(avatarPattern))
			{
				if (!(item == null))
				{
					RectTransform rectTransform = RootChildren.AddChildGameObjectComponent<RectTransform>(item.name);
					rectTransform.gameObject.hideFlags = HideFlags.DontSave;
					Image image = rectTransform.gameObject.AddComponent<Image>();
					image.material = Material;
					image.sprite = item;
					image.SetNativeSize();
					UguiFlip uguiFlip = image.gameObject.AddComponent<UguiFlip>();
					uguiFlip.FlipX = flipX;
					uguiFlip.FlipY = FlipY;
				}
			}
		}

		public void ChangePattern(string tag, string patternName)
		{
			avatarPattern.SetPatternName(tag, patternName);
			HasChanged = true;
		}
	}
}
