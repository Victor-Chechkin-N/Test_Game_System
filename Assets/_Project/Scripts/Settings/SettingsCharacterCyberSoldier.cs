using UnityEngine;

namespace _Project
{
	public class SettingsCharacterCyberSoldier : SettingsCharacterBase
	{
		[Header("SettingsCharacterCyberSoldier:")]
		[SerializeField, AttributeSettingsController]
		private ControllerCharacterCyberSoldier controllerCharacterCyberSoldier;
		
		protected override void InitializeInherit()
		{
			
		}
	}
}