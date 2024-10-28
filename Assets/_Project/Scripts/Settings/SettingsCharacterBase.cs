using UnityEngine;

namespace _Project
{
	public abstract class SettingsCharacterBase : SettingsBase
	{
		[SerializeField]
		private MoveSettings moveSettings;
		
		[SerializeField]
		private SkillUseRadiusSettings skillUseRadiusSettings;
		
		[SerializeField]
		private FightModeSettings fightModeSettings;
		
		[SerializeField]
		private TSkills[] skills;
		
		[Header("Materials:")]
		[SerializeField, AttributeEditorArrayElementTitle("tTeam")]
		private MaterialContainer[] materialContainers;
		
		protected override void InitializeInherit()
		{
			
		}
		
		public MoveSettings GetMoveSettings()
		{
			return this.moveSettings;
		}
		
		public SkillUseRadiusSettings GetSkillUseRadiusSettings()
		{
			return this.skillUseRadiusSettings;
		}
		
		public FightModeSettings GetFightModeSettings()
		{
			return this.fightModeSettings;
		}
		
		public TSkills[] GetSkills()
		{
			return this.skills;
		}
		
		public MaterialContainer GetMaterialContainer(TTeam tTeam)
		{
			for (int i = 0; i < this.materialContainers.Length; i++)
			{
				if (this.materialContainers[i].tTeam == tTeam)
				{
					return this.materialContainers[i];
				}
			}
			
			return this.materialContainers[0];
		}
		
		[System.Serializable]
		public class MoveSettings
		{
			public float blendSpeed = 10;
			public float angularSpeed = 220;
			public float acceleration = 30;
			public float moveSpeedDefault = 4.5f;
			public float moveSpeedFightMode = 3.5f;
		}
		
		[System.Serializable]
		public class SkillUseRadiusSettings
		{
			public float radius = 8;
			public int detectionAngle = 30;
		}
		
		[System.Serializable]
		public class FightModeSettings
		{
			public float enemiesInRadius = 15;
		}
		
		[System.Serializable]
		public class MaterialContainer
		{
			public TTeam tTeam;
			public Material[] materialDefault;
			public Material[] materialTarget;
		}
	}
}