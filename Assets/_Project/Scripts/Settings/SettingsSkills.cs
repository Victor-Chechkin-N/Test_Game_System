using UnityEngine;

namespace _Project
{
	public class SettingsSkills : SettingsBase
	{
		[SerializeField, AttributeSettingsController]
		private ControllerBulletFire controllerBulletFire;
		[SerializeField, AttributeSettingsController]
		private ControllerBulletFireExplosion controllerBulletFireExplosion;
		
		[SerializeField, AttributeEditorArrayElementTitle("tSkills")]
		private SkillSettings[] skillSettings;
		
		protected override void InitializeInherit()
		{
			
		}
		
		public SkillSettings GetSkillSettings(TSkills tSkills)
		{
			for (int i = 0; i < this.skillSettings.Length; i++)
			{
				if (this.skillSettings[i].tSkills == tSkills)
				{
					return this.skillSettings[i];
				}
			}
			
			return null;
		}
		
		[System.Serializable]
		public class SkillSettings
		{
			public TSkills tSkills;
			public int damage = 20;
			public float moveSpeed = 1f;
			public float reloadTime = 2f;
		}
	}
}