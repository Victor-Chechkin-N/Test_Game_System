using UnityEngine;

namespace _Project
{
	public class SettingsCharacterSticky : SettingsCharacterBase
	{
		[Header("SettingsCharacterSticky:")]
		[SerializeField, AttributeSettingsController]
		private ControllerCharacterSticky controllerCharacterSticky;
		
		protected override void InitializeInherit()
		{
			
		}
	}
}