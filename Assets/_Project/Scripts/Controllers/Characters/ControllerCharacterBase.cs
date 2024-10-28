using System.Collections.Generic;
using System.Linq;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using UnityEngine;

namespace _Project
{
	[SelectionBase]
	public abstract class ControllerCharacterBase : ControllerBase, IActiveUpdate
	{
		public delegate void ActionHealthChange(int healthMax, int healthCurrent);
		public event ActionHealthChange OnActionHealthChange;
		
		public event System.Action OnActionDead;
		
		[SerializeField]
		private Animator animator;
		[SerializeField]
		private NavMeshAgent navMeshAgent;
		
		private Rigidbody[] rigidbodies;
		
		private CharacterComponentSkills characterComponentSkills;
		private CharacterComponentLocomotion characterComponentLocomotion;
		private CharacterComponentTargetedMode characterComponentTargetedMode;
		
		// Move conditions.
		private SettingsCharacterBase.SkillUseRadiusSettings skillUseRadiusSettings;
		private SettingsCharacterBase.FightModeSettings fightModeSettings;
		
		private int hashIdleAnimation;
		
		private int healthMax;
		private int healthCurrent;
		
		private bool isDead;
		
		private TTeam tTeam;
		
		protected override void InitializeInherit()
		{
			this.hashIdleAnimation = Animator.StringToHash("Idle");
			
			this.rigidbodies ??= this.GetComponentsInChildren<Rigidbody>();
			
			this.characterComponentSkills ??= this.gameObject.AddComponent<CharacterComponentSkills>();
			this.characterComponentTargetedMode ??= this.gameObject.GetComponent<CharacterComponentTargetedMode>();
			this.characterComponentLocomotion ??= this.gameObject.AddComponent<CharacterComponentLocomotion>()
																	.SetNavMeshAgent(this.navMeshAgent)
																	.SetAnimator(this.animator);
			
			this.GetNavMeshAgent().enabled = true;
			this.GetAnimator().enabled = true;
			
			this.GetAnimator().Play(this.hashIdleAnimation);
			
			this.healthMax = 100;
			this.healthCurrent = this.healthMax;
			
			this.isDead = false;
			
			this.characterComponentSkills.InitializeInherit();
			this.characterComponentTargetedMode.InitializeInherit();
			this.characterComponentLocomotion.InitializeInherit();
			this.RagdollDisable();
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		protected override void PauseInherit()
		{
			if (this.animator.enabled)
			{
				this.animator.speed = 0;
			}
			
			this.characterComponentSkills.PauseInherit();
			this.characterComponentLocomotion.PauseInherit();
			this.characterComponentTargetedMode.PauseInherit();
		}
		
		protected override void ResumeInherit()
		{
			if (this.animator.enabled)
			{
				this.animator.speed = 1;
			}
			
			this.characterComponentSkills.ResumeInherit();
			this.characterComponentLocomotion.ResumeInherit();
			this.characterComponentTargetedMode.ResumeInherit();
		}
		
		public virtual void UpdateInherit()
		{
			this.characterComponentTargetedMode.UpdateInherit();
			
			if (!this.IfDead())
			{
				this.characterComponentLocomotion.UpdateInherit();
			}
		}
		
		private NavMeshAgent GetNavMeshAgent()
		{
			return this.navMeshAgent;
		}
		
		private Animator GetAnimator()
		{
			return this.animator;
		}
		
		public SettingsCharacterBase.SkillUseRadiusSettings GetSkillUseRadiusSettings()
		{
			return this.skillUseRadiusSettings;
		}
		
		public SettingsCharacterBase.FightModeSettings GetFightModeSettings()
		{
			return this.fightModeSettings;
		}
		
		public void SetSettings(SettingsCharacterBase.MoveSettings moveSettings, SettingsCharacterBase.SkillUseRadiusSettings skillUseRadiusSettings, SettingsCharacterBase.FightModeSettings fightModeSettings)
		{
			this.characterComponentLocomotion.SetSettings(moveSettings, skillUseRadiusSettings);
			
			this.skillUseRadiusSettings = skillUseRadiusSettings;
			this.fightModeSettings = fightModeSettings;
		}
		
		public void SetMaterials(SettingsCharacterBase.MaterialContainer materialContainer)
		{
			this.characterComponentTargetedMode.SetMaterials(materialContainer.materialDefault, materialContainer.materialTarget);
		}
		
		public void SetTTeam(TTeam value)
		{
			this.tTeam = value;
		}
		
		public TTeam GetTTeam()
		{
			return this.tTeam;
		}
		
		private void RagdollEnable()
		{
			for (var i = 0; i < this.rigidbodies.Length; i++)
			{
				this.rigidbodies[i].isKinematic = false;
			}
		}
		
		private void RagdollDisable()
		{
			for (var i = 0; i < this.rigidbodies.Length; i++)
			{
				this.rigidbodies[i].isKinematic = true;
			}
		}
		
		public void FightModeActivate()
		{
			this.characterComponentLocomotion.FightModeActivate();
		}
		
		public void FightModeDeactivate()
		{
			this.characterComponentLocomotion.FightModeDeactivate();
		}
		
		public bool IfFightMode()
		{
			return this.characterComponentLocomotion.IfFightMode();
		}
		
		public void TargetedStateActivate()
		{
			this.characterComponentTargetedMode.TargetedStateActivate();
		}
		
		public void SetFollowTarget(ControllerCharacterBase followTarget)
		{
			this.characterComponentLocomotion.SetFollowTarget(followTarget);
		}
		
		public bool UseSkillTry(out SkillHandler skillHandler)
		{
			return this.characterComponentSkills.UseSkillTry(out skillHandler);
		}
		
		public List<SkillHandler> GetSkillHandlerList()
		{
			return this.characterComponentSkills.GetSkillHandlerList();
		}
		
		public void AddSkill(SkillHandler skillHandler)
		{
			this.characterComponentSkills.AddSkill(skillHandler);
		}
		
		public void SetMoveVector(Vector3 moveVector, Vector3 cameraMainTransformForward, Vector3 cameraMainTransformRight)
		{
			this.characterComponentLocomotion.SetMoveVector(moveVector, cameraMainTransformForward, cameraMainTransformRight);
		}
		
		public void ReduceHealth(int value, Vector3 hitPosition, Collider hitCollider)
		{
			this.healthCurrent -= value;
			if (this.healthCurrent < 0)
			{
				this.healthCurrent = 0;
			}
			
			this.OnActionHealthChange?.Invoke(this.healthMax, this.healthCurrent);
			
			if (this.healthCurrent == 0)
			{
				this.DieAction();
				
				if (hitCollider != null)
				{
					var rigidbody = hitCollider.GetComponent<Rigidbody>();
					if (rigidbody == null)
					{
						rigidbody = this.rigidbodies.OrderBy(r => Vector3.Distance(r.position, hitPosition)).First();
					}
					
					if (rigidbody != null)
					{
						this.GetNavMeshAgent().enabled = false;
						rigidbody.AddForceAtPosition((rigidbody.position - hitPosition).normalized * 250, hitPosition, ForceMode.Impulse);
					}
				}
			}
		}
		
		private void DieAction()
		{
			if (!this.isDead)
			{
				this.isDead = true;
				
				this.OnActionDead?.Invoke();
				
				this.RagdollEnable();
				this.GetAnimator().enabled = false;
			}
		}
		
		public bool IfDead()
		{
			return this.isDead;
		}
	}
}
