using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public class ManagerUiSkills : ManagerBase
	{
		private SettingsUiSkills settingsUiSkills;
		
		private PanelPlayerSkills panelPlayerSkills;
		
		private ControllerCharacterBase character;
		
		private List<ControllerUiSkills> controllersList;
		
		protected override void InitializeInherit()
		{
			this.controllersList = new List<ControllerUiSkills>();
			
			this.settingsUiSkills = this.GetSettings<SettingsUiSkills>();
			this.panelPlayerSkills = this.AddPanel<PanelPlayerSkills>();
			
			this.AddControllersForCreate(this.settingsUiSkills);
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
		
		private ControllerUiSkills CreateControllerUiSkills(Vector3 localPosition, SkillHandler skillHandler)
		{
			var c = this.CreateController<ControllerUiSkills>(localPosition, this.panelPlayerSkills.rectTransform);
			var uiSkillsSettings = this.settingsUiSkills.GetUiSkillsSettings(skillHandler.GetTSkills());
			c.SetSkillSettings(skillHandler, uiSkillsSettings);
			return c;
		}
		
		public void AddPlayerToFollow(ControllerCharacterBase character)
		{
			for (int i = this.controllersList.Count - 1; i >= 0; i--)
			{
				this.controllersList[i].Destroy();
			}
			
			this.controllersList = new List<ControllerUiSkills>();
			
			this.character = character;
			
			var skillHandlersList = this.character.GetSkillHandlerList();
			for (int i = 0; i < skillHandlersList.Count; i++)
			{
				var controllerUiSkills = this.CreateControllerUiSkills(new Vector3(-100 * (i + 1), -100), skillHandlersList[i]);
				this.controllersList.Add(controllerUiSkills);
			}
		}
	}
}
