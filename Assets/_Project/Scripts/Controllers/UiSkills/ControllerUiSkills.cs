using UnityEngine;
using UnityEngine.UI;

namespace _Project
{
	public class ControllerUiSkills : ControllerBase, IActiveUpdate
	{
		[SerializeField]
		private Image background;
		[SerializeField]
		private Image foreground;
		
		private SkillHandler skillHandler;
		
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
		
		void IActiveUpdate.UpdateInherit()
		{
			this.UpdateView();
		}
		
		public void SetSkillSettings(SkillHandler skillHandler, SettingsUiSkills.UiSkillsSettings uiSkillsSettings)
		{
			this.skillHandler = skillHandler;
			this.background.sprite = uiSkillsSettings.iconPassive;
			this.foreground.sprite = uiSkillsSettings.iconActive;
			
			this.UpdateView();
		}
		
		private void UpdateView()
		{
			this.foreground.fillAmount = this.skillHandler.GetReadyPercent();
		}
	}
}
