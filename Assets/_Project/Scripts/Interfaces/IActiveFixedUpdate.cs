namespace _Project
{
	public interface IActiveFixedUpdate 
	{
		public void FixedUpdateInherit();
		public void Pause();
		public void Resume();
		public bool IfPaused();
	}
}