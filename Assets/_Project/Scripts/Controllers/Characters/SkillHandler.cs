namespace _Project
{
	public class SkillHandler
	{
		private TSkills tSkills;
		private Timer timerReady;
		private float timerDelay;
		
		public SkillHandler(TSkills tSkills, float timerDelay)
		{
			this.tSkills = tSkills;
			this.timerDelay = timerDelay;
			this.timerReady = new Timer().Wait(this.timerDelay);
		}
		
		public void PauseInherit()
		{
			this.timerReady.Pause();
		}
		
		public void ResumeInherit()
		{
			this.timerReady.Resume();
		}
		
		public bool IfReady()
		{
			return this.timerReady.IfReady();
		}
		
		public void Reset()
		{
			this.timerReady.Wait(this.timerDelay);
		}
		
		public float GetReadyPercent()
		{
			return (float) this.timerReady.GetPassedTimeInPercent();
		}
		
		public TSkills GetTSkills()
		{
			return this.tSkills;
		}
	}
}
