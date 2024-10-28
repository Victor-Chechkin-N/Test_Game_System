using System.Threading.Tasks;

namespace _Project
{
	public class ControllerBulletFireExplosion : ControllerBase
	{
		
		protected override void InitializeInherit()
		{
			this.DestroyWithDelay();
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		protected override void PauseInherit()
		{
			
		}
		
		protected override void ResumeInherit()
		{
			
		}
		
		private async void DestroyWithDelay()
		{
			await Task.Delay(1000);
			
			if (this != null && this.gameObject != null)
			{
				this.Destroy();
			}
		}
	}
}
