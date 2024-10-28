using UnityEngine;
using UnityEngine.AI;

namespace _Project
{
	public class CharacterComponentLocomotion : MonoBehaviour
	{
		private Animator animator;
		private NavMeshAgent navMeshAgent;
		
		private SettingsCharacterBase.MoveSettings moveSettings;
		private SettingsCharacterBase.SkillUseRadiusSettings skillUseRadiusSettings;
		
		private int hashFightMode;
		private int hashIsMoving;
		private int hashTurn;
		private int hashForward;
		
		private ControllerCharacterBase followTarget;
		private Vector3 followTargetRandomBias;
		private Timer followTargetRandomBiasChangeTimer;
		
		// Move conditions.
		private Vector3 moveAnimationBlend;
		
		private bool isMoving;
		private Vector3 lastForward;
		private float moveX;
		private float moveZ;
		private Vector3 rotationDirectionForward;
		private Vector3 rotationDirectionSideRight;
		
		private Vector3 movingTo;
		
		// Animator turning sensitivity.
		private float turnSensitivity = 0.2f;
		
		private bool isFightMode;
		private Timer fightModeActivateBlockTimer;
		
		private Vector3 navMeshAgentVelocityBeforePause;
		
		public void InitializeInherit()
		{
			this.hashFightMode = Animator.StringToHash("FightMode");
			this.hashIsMoving = Animator.StringToHash("IsMoving");
			this.hashTurn = Animator.StringToHash("Turn");
			this.hashForward = Animator.StringToHash("Forward");
			
			this.GetAnimator().SetBool(this.hashFightMode, false);
			this.GetAnimator().SetBool(this.hashIsMoving, false);
			
			this.lastForward = this.transform.forward;
			
			this.isFightMode = false;
			(this.fightModeActivateBlockTimer ??= new Timer()).Wait(0);
			
			this.followTarget = null;
		}
		
		public void PauseInherit()
		{
			this.navMeshAgentVelocityBeforePause = this.GetNavMeshAgent().velocity;
			
			this.fightModeActivateBlockTimer.Pause();
			this.GetNavMeshAgent().speed = 0;
			this.GetNavMeshAgent().velocity = Vector3.zero;
		}
		
		public void ResumeInherit()
		{
			this.fightModeActivateBlockTimer.Resume();
			this.UpdateMoveSpeed();
			this.GetNavMeshAgent().velocity = this.navMeshAgentVelocityBeforePause;
		}
		
		public void UpdateInherit()
		{
			if (this.followTarget != null && !this.followTarget.IfDead())
			{
				Vector3 moveRandomBias = Vector3.one;
				
				float distance = Vector3.Distance(this.transform.position, this.followTarget.transform.position);
				if (distance < this.skillUseRadiusSettings.detectionAngle * 0.9f)
				{
					if (this.followTargetRandomBiasChangeTimer == null || this.followTargetRandomBiasChangeTimer.IfReady())
					{
						(this.followTargetRandomBiasChangeTimer ??= new Timer()).Wait(Random.Range(0.5f, 1.5f));
						this.followTargetRandomBias = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
					}
					
					moveRandomBias = this.followTargetRandomBias;
				}
				
				Vector3 direction = (this.transform.position - this.followTarget.transform.position).normalized;
				this.moveX = direction.x * moveRandomBias.x;
				this.moveZ = direction.z * moveRandomBias.z;
				
				this.rotationDirectionForward = (this.followTarget.transform.position - this.transform.position).normalized;
				this.rotationDirectionSideRight = this.transform.right;
			}
			
			if (this.moveX != 0 || this.moveZ != 0)
			{
				this.isMoving = true;
				this.GetNavMeshAgent().isStopped = false;
				
				this.Move(new Vector3(this.moveX, 0f, this.moveZ));
				this.GetAnimator().SetBool(this.hashIsMoving, true);
			}
			else
			{
				if (this.isMoving)
				{
					this.MoveToStop();
				}
			}
		}
		
		public void SetSettings(SettingsCharacterBase.MoveSettings moveSettings, SettingsCharacterBase.SkillUseRadiusSettings skillUseRadiusSettings)
		{
			this.moveSettings = moveSettings;
			this.skillUseRadiusSettings = skillUseRadiusSettings;
		}
		
		public CharacterComponentLocomotion SetNavMeshAgent(NavMeshAgent component)
		{
			this.navMeshAgent = component;
			return this;
		}
		
		private NavMeshAgent GetNavMeshAgent()
		{
			return this.navMeshAgent;
		}
		
		public CharacterComponentLocomotion SetAnimator(Animator component)
		{
			this.animator = component;
			return this;
		}
		
		private Animator GetAnimator()
		{
			return this.animator;
		}
		
		private void Move(Vector3 moveVector)
		{
			Vector3 forwardDirection = this.rotationDirectionForward * moveVector.z;
			forwardDirection = new Vector3(forwardDirection.x, 0, forwardDirection.z);
			Vector3 sideDirection = this.rotationDirectionSideRight * moveVector.x;
			Vector3 moveDirection = forwardDirection + sideDirection;
			
			Vector3 newPosition = this.transform.position + moveDirection * 2;
			this.MoveTo(newPosition);
			this.MoveAnimation(moveVector);
			
			if (this.IfFightMode())
			{
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.rotationDirectionForward), Time.deltaTime * 10);
			}
		}
		
		private void MoveTo(Vector3 position)
		{
			if (!this.GetNavMeshAgent().enabled)
			{
				return;
			}
			
			this.movingTo = position;
			
			this.GetNavMeshAgent().SetDestination(this.movingTo);
			
			if (this.IfFightMode())
			{
				this.GetNavMeshAgent().angularSpeed = 0f;
				this.GetAnimator().applyRootMotion = false;
			}
			else
			{
				this.GetNavMeshAgent().angularSpeed = this.moveSettings.angularSpeed;
				this.GetAnimator().applyRootMotion = true;
			}
			
			this.GetNavMeshAgent().acceleration = this.moveSettings.acceleration;
		}
		
		private void MoveAnimation(Vector3 moveVector)
		{
			// Locomotion animation blending.
			this.moveAnimationBlend = Vector3.Lerp(this.moveAnimationBlend, moveVector, Time.deltaTime * this.moveSettings.blendSpeed);
			
			this.GetAnimator().SetFloat("X", this.moveAnimationBlend.x);
			this.GetAnimator().SetFloat("Z", this.moveAnimationBlend.z);
			
			float angle = -this.GetAngleFromForward(this.lastForward);
			this.lastForward = this.transform.forward;
			angle *= this.turnSensitivity * 0.01f;
			angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);
			
			float turnSpeed = 5f;
			this.GetAnimator().SetFloat(this.hashTurn, Mathf.Lerp(this.GetAnimator().GetFloat(this.hashTurn), angle, Time.deltaTime * turnSpeed));
			this.GetAnimator().SetFloat(this.hashForward, 1f);
		}
		
		private float GetAngleFromForward(Vector3 worldDirection)
		{
			Vector3 local = this.transform.InverseTransformDirection(worldDirection);
			return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
		}
		
		private void MoveToStop()
		{
			this.isMoving = false;
			
			this.GetNavMeshAgent().velocity = Vector3.zero;
			this.GetNavMeshAgent().isStopped = true;
			
			this.GetAnimator().SetBool(this.hashIsMoving, false);
		}
		
		public void SetMoveVector(Vector3 moveVector, Vector3 rotationDirectionForward, Vector3 rotationDirectionSideRight)
		{
			this.moveX = moveVector.x;
			this.moveZ = moveVector.z;
			this.rotationDirectionForward = rotationDirectionForward;
			this.rotationDirectionSideRight = rotationDirectionSideRight;
		}
		
		public void FightModeActivate()
		{
			if (!this.isFightMode && this.fightModeActivateBlockTimer.IfReady())
			{
				this.isFightMode = true;
				this.GetAnimator().SetBool(this.hashFightMode, true);
				this.UpdateMoveSpeed();
			}
		}
		
		public void FightModeDeactivate()
		{
			if (this.isFightMode)
			{
				this.isFightMode = false;
				this.fightModeActivateBlockTimer.Wait(1f);
				
				this.GetAnimator().SetBool(this.hashFightMode, false);
				this.UpdateMoveSpeed();
			}
		}
		
		public bool IfFightMode()
		{
			return this.isFightMode;
		}
		
		private void UpdateMoveSpeed()
		{
			this.GetNavMeshAgent().speed = this.IfFightMode() ? this.moveSettings.moveSpeedFightMode : this.moveSettings.moveSpeedDefault;
		}
		
		public void SetFollowTarget(ControllerCharacterBase followTarget)
		{
			this.followTarget = followTarget;
		}
	}
}
