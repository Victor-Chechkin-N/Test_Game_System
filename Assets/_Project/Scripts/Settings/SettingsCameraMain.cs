using UnityEngine;

namespace _Project
{
	public class SettingsCameraMain : SettingsBase
	{
		[SerializeField]
		private SettingsFollowTarget settingsFollowTarget;
		
		protected override void InitializeInherit()
		{
			
		}
		
		public SettingsFollowTarget GetFollowTargetSettings()
		{
			return this.settingsFollowTarget;
		}
		
		[System.Serializable]
		public class SettingsFollowTarget
		{
			public float followTargetSpeed = 10f;
			public float offsetZ = 0.3f;
			public float offsetY = 8;
		}
	}
}