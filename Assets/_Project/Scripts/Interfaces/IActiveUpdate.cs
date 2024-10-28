namespace _Project
{
	public interface IActiveUpdate
	{
		public void UpdateInherit();
		public void Pause();
		public void Resume();
		public bool IfPaused();
	}
}