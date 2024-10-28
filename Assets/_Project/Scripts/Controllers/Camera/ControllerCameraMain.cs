using UnityEngine;

namespace _Project
{
	public class ControllerCameraMain : ControllerBase, IActiveLateUpdate
	{
		[SerializeField]
		private new Camera camera;
		
		private ICameraRotate cameraRotateComponent;
		
		private SettingsCameraMain.SettingsFollowTarget settingsFollowTarget;
		
		private Transform followTransform;
		
		protected override void InitializeInherit()
		{
			#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
				this.cameraRotateComponent = new CameraRotateComponentMobile();
			#else
				this.cameraRotateComponent = new CameraRotateComponentPc();
			#endif
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
		
		void IActiveLateUpdate.LateUpdateInherit()
		{
			if (this.followTransform != null)
			{
				Vector3 forwardInverseNormal = -this.transform.forward;
				
				this.transform.position = Vector3.Lerp(
					this.transform.position,
					this.followTransform.position
						+ new Vector3(forwardInverseNormal.x * this.settingsFollowTarget.offsetZ, forwardInverseNormal.y * this.settingsFollowTarget.offsetY, forwardInverseNormal.z * this.settingsFollowTarget.offsetZ),
					Time.deltaTime * this.settingsFollowTarget.followTargetSpeed
				);
			}
			
			var isAngleChanged = this.cameraRotateComponent.ProcessRotation(this.transform.rotation, out float newAngle);
			if (isAngleChanged)
			{
				float rotateToTargetSpeed = 200.0f;
				var eulerAngles = this.transform.rotation.eulerAngles;
				this.transform.rotation = Quaternion.RotateTowards(
					this.transform.rotation,
					Quaternion.Euler(eulerAngles.x, newAngle, eulerAngles.z),
					Time.deltaTime * rotateToTargetSpeed
				);
			}
		}
		
		public void SetSettingsCameraMain(SettingsCameraMain settingsCameraMain)
		{
			this.settingsFollowTarget = settingsCameraMain.GetFollowTargetSettings();
		}
		
		public void SetFollowTransform(Transform followTransform)
		{
			this.followTransform = followTransform;
		}
		
		public Camera GetCamera()
		{
			return this.camera;
		}
	}
}
