using System.IO;

namespace Utage
{
	public interface IBinaryIO
	{
		string SaveKey { get; }

		void OnWrite(BinaryWriter writer);

		void OnRead(BinaryReader reader);
	}
}
