using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace _Project
{
	public abstract class ControllerBase : MonoBehaviour, IPoolObject
	{
		public event System.Action<ControllerBase> OnActionDestroy;
		
		[HideInInspector]
		public RectTransform rectTransform
		{
			get
			{
				return this.hashRectTransform ??= this.GetComponent<RectTransform>();
			}
		}
		
		private RectTransform hashRectTransform;
		
		/// <summary>
		/// All class events.
		/// </summary>
		private List<FieldInfo> eventFieldInfoList;
		
		private Transform defaultParent;
		
		private bool? isActiveUpdate;
		private bool? isActiveFixedUpdate;
		private bool? isActiveLateUpdate;
		
		private bool isEnabled;
		private bool isPaused;
		
		protected abstract void InitializeInherit();
		protected abstract void DestroyInherit();
		
		protected abstract void PauseInherit();
		protected abstract void ResumeInherit();
		
		public void Initialize()
		{
			if (!this.IfEnabled())
			{
				this.isEnabled = true;
				this.isPaused = false;
				
				this.gameObject.SetActive(true);
				
				this.InitializeInherit();
			}
		}
		
		public void Destroy()
		{
			if (!this.IfEnabled())
			{
				return;
			}
			
			this.DestroyInherit();
			
			this.OnActionDestroy?.Invoke(this);
			
			this.isEnabled = false;
			this.isPaused = false;
			
			this.ResetAllEvents();
			
			this.gameObject.SetActive(false);
			
			if (this.transform.parent != this.defaultParent)
			{
				this.transform.SetParent(this.defaultParent);
			}
		}
		
		public void Pause()
		{
			if (this.IfEnabled() && !this.IfPaused())
			{
				this.isPaused = true;
				
				this.PauseInherit();
			}
		}
		
		public void Resume()
		{
			if (this.IfEnabled() && this.IfPaused())
			{
				this.isPaused = false;
				
				this.ResumeInherit();
			}
		}
		
		public bool IfPaused()
		{
			return this.isPaused;
		}
		
		bool IPoolObject.PoolIfObjectActive()
		{
			return this.IfEnabled();
		}
		
		private bool IfEnabled()
		{
			return this.isEnabled;
		}
		
		public bool IfActiveUpdate(out IActiveUpdate instance)
		{
			var isActive = this.isActiveUpdate ??= this is IActiveUpdate;
			instance = isActive ? this as IActiveUpdate : null;
			return isActive;
		}
		
		public bool IfActiveFixedUpdate(out IActiveFixedUpdate instance)
		{
			var isActive = this.isActiveFixedUpdate ??= this is IActiveFixedUpdate;
			instance = isActive ? this as IActiveFixedUpdate : null;
			return isActive;
		}
		
		public bool IfActiveLateUpdate(out IActiveLateUpdate instance)
		{
			var isActive = this.isActiveLateUpdate ??= this is IActiveLateUpdate;
			instance = isActive ? this as IActiveLateUpdate : null;
			return isActive;
		}
		
		public void SetDefaultPartner(Transform partner)
		{
			this.defaultParent = partner;
		}
		
		#region SetParent
		public void SetParent(Transform parent, bool resetTransform = false, bool worldPositionStays = true)
		{
			this._SetParent(parent, this.transform, worldPositionStays, resetTransform);
		}
		
		private void _SetParent(Transform parent, Transform child, bool worldPositionStays, bool resetTransform)
		{
			child.SetParent(parent, worldPositionStays);
			
			if (resetTransform)
			{
				child.localPosition = Vector3.zero;
				child.localScale = Vector3.one;
				child.localRotation = Quaternion.identity;
			}
		}
		#endregion SetParent
		
		private void ResetAllEvents()
		{
			if (this.eventFieldInfoList == null)
			{
				this.eventFieldInfoList = new List<FieldInfo>();
				
				var thisType = this.GetType();
				var monoBehaviourType = typeof(MonoBehaviour);
				
				var eventInfos = this.GetType().GetEvents();
				var count1 = eventInfos.Length;
				for (int i = 0; i < count1; i++)
				{
					var eventInfo = eventInfos[i];
					
					System.Type t = thisType;
					FieldInfo field = null;
					while (t != monoBehaviourType)
					{
						field = t.GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic);
						
						if (field != null)
						{
							break;
						}
						
						t = t.BaseType;
					}
					
					this.eventFieldInfoList.Add(field);
				}
			}
			
			var count = this.eventFieldInfoList.Count;
			for (int i = 0; i < count; i++)
			{
				this.eventFieldInfoList[i].SetValue(this, null);
			}
		}
	}
}
