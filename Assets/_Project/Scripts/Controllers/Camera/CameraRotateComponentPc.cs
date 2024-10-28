using UnityEngine;

namespace _Project
{
	public class CameraRotateComponentPc : ICameraRotate
	{
		private float slidingWithPointerSpeed = 360f;
		
		private bool isSlidingEnabled;
		private bool isSlidingActivated;
		private bool isSlidingStarted;
		private Vector3 mouseDownPosition;
		
		private Quaternion slidingStartedQuaternion = Quaternion.identity;
		private Quaternion slidingQuaternion = Quaternion.identity;
		
		// How many pixels do the finger or mouse need to move for the slider to start scrolling?
		private int startSlidingAfter = 4;
		
		bool ICameraRotate.ProcessRotation(Quaternion cameraCurrentRotation, out float newAngle)
		{
			newAngle = 0;
			bool isAngleChanged = false;
			
			if (Input.GetMouseButtonDown(0))
			{
				this.mouseDownPosition = Input.mousePosition;
				
				this.slidingStartedQuaternion = cameraCurrentRotation;
				
				this.slidingQuaternion = this.slidingStartedQuaternion;
				
				this.isSlidingActivated = true;
			}
			
			if (this.isSlidingActivated && !Input.GetMouseButton(0))
			{
				this.isSlidingActivated = false;
				this.slidingQuaternion = Quaternion.identity;
				
				if (this.isSlidingStarted)
				{
					this.StopSliding();
				}
			}
			
			if (this.isSlidingActivated)
			{
				if (Mathf.Abs(mouseDownPosition.x - Input.mousePosition.x) > startSlidingAfter)
				{
					Vector3 move = Camera.main.ScreenToViewportPoint(mouseDownPosition - Input.mousePosition);
					if (!this.isSlidingStarted)
					{
						this.StartSliding();
					}
					
					newAngle = this.slidingStartedQuaternion.eulerAngles.y + move.x * this.slidingWithPointerSpeed;
					isAngleChanged = true;
				}
			}
			
			return isAngleChanged;
		}
		
		private void StartSliding()
		{
			this.isSlidingStarted = true;
		}
		
		private void StopSliding()
		{
			this.isSlidingStarted = false;
		}
	}
}
