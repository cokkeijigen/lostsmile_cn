using System;

namespace Utage
{
	[Flags]
	public enum AssetFileLoadFlags
	{
		None = 0,
		Streaming = 1,
		Audio3D = 2,
		TextureMipmap = 4,
		Tsv = 8
	}
}
