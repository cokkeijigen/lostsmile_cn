using UnityEngine;

namespace Utage
{
	public class AdvCommandZoomCamera : AdvCommandEffectBase
	{
		private bool isEmptyZoom;

		private float zoom;

		private bool isEmptyZoomCenter;

		private Vector2 zoomCenter;

		private float time;

		public AdvCommandZoomCamera(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			targetType = AdvEffectManager.TargetType.Camera;
			isEmptyZoom = IsEmptyCell(AdvColumnName.Arg2);
			zoom = ParseCellOptional(AdvColumnName.Arg2, 1f);
			isEmptyZoomCenter = IsEmptyCell(AdvColumnName.Arg3) && IsEmptyCell(AdvColumnName.Arg4);
			zoomCenter.x = ParseCellOptional(AdvColumnName.Arg3, 0f);
			zoomCenter.y = ParseCellOptional(AdvColumnName.Arg4, 0f);
			time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			if (target != null)
			{
				LetterBoxCamera camera = target.GetComponentInChildren<LetterBoxCamera>();
				float zoom0 = camera.Zoom2D;
				float zoomTo = (isEmptyZoom ? zoom0 : zoom);
				Vector2 center0 = ((zoom0 == 1f) ? zoomCenter : camera.Zoom2DCenter);
				Vector2 centerTo = (isEmptyZoomCenter ? center0 : zoomCenter);
				Timer timer = target.AddComponent<Timer>();
				timer.AutoDestroy = true;
				timer.StartTimer(engine.Page.ToSkippedTime(time), delegate
				{
					float curve = timer.GetCurve(zoom0, zoomTo);
					Vector2 curve2 = timer.GetCurve(center0, centerTo);
					camera.SetZoom2D(curve, curve2);
				}, delegate
				{
					if (zoomTo == 1f)
					{
						camera.Zoom2DCenter = Vector2.zero;
					}
					OnComplete(thread);
				});
			}
			else
			{
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundTweenGameObject, "SpriteCamera"));
				OnComplete(thread);
			}
		}
	}
}
