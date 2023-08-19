using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/ButtonSe")]
	public class UguiButtonSe : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, ISubmitHandler, IMoveHandler
	{
		private Selectable selectable;

		public AudioClip clicked;

		public AudioClip highlited;

		public SoundPlayMode clickedPlayMode;

		public SoundPlayMode highlitedPlayMode;

		private Selectable Selectable
		{
			get
			{
				return selectable ?? (selectable = GetComponent<Selectable>());
			}
		}

		public void OnPointerClick(PointerEventData data)
		{
			int pointerId = data.pointerId;
			if ((uint)(pointerId - -1) <= 1u)
			{
				PlayeSe(clickedPlayMode, clicked);
			}
		}

		public void OnPointerEnter(PointerEventData data)
		{
			PlayeSe(highlitedPlayMode, highlited);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			PlayeSe(clickedPlayMode, clicked);
		}

		public void OnMove(AxisEventData eventData)
		{
			if (!(eventData.selectedObject == base.gameObject))
			{
				PlayeSe(highlitedPlayMode, highlited);
			}
		}

		private void PlayeSe(SoundPlayMode playMode, AudioClip clip)
		{
			if (!(Selectable == null) && Selectable.interactable && clip != null)
			{
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.PlaySe(clip, clip.name, playMode);
				}
				else
				{
					AudioSource.PlayClipAtPoint(clip, Vector3.zero);
				}
			}
		}
	}
}
