using UnityEngine;

namespace _Project
{
	public class SettingsUiSkills : SettingsBase
	{
		[SerializeField, AttributeSettingsController]
		private ControllerUiSkills controllerUiSkills;
		
		[SerializeField, AttributeEditorArrayElementTitle("tSkills")]
		private UiSkillsSettings[] uiSkillsSettings;
		
		protected override void InitializeInherit()
		{
			
		}
		
		public UiSkillsSettings GetUiSkillsSettings(TSkills tSkills)
		{
			for (int i = 0; i < this.uiSkillsSettings.Length; i++)
			{
				if (this.uiSkillsSettings[i].tSkills == tSkills)
				{
					return this.uiSkillsSettings[i];
				}
			}
			
			return null;
		}
		
		[System.Serializable]
		public class UiSkillsSettings
		{
			public TSkills tSkills;
			public Sprite iconActive;
			public Sprite iconPassive;
		}
	}
}