using UnityEngine;

namespace _Project
{
	public class PanelJoystick : PanelBase
	{
		[SerializeField]
		private ControllerJoystick controllerJoystick;
		
		protected override void InitializeInherit()
		{
			
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
		
		public Vector3 GetMoveVector()
		{
			return this.controllerJoystick.GetMoveVector();
		}
	}
}
