namespace _Project
{
	public interface IActiveLateUpdate
	{
		public void LateUpdateInherit();
		public void Pause();
		public void Resume();
		public bool IfPaused();
	}
}