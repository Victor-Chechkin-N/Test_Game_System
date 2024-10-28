using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public class ManagerCharacters : ManagerBase, IActiveUpdate
	{
		private SettingsCharacterPlayer settingsCharacterPlayer;
		private SettingsCharacterCreature settingsCharacterCreature;
		private SettingsCharacterCyberSoldier settingsCharacterCyberSoldier;
		private SettingsCharacterSticky settingsCharacterSticky;
		
		private SettingsPanelCharactersHealth settingsPanelCharactersHealth;
		private SettingsSkills settingsSkills;
		
		private PanelJoystick panelJoystick;
		private PanelCharactersHealth panelCharactersHealth;
		
		private ManagerSkills managerSkills;
		
		private ControllerCharacterPlayer playerMain;
		private ControllerCameraMain controllerCameraMain;
		
		private Dictionary<TTeam, List<ControllerCharacterBase>> charactersContainer;
		
		protected override void InitializeInherit()
		{
			this.charactersContainer = new Dictionary<TTeam, List<ControllerCharacterBase>>();
			
			this.settingsCharacterPlayer = this.GetSettings<SettingsCharacterPlayer>();
			this.settingsCharacterSticky = this.GetSettings<SettingsCharacterSticky>();
			this.settingsCharacterCyberSoldier = this.GetSettings<SettingsCharacterCyberSoldier>();
			this.settingsCharacterCreature = this.GetSettings<SettingsCharacterCreature>();
			
			this.settingsPanelCharactersHealth = this.GetSettings<SettingsPanelCharactersHealth>();
			this.settingsSkills = this.GetSettings<SettingsSkills>();
			
			this.panelJoystick = this.AddPanel<PanelJoystick>();
			this.panelCharactersHealth = this.AddPanel<PanelCharactersHealth>();
			
			this.AddControllersForCreate(this.settingsCharacterPlayer, this.settingsCharacterSticky, this.settingsCharacterCyberSoldier, this.settingsCharacterCreature, this.settingsPanelCharactersHealth);
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
		
		void IActiveUpdate.UpdateInherit()
		{
			if (!this.playerMain.IfDead())
			{
				this.playerMain.SetMoveVector(this.panelJoystick.GetMoveVector(), this.controllerCameraMain.transform.forward, this.controllerCameraMain.transform.right);
			}
			
			foreach (var characterInTeam in this.charactersContainer)
			{
				var tTeam = characterInTeam.Key;
				var characters = characterInTeam.Value;
				
				for (int i = characters.Count - 1; i >= 0; i--)
				{
					if (!characters[i].IfDead())
					{
						this.ProcessCharacter(characters[i], tTeam);
					}
				}
			}
		}
		
		private void ProcessCharacter(ControllerCharacterBase character, TTeam tCharacterTeam)
		{
			var skillUseRadiusSettings = character.GetSkillUseRadiusSettings();
			var fightModeSettings = character.GetFightModeSettings();
			
			var detectionRadius = skillUseRadiusSettings.radius;
			var detectionAngle = skillUseRadiusSettings.detectionAngle;
			var characterDirection = character.transform.forward;
			var characterPosition = character.transform.position;
			
			ControllerCharacterBase closestEnemy = null;
			var closestEnemyDistance = 9999f;
			
			ControllerCharacterBase closestAlly = null;
			var closestAllyDistance = 9999f;
			
			ControllerCharacterBase followTarget = null;
			var followTargetDistance = 9999f;
			
			var isActivateFightMode = false;
			
			foreach (var characterInTeam in this.charactersContainer)
			{
				var tTeam = characterInTeam.Key;
				var characters = characterInTeam.Value;
				var isAlly = tTeam == tCharacterTeam;
				
				for (int i = characters.Count - 1; i >= 0; i--)
				{
					var otherCharacter = characters[i];
					if (otherCharacter.IfDead())
					{
						continue;
					}
					
					// Don't process itself.
					if (isAlly && characters[i] == character)
					{
						continue;
					}
					
					var otherCharacterPosition = otherCharacter.transform.position;
					float distance = Vector3.Distance(characterPosition, otherCharacterPosition);
					
					if (isAlly)
					{
						if (distance < closestAllyDistance)
						{
							closestAllyDistance = distance;
							closestAlly = otherCharacter;
						}
					}
					else
					{
						var directionToEnemy = otherCharacterPosition - characterPosition;
						
						if (directionToEnemy.magnitude <= detectionRadius && Vector3.Angle(characterDirection, directionToEnemy) <= detectionAngle)
						{
							if (distance < closestEnemyDistance)
							{
								closestEnemyDistance = distance;
								closestEnemy = otherCharacter;
							}
						}
						
						if (distance < followTargetDistance)
						{
							followTargetDistance = distance;
							followTarget = otherCharacter;
						}
					}
					
					if (!isActivateFightMode && !isAlly && character.transform.position.IfCloserToTargetThan(otherCharacter.transform.position, fightModeSettings.enemiesInRadius))
					{
						isActivateFightMode = true;
					}
				}
			}
			
			if (closestEnemy != null)
			{
				closestEnemy.TargetedStateActivate();
				
				if (character.UseSkillTry(out SkillHandler skillHandler))
				{
					this.managerSkills.SkillUse(skillHandler, character, closestEnemy);
				}
			}
			
			if (followTarget != null && character is not ControllerCharacterPlayer)
			{
				character.SetFollowTarget(followTarget);
			}
			
			if (isActivateFightMode)
			{
				character.FightModeActivate();
			}
			else
			{
				character.FightModeDeactivate();
			}
		}
		
		public void SetControllerCameraMain(ControllerCameraMain controllerCameraMain)
		{
			this.controllerCameraMain = controllerCameraMain;
		}
		
		public void SetManagerBullets(ManagerSkills managerSkills)
		{
			this.managerSkills = managerSkills;
		}
		
		private void PrepareCharacter(ControllerCharacterBase character, TSkills[] tSkills, TTeam tTeam)
		{
			if (!this.charactersContainer.ContainsKey(tTeam))
			{
				this.charactersContainer.Add(tTeam, new List<ControllerCharacterBase>());
			}
			
			this.charactersContainer[tTeam].Add(character);
			
			character.SetTTeam(tTeam);
			
			if (tSkills != null)
			{
				for (int i = 0; i < tSkills.Length; i++)
				{
					character.AddSkill(
						new SkillHandler(tSkills[i], this.settingsSkills.GetSkillSettings(tSkills[i]).reloadTime)
					);
				}
			}
			
			var characterHealth = this.CreateController<ControllerCharacterHealth>(new Vector3(), this.panelCharactersHealth.transform);
			characterHealth.SetFollowCharacter(character, this.controllerCameraMain);
			characterHealth.SetHealthInPercent(1, false);
			
			character.OnActionDestroy += delegate
			{
				this.charactersContainer[tTeam].Remove(character);
				
				if (!character.IfDead())
				{
					characterHealth.Destroy();
				}
			};
			
			character.OnActionHealthChange += delegate(int healthMax, int healthCurrent)
			{
				characterHealth.SetHealthInPercent((float) healthCurrent / healthMax, true);
			};
			
			character.OnActionDead += delegate
			{
				characterHealth.Destroy();
			};
		}
		
		public ControllerCharacterPlayer CreateControllerPlayer(Vector3 localPosition)
		{
			var tTeam = TTeam.Team1;
			
			this.playerMain = this.CreateController<ControllerCharacterPlayer>(localPosition);
			this.playerMain.SetSettings(this.settingsCharacterPlayer.GetMoveSettings(), this.settingsCharacterPlayer.GetSkillUseRadiusSettings(), this.settingsCharacterPlayer.GetFightModeSettings());
			this.playerMain.SetMaterials(this.settingsCharacterPlayer.GetMaterialContainer(tTeam));
			
			this.PrepareCharacter(this.playerMain, this.settingsCharacterPlayer.GetSkills(), tTeam);
			
			return this.playerMain;
		}
		
		public ControllerCharacterSticky CreateControllerSticky(Vector3 localPosition, TTeam tTeam)
		{
			var c = this.CreateController<ControllerCharacterSticky>(localPosition);
			c.SetSettings(this.settingsCharacterSticky.GetMoveSettings(), this.settingsCharacterSticky.GetSkillUseRadiusSettings(), this.settingsCharacterSticky.GetFightModeSettings());
			c.SetMaterials(this.settingsCharacterSticky.GetMaterialContainer(tTeam));
			
			this.PrepareCharacter(c, this.settingsCharacterSticky.GetSkills(), tTeam);
			
			return c;
		}
		
		public ControllerCharacterCyberSoldier CreateControllerCyberSoldier(Vector3 localPosition, TTeam tTeam)
		{
			var c = this.CreateController<ControllerCharacterCyberSoldier>(localPosition);
			c.SetSettings(this.settingsCharacterCyberSoldier.GetMoveSettings(), this.settingsCharacterCyberSoldier.GetSkillUseRadiusSettings(), this.settingsCharacterCyberSoldier.GetFightModeSettings());
			c.SetMaterials(this.settingsCharacterCyberSoldier.GetMaterialContainer(tTeam));
			
			this.PrepareCharacter(c, this.settingsCharacterCyberSoldier.GetSkills(), tTeam);
			
			return c;
		}
		
		public ControllerCharacterCreature CreateControllerCreature(Vector3 localPosition, TTeam tTeam)
		{
			var c = this.CreateController<ControllerCharacterCreature>(localPosition);
			c.SetSettings(this.settingsCharacterCreature.GetMoveSettings(), this.settingsCharacterCreature.GetSkillUseRadiusSettings(), this.settingsCharacterCreature.GetFightModeSettings());
			c.SetMaterials(this.settingsCharacterCreature.GetMaterialContainer(tTeam));
			
			this.PrepareCharacter(c, this.settingsCharacterCreature.GetSkills(), tTeam);
			
			return c;
		}
	}
}
