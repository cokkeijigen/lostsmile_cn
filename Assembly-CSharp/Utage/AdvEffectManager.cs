using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/EffectManager")]
	public class AdvEffectManager : MonoBehaviour
	{
		public enum TargetType
		{
			Default,
			Camera,
			Graphics,
			MessageWindow
		}

		private AdvEngine engine;

		[SerializeField]
		private AdvUguiMessageWindowManager messageWindow;

		[SerializeField]
		private List<Texture2D> ruleTextureList = new List<Texture2D>();

		public AdvEngine Engine => engine ?? (engine = GetComponentInParent<AdvEngine>());

		private AdvUguiMessageWindowManager MessageWindow => messageWindow ?? (messageWindow = Engine.GetComponentInChildren<AdvUguiMessageWindowManager>(true));

		public List<Texture2D> RuleTextureList
		{
			get
			{
				return ruleTextureList;
			}
			set
			{
				ruleTextureList = value;
			}
		}

		internal Texture2D FindRuleTexture(string name)
		{
			foreach (Texture2D ruleTexture in ruleTextureList)
			{
				if (!(ruleTexture == null) && ruleTexture.name == name)
				{
					return ruleTexture;
				}
			}
			Debug.LogErrorFormat("Not Found Rule Texture [ {0} ]", name);
			return null;
		}

		internal GameObject FindTarget(AdvCommandEffectBase command)
		{
			return FindTarget(command.Target, command.TargetName);
		}

		internal GameObject FindTarget(TargetType targetType, string targetName)
		{
			switch (targetType)
			{
			case TargetType.MessageWindow:
				return MessageWindow.gameObject;
			case TargetType.Graphics:
				return Engine.GraphicManager.gameObject;
			case TargetType.Camera:
			{
				if (string.IsNullOrEmpty(targetName) || targetName == TargetType.Camera.ToString())
				{
					return Engine.CameraManager.gameObject;
				}
				CameraRoot cameraRoot = Engine.CameraManager.FindCameraRoot(targetName);
				if (cameraRoot == null)
				{
					return null;
				}
				return cameraRoot.gameObject;
			}
			default:
				return Engine.GraphicManager.FindObjectOrLayer(targetName);
			}
		}
	}
}
