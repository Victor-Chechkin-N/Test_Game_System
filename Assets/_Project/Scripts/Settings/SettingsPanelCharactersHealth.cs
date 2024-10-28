using UnityEngine;

namespace _Project
{
	public class SettingsPanelCharactersHealth : SettingsBase
	{
		[SerializeField, AttributeSettingsController]
		private ControllerCharacterHealth controllerCharacterHealth;
		
		protected override void InitializeInherit()
		{
			
		}
	}
}