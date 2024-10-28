using UnityEngine;

namespace _Project
{
	public class CameraRotateComponentMobile : ICameraRotate
	{
		private float slidingWithPointerSpeed = 15f;
		
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
			
			if (1 == Input.touchCount)
			{
				Touch touchZero = Input.GetTouch(0);
				
				if (!this.isSlidingActivated)
				{
					this.mouseDownPosition = touchZero.position;
					
					this.slidingStartedQuaternion = cameraCurrentRotation;
					
					this.slidingQuaternion = this.slidingStartedQuaternion;
					
					this.isSlidingActivated = true;
				}
				
				if (Mathf.Abs(mouseDownPosition.x - touchZero.position.x) > startSlidingAfter)
				{
					Vector3 move = Camera.main.ScreenToViewportPoint(new Vector2(mouseDownPosition.x, mouseDownPosition.y) - touchZero.position);
					if (!this.isSlidingStarted)
					{
						this.StartSliding();
					}
					
					newAngle = this.slidingStartedQuaternion.eulerAngles.y + move.x * this.slidingWithPointerSpeed;
					isAngleChanged = true;
				}
			}
			else if (this.isSlidingActivated)
			{
				this.isSlidingActivated = false;
				this.slidingQuaternion = Quaternion.identity;
				
				if (this.isSlidingStarted)
				{
					this.StopSliding();
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
