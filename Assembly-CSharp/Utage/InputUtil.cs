using System;
using UnityEngine;

namespace Utage
{
	public static class InputUtil
	{
		private static float wheelSensitive = 0.1f;

		[Obsolete("Use IsMouseRightButtonDown instead")]
		public static bool IsMousceRightButtonDown()
		{
			return IsMouseRightButtonDown();
		}

		public static bool EnableWebGLInput()
		{
			return Application.platform == RuntimePlatform.WebGLPlayer;
		}

		public static bool IsMouseRightButtonDown()
		{
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				return Input.GetMouseButtonDown(1);
			}
			return false;
		}

		public static bool IsInputControl()
		{
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				if (!Input.GetKey(KeyCode.LeftControl))
				{
					return Input.GetKey(KeyCode.RightControl);
				}
				return true;
			}
			return false;
		}

		public static bool IsInputScrollWheelUp()
		{
			if (Input.GetAxis("Mouse ScrollWheel") >= wheelSensitive)
			{
				return true;
			}
			return false;
		}

		public static bool IsInputScrollWheelDown()
		{
			if (Input.GetAxis("Mouse ScrollWheel") <= 0f - wheelSensitive)
			{
				return true;
			}
			return false;
		}

		public static bool IsInputKeyboadReturnDown()
		{
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				return Input.GetKeyDown(KeyCode.Return);
			}
			return false;
		}
	}
}
