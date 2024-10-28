using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public abstract class ManagerBase
	{
		public delegate SettingsBase FuncGetSettings(Type settingsType);
		public event FuncGetSettings OnFuncGetSettings;
		
		public delegate CanvasBase FuncGetCanvas(Type canvasType);
		public event FuncGetCanvas OnFuncGetCanvas;
		
		public delegate PanelBase FuncAddPanel(Type panelType);
		public event FuncAddPanel OnFuncAddPanel;
		
		private List<ControllerBase> controllerPrefabsList;
		
		private List<PoolContainerBase> poolsList;
		
		private IActiveUpdate thisActiveUpdate;
		private IActiveFixedUpdate thisActiveFixedUpdate;
		private IActiveLateUpdate thisActiveLateUpdate;
		
		private ControllersHandler controllersHandler;
		
		// Automatic creation of containers for controllers.
		private Transform containerForCreatedControllers;
		private Transform containerForCreatedControllersUi;
		
		private bool isReadyForDestroy;
		private bool isPaused;
		
		protected abstract void InitializeInherit();
		protected abstract void DestroyInherit();
		protected abstract void PauseInherit();
		protected abstract void ResumeInherit();
		
		public void Initialize()
		{
			this.controllerPrefabsList = new List<ControllerBase>();
			
			this.poolsList = new List<PoolContainerBase>();
			
			this.controllersHandler = new ControllersHandler();
			
			this.thisActiveUpdate = this as IActiveUpdate;
			this.thisActiveFixedUpdate = this as IActiveFixedUpdate;
			this.thisActiveLateUpdate = this as IActiveLateUpdate;
			
			this.InitializeInherit();
		}
		
		public void Destroy()
		{
			if (!this.isReadyForDestroy)
			{
				this.isReadyForDestroy = true;
				this.DestroyInherit();
			}
		}
		
		public void Pause()
		{
			if (!this.IfReadyForDestroy() && !this.IfPaused())
			{
				this.isPaused = true;
				
				this.controllersHandler.Pause();
				
				this.PauseInherit();
			}
		}
		
		public void Resume()
		{
			if (!this.IfReadyForDestroy() && this.IfPaused())
			{
				this.isPaused = false;
				
				this.controllersHandler.Resume();
				
				this.ResumeInherit();
			}
		}
		
		public bool IfPaused()
		{
			return this.isPaused;
		}
		
		public void Update()
		{
			if (this.thisActiveUpdate != null)
			{
				this.thisActiveUpdate.UpdateInherit();
			}
			
			this.controllersHandler.Update();
		}
		
		public void FixedUpdate()
		{
			if (this.thisActiveFixedUpdate != null)
			{
				this.thisActiveFixedUpdate.FixedUpdateInherit();
			}
			
			this.controllersHandler.FixedUpdate();
		}
		
		public void LateUpdate()
		{
			if (this.thisActiveLateUpdate != null)
			{
				this.thisActiveLateUpdate.LateUpdateInherit();
			}
			
			this.controllersHandler.LateUpdate();
		}
		
		public void Dispose()
		{
			
		}
		
		public bool IfReadyForDestroy()
		{
			return this.isReadyForDestroy;
		}
		
		protected void AddControllersForCreate(params SettingsBase[] settings)
		{
			for (var i = 0; i < settings.Length; i++)
			{
				this.controllerPrefabsList.AddRange(settings[i].GetControllers());
			}
		}
		
		protected T CreateController<T>(Vector3 localPosition) where T : ControllerBase, IPoolObject
		{
			return this.CreateController<T>(localPosition, null);
		}
		
		protected T CreateController<T>(Vector3 localPosition, Transform customParent, System.Action<T> OnActionNewInstanceCreated = null) where T : ControllerBase, IPoolObject
		{
			bool isJustCreated = false;
			
			T c = this.GetPoolObject<T>(
				OnActionNewInstanceCreated: delegate
				{
					isJustCreated = true;
				}
			);
			
			if (customParent != null)
			{
				if (c.rectTransform != null)
				{
					var sizeDelta = c.rectTransform.sizeDelta;
					c.SetParent(customParent, true);
					c.rectTransform.sizeDelta = sizeDelta;
				}
				else
				{
					c.SetParent(customParent, true);
				}
			}
			
			if (c.rectTransform != null)
			{
				c.rectTransform.anchoredPosition = localPosition;
			}
			else
			{
				c.transform.localPosition = localPosition;
			}
			
			this.controllersHandler.AddController(c);
			
			c.Initialize();
			
			if (isJustCreated)
			{
				OnActionNewInstanceCreated?.Invoke(c);
			}
			
			return c;
		}
		
		private T GetPoolObject<T>(System.Action<T> OnActionNewInstanceCreated = null) where T : ControllerBase, IPoolObject
		{
			var c = this.GetPoolContainer<T>() ?? this.CreatePoolContainer<T>();
			
			return c != null ? c.GetObject(OnActionNewInstanceCreated) : default(T);
		}
		
		private PoolContainer<T> GetPoolContainer<T>() where T : IPoolObject
		{
			var count = this.poolsList.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.poolsList[i] is PoolContainer<T> pool)
				{
					return (PoolContainer<T>) Convert.ChangeType(pool, typeof(PoolContainer<T>));
				}
			}
			
			return null;
		}
		
		private PoolContainer<T> CreatePoolContainer<T>() where T : ControllerBase, IPoolObject
		{
			var poolContainer = new PoolContainer<T>(
				OnFuncCreteObject: delegate
				{
					T prefab = this.GetPrefab<T>();
					
					// Dynamic creation of containers.
					Transform parent = null;
					if (prefab.rectTransform != null)
					{
						if (this.containerForCreatedControllersUi == null)
						{
							this.containerForCreatedControllersUi = new GameObject {name = this.GetType().Name + "Container"}.AddComponent<RectTransform>();
							this.containerForCreatedControllersUi.SetParent(this.GetCanvas<CanvasDefault>().rectTransform, false);
						}
						
						parent = this.containerForCreatedControllersUi;
					}
					else
					{
						if (this.containerForCreatedControllers == null)
						{
							this.containerForCreatedControllers = new GameObject {name = this.GetType().Name + "Container"}.transform;
						}
						
						parent = this.containerForCreatedControllers;
					}
					
					var c = prefab.transform.Instantiate(
						parent,
						new Vector3(),
						true
					).GetComponent<T>();
					
					c.SetDefaultPartner(parent);
					
					return c;
				}
			);
			
			this.poolsList.Add(poolContainer);
			
			return poolContainer;
		}
		
		private T GetPrefab<T>() where T : ControllerBase
		{
			for (int i = 0; i < this.controllerPrefabsList.Count; i++)
			{
				if (this.controllerPrefabsList[i] is T conroller)
				{
					return conroller;
				}
			}
			
			return null;
		}
		
		protected T GetSettings<T>() where T : SettingsBase
		{
			return this.OnFuncGetSettings.Invoke(typeof(T)) as T;
		}
		
		protected T GetCanvas<T>() where T : CanvasBase
		{
			return this.OnFuncGetCanvas.Invoke(typeof(T)) as T;
		}
		
		protected T AddPanel<T>() where T : PanelBase
		{
			return this.OnFuncAddPanel.Invoke(typeof(T)) as T;
		}
	}
}
