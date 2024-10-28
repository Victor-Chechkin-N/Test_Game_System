using System;
using UnityEngine;

public static class Vector3Extensions
{
	
	public static bool IfCloserToTargetThan(this Vector3 _this, Vector3 target, float distance)
	{
		return (_this - target).sqrMagnitude < distance * distance;
	}
	
	public static Vector3 ModifyReplace(this Vector3 _this, float? x = null, float? y = null, float? z = null)
	{
		if (x.HasValue)
		{
			_this.x = x.Value;
		}
		
		if (y.HasValue)
		{
			_this.y = y.Value;
		}
		
		if (z.HasValue)
		{
			_this.z = z.Value;
		}
		
		return _this;
	}
	
	public static Vector3 ModifyAddValues(this Vector3 _this, Vector3 vector)
	{
		return _this.ModifyAddValues(vector.x, vector.y, vector.z);
	}
	
	public static Vector3 ModifyAddValues(this Vector3 _this, float x, float y, float z)
	{
		_this.x += x;
		_this.y += y;
		_this.z += z;
		
		return _this;
	}
	
	public static Vector3 ModifyAngle(this Vector3 _this, float angle, Vector3 axis)
	{
		return Quaternion.AngleAxis(angle, axis) * _this;
	}
}