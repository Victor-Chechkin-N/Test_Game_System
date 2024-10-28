using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public class ManagerSkillDistanceView : ManagerBase, IActiveUpdate
	{
		private SettingsSkillDistanceView.WaveSettings waveSettings;
		
		private List<ControllerCharacterBase> characters;
		
		private Timer createWaveTimer;
		
		protected override void InitializeInherit()
		{
			this.characters = new List<ControllerCharacterBase>();
			this.createWaveTimer = new Timer().Wait(0);
			
			var settingsSkillDistanceView = this.GetSettings<SettingsSkillDistanceView>();
			
			this.waveSettings = settingsSkillDistanceView.GetWaveSettings();
			
			this.AddControllersForCreate(settingsSkillDistanceView);
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
			if (this.createWaveTimer.IfReady())
			{
				this.createWaveTimer.Wait(this.waveSettings.createWaveDelay);
				
				for (int i = this.characters.Count - 1; i >= 0; i--)
				{
					var character = characters[i];
					if (!character.IfDead() && character.IfFightMode())
					{
						this.CreateControllerRadarWave(character, new Vector3());
					}
				}
			}
		}
		
		private ControllerDistanceViewWave CreateControllerRadarWave(ControllerCharacterBase character, Vector3 localPosition)
		{
			var skillUseRadiusSettings = character.GetSkillUseRadiusSettings();
			
			var c = this.CreateController<ControllerDistanceViewWave>(localPosition);
			c.SetFollowTarget(character.transform);
			c.SetSettings(this.waveSettings, skillUseRadiusSettings.radius, skillUseRadiusSettings.detectionAngle);
			return c;
		}
		
		public void AddPlayerToFollow(ControllerCharacterBase character)
		{
			this.characters.Add(character);
			
			character.OnActionDestroy += delegate
			{
				this.characters.Remove(character);
			};
		}
	}
}
