using UnityEngine;

namespace _Project
{
	public class ControllerDistanceViewWave : ControllerBase, IActiveUpdate
	{
		[SerializeField]
		private LineRenderer lineRenderer;
		
		private SettingsSkillDistanceView.WaveSettings waveSettings;
		
		private Transform followTarget;
		
		private int detectionAngle;
		private float radius;
		
		private float currentRadius;
		
		protected override void InitializeInherit()
		{
			this.currentRadius = 0;
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		protected override void PauseInherit()
		{
			
		}
		
		protected override void ResumeInherit()
		{
			
		}
		
		void IActiveUpdate.UpdateInherit()
		{
			this.Expand(this.waveSettings.moveSpeed, this.radius);
		}
		
		public void SetSettings(SettingsSkillDistanceView.WaveSettings waveSettings, float radius, int detectionAngle)
		{
			this.waveSettings = waveSettings;
			this.radius = radius;
			this.detectionAngle = detectionAngle;
		}
		
		public void SetFollowTarget(Transform followTarget)
		{
			this.followTarget = followTarget;
		}
		
		private void Expand(float speed, float maxRadius)
		{
			this.currentRadius += speed * Time.deltaTime;
			
			if (this.currentRadius > maxRadius)
			{
				this.Destroy();
				return;
			}
			
			
			float alpha = Mathf.InverseLerp(maxRadius, 0, this.currentRadius);
			this.SetLineRendererAlpha(alpha);
			
			this.DrawCircle(this.currentRadius);
		}
		
		private void DrawCircle(float radius)
		{
			int segments = this.waveSettings.segments;
			this.lineRenderer.positionCount = segments + 1;
			
			Quaternion rotation = Quaternion.Euler(0, followTarget.eulerAngles.y, 0);
			
			for (int i = 0; i <= segments; i++)
			{
				float angle = Mathf.Lerp(-this.detectionAngle, this.detectionAngle, (float) i / segments);
				float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
				float z = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
				
				Vector3 point = rotation * new Vector3(x, 0, z);
				this.lineRenderer.SetPosition(i, point + this.followTarget.position);
			}
		}
		
		private void SetLineRendererAlpha(float alpha)
		{
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
				new GradientAlphaKey[] { 
					new GradientAlphaKey(0.0f, 0.0f),
					new GradientAlphaKey(alpha, 0.2f),
					new GradientAlphaKey(alpha, 0.7f),
					new GradientAlphaKey(0.0f, 1.0f)
				}
			);
			
			this.lineRenderer.colorGradient = gradient;
		}
	}
}
