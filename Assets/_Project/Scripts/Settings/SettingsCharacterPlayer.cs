using UnityEngine;

namespace _Project
{
	public class SettingsCharacterPlayer : SettingsCharacterBase
	{
		[Header("SettingsCharacterPlayer:")]
		[SerializeField, AttributeSettingsController]
		private ControllerCharacterPlayer controllerCharacterPlayer;
		
		protected override void InitializeInherit()
		{
			
		}
	}
}