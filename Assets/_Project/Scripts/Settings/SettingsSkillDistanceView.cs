using UnityEngine;

namespace _Project
{
	public class SettingsSkillDistanceView : SettingsBase
	{
		[SerializeField, AttributeSettingsController]
		private ControllerDistanceViewWave controllerDistanceViewWave;
		
		[SerializeField]
		private WaveSettings waveSettings;
		
		protected override void InitializeInherit()
		{
			
		}
		
		public WaveSettings GetWaveSettings()
		{
			return this.waveSettings;
		}
		
		[System.Serializable]
		public class WaveSettings
		{
			public float moveSpeed = 1f;
			public float createWaveDelay = 1f;
			public int segments = 10;
		}
	}
}