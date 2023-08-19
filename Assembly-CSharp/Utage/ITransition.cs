namespace Utage
{
	public interface ITransition
	{
		bool IsPlaying { get; }

		void Open();

		void Close();

		void CancelClosing();
	}
}
