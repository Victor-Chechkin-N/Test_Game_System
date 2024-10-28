using UnityEngine;

namespace _Project
{
	public class SettingsCharacterCreature : SettingsCharacterBase
	{
		[Header("SettingsCharacterCyberSoldier:")]
		[SerializeField, AttributeSettingsController]
		private ControllerCharacterCreature controllerCharacterCreature;
		
		protected override void InitializeInherit()
		{
			
		}
	}
}