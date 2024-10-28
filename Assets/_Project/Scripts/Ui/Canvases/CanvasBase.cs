using UnityEngine;

namespace _Project
{
	public class CanvasBase : MonoBehaviour
	{
		[HideInInspector]
		public RectTransform rectTransform
		{
			get
			{
				return this.hashRectTransform ??= this.GetComponent<RectTransform>();
			}
		}
		
		private RectTransform hashRectTransform;
	}
}
