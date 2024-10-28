using UnityEngine;

namespace _Project
{
	public interface ICameraRotate 
	{
		public bool ProcessRotation(Quaternion cameraCurrentRotation, out float newAngle);
	}
}