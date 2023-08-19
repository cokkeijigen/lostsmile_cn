using System;

namespace Utage
{
	[Flags]
	public enum AssetBundleTargetFlags
	{
		Android = 1,
		iOS = 2,
		WebGL = 4,
		Windows = 8,
		OSX = 0x10
	}
}
