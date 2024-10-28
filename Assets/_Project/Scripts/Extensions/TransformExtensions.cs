using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	/// <summary>
	/// Find descendants of a component.
	/// Find only top objects and do not search among their descendants.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <returns></returns>
	public static List<T> FindChildrenWithComponentOnlyTop<T>(this Transform target) where T : MonoBehaviour
	{
		List<T> children = new List<T>();
		TransformExtensions.FindChildrenWithComponentOnlyTop(children, target);
		
		return children;
	}
	
	private static void FindChildrenWithComponentOnlyTop<T>(List<T> children, Transform target) where T : MonoBehaviour
	{
		var c = target.GetComponent<T>();
		if (c != null)
		{
			children.Add(c);
			return;
		}
		
		for (int i = target.childCount - 1; i > -1; i--)
		{
			TransformExtensions.FindChildrenWithComponentOnlyTop(children, target.GetChild(i));
		}
	}
	
	public static Transform Instantiate(this Transform target, Transform parent, Vector3 position, bool isLocalPosition)
	{
		Transform clone = isLocalPosition
			? Object.Instantiate(target) as Transform
			: Object.Instantiate(target, position, Quaternion.identity) as Transform;
		if (null == clone)
		{
			return null;
		}
		
		if (null != parent)
		{
			clone.SetParent(parent, false);
		}
		
		if (isLocalPosition)
		{
			clone.localPosition = position;
		}
		
		return clone;
	}
}