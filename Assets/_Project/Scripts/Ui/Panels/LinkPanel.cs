using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public class LinkPanel : ScriptableObject
	{
		[SerializeField]
		private PanelBase panel;
		
		public PanelBase GetPanel()
		{
			return this.panel;
		}
	}
}
