using UnityEngine;

namespace _Project
{
	public class CharacterComponentTargetedMode : MonoBehaviour
	{
		[SerializeField]
		private SkinnedMeshRenderer[] mesh;
		
		private Material[] materialDefault;
		private Material[] materialTarget;
		
		private bool isTargetedMode;
		private Timer resetTargetModeTimer;
		
		public void InitializeInherit()
		{
			this.resetTargetModeTimer ??= new Timer();
			this.isTargetedMode = false;
		}
		
		public void UpdateInherit()
		{
			if (this.isTargetedMode && this.resetTargetModeTimer.IfReady())
			{
				this.UpdateTargetedState(false);
			}
		}
		
		public void PauseInherit()
		{
			this.resetTargetModeTimer.Pause();
		}
		
		public void ResumeInherit()
		{
			this.resetTargetModeTimer.Resume();
		}
		
		public void TargetedStateActivate()
		{
			this.resetTargetModeTimer.Wait(0.3f);
			this.UpdateTargetedState(true);
		}
		
		private void UpdateTargetedState(bool isEnable)
		{
			if (isEnable)
			{
				if (!this.isTargetedMode)
				{
					this.isTargetedMode = true;
					
					for (int i = 0; i < this.mesh.Length; i++)
					{
						this.mesh[i].material = this.materialTarget[Mathf.Clamp(i, 0, this.materialTarget.Length - 1)];
					}
				}
			}
			else
			{
				if (this.isTargetedMode)
				{
					this.isTargetedMode = false;
					
					for (int i = 0; i < this.mesh.Length; i++)
					{
						this.mesh[i].material = this.materialDefault[Mathf.Clamp(i, 0, this.materialDefault.Length - 1)];
					}
				}
			}
		}
		
		public void SetMaterials(Material[] materialDefault, Material[] materialTarget)
		{
			this.materialDefault = materialDefault;
			this.materialTarget = materialTarget;
			
			for (int i = 0; i < this.mesh.Length; i++)
			{
				this.mesh[i].material = this.materialDefault[Mathf.Clamp(i, 0, this.materialDefault.Length - 1)];
			}
		}
	}
}