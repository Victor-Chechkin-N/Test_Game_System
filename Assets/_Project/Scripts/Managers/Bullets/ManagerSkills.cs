using UnityEngine;

namespace _Project
{
	public class ManagerSkills : ManagerBase
	{
		private SettingsSkills settingsSkills;
		
		protected override void InitializeInherit()
		{
			this.settingsSkills = this.GetSettings<SettingsSkills>();
			
			this.AddControllersForCreate(this.settingsSkills);
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
		
		public void SkillUse(SkillHandler skillHandler, ControllerCharacterBase from, ControllerCharacterBase target)
		{
			bool isResetSkill = false;
			var tSkills = skillHandler.GetTSkills();
			
			switch (tSkills)
			{
				case TSkills.BulletFire:
					isResetSkill = true;
					this.CreateControllerBulletFire(tSkills, from, target);
					break;
			}
			
			if (isResetSkill)
			{
				skillHandler.Reset();
			}
		}
		
		private void PrepareBullet(TSkills tSkills, ControllerBulletBase bullet, ControllerCharacterBase owner, ControllerCharacterBase target)
		{
			var targetPosition = target.transform.position.ModifyReplace(null, 1, null);
			
			bullet.SetSettings(this.settingsSkills.GetSkillSettings(tSkills));
			bullet.SetTarget(owner, target, targetPosition);
			bullet.SetTTeam(owner.GetTTeam());
			
			bullet.OnActionCharacterCollide += delegate (ControllerBulletBase bulletBase, ControllerCharacterBase character, Vector3 hitPosition, Collider hitCollider)
			{
				if (bulletBase.GetTTeam() != character.GetTTeam())
				{
					character.ReduceHealth(bulletBase.GetDamage(), hitPosition, hitCollider);
					bulletBase.Destroy();
				}
			};
		}
		
		private ControllerBulletFire CreateControllerBulletFire(TSkills tSkills, ControllerCharacterBase owner, ControllerCharacterBase target)
		{
			var createPosition = owner.transform.position.ModifyReplace(null, 1, null);
			
			var c = this.CreateController<ControllerBulletFire>(createPosition);
			
			this.PrepareBullet(tSkills, c, owner, target);
			
			c.OnActionDestroy += delegate
			{
				this.CreateControllerBulletFireExplosion(c.transform.position);
			};
			
			return c;
		}
		
		private ControllerBulletFireExplosion CreateControllerBulletFireExplosion(Vector3 localPosition)
		{
			return this.CreateController<ControllerBulletFireExplosion>(localPosition);
		}
	}
}
