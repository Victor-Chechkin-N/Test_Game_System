using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace _Project
{
	public abstract class SettingsBase : ScriptableObject
	{
		[NonSerialized]
		private ControllerBase[] controllersForCreate;
		
		protected abstract void InitializeInherit();
		
		public void Initialize()
		{
			if (this.controllersForCreate == null)
			{
				// Creatable controllers.
				var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					.Where(f => f.IsDefined(typeof(AttributeSettingsController), false))
					.ToArray();
				
				this.controllersForCreate = new ControllerBase[fields.Length];
				
				for (int i = 0; i < fields.Length; i++)
				{
					controllersForCreate[i] = fields[i].GetValue(this) as ControllerBase;
				}
				
				this.InitializeInherit();
			}
		}
		
		private T GetController<T>() where T : ControllerBase
		{
			for (int i = 0; i < this.controllersForCreate.Length; i++)
			{
				if (this.controllersForCreate[i] is T controller)
				{
					return controller;
				}
			}
			
			return null;
		}
		
		public ControllerBase[] GetControllers()
		{
			return this.controllersForCreate;
		}
	}
}
