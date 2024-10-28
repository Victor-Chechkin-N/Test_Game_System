using UnityEngine;

namespace _Project
{
	public abstract class ControllerBulletBase : ControllerBase, IActiveUpdate
	{
		public delegate void ActionCharacterCollide(ControllerBulletBase bullet, ControllerCharacterBase character, Vector3 hitPosition, Collider hitCollider);
		public event ActionCharacterCollide OnActionCharacterCollide;
		
		private Vector3 targetPosition;
		private float arcHeight;
		
		private float xOffset;
		private float zOffset;
		
		private Vector3 startPosition;
		private float timeElapsed;
		
		private ControllerCharacterBase owner;
		private ControllerCharacterBase target;
		
		private SettingsSkills.SkillSettings skillSettings;
		
		private TTeam tTeam;
		
		protected override void InitializeInherit()
		{
			this.startPosition = this.transform.position;
			this.timeElapsed = 0f;
			
			this.arcHeight = Random.Range(-5f, 5f);
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
			this.timeElapsed += Time.deltaTime;
			
			var t = this.timeElapsed / this.skillSettings.moveSpeed;
			
			if (t >= 1f)
			{
				this.Destroy();
				return;
			}
			
			var horizontalPosition = Vector3.Lerp(this.startPosition, this.targetPosition, t);
			
			var offset = Mathf.Sin(t * Mathf.PI) * this.arcHeight;
			this.transform.position = new Vector3(horizontalPosition.x + offset * this.xOffset, horizontalPosition.y, horizontalPosition.z + offset * this.zOffset);
		}
		
		protected virtual void OnTriggerEnter(Collider other)
		{
			ControllerCharacterBase character = other.transform.GetComponentInParent<ControllerCharacterBase>();
			if (null != character && character != this.owner)
			{
				Vector3 hitPosition = other.ClosestPoint(transform.position);
				this.OnActionCharacterCollide?.Invoke(this, character, hitPosition, other);
			}
		}
		
		public void SetSettings(SettingsSkills.SkillSettings skillSettings)
		{
			this.skillSettings = skillSettings;
		}
		
		public void SetTTeam(TTeam value)
		{
			this.tTeam = value;
		}
		
		public TTeam GetTTeam()
		{
			return this.tTeam;
		}
		
		public int GetDamage()
		{
			return this.skillSettings.damage;
		}
		
		public void SetTarget(ControllerCharacterBase owner, ControllerCharacterBase target, Vector3 targetPosition)
		{
			this.owner = owner;
			this.target = target;
			
			this.targetPosition = targetPosition;
			
			var angleY = this.owner.transform.eulerAngles.y;
			var angleInRadians = angleY * Mathf.Deg2Rad;
			
			this.xOffset = Mathf.Cos(angleInRadians);
			this.zOffset = Mathf.Sin(angleInRadians);
		}
	}
}
