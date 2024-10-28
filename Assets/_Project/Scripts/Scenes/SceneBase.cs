using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public abstract class SceneBase : MonoBehaviour
	{
		[Header("SceneBase:")]
		[SerializeField]
		private Transform[] canvasesContainers;
		
		private List<CanvasBase> canvasesList;
		private List<PanelBase> panels;
		private List<ManagerBase> createdManagersList;
		private List<SettingsBase> createdSettingsList;
		
		private IActiveUpdate thisActiveUpdate;
		private IActiveFixedUpdate thisActiveFixedUpdate;
		private IActiveLateUpdate thisActiveLateUpdate;
		
		private ControllersHandler controllersHandler;
		
		protected abstract void InitializeInherit();
		protected abstract void DestroyInherit();
		
		private void Awake()
		{
			this.panels = new List<PanelBase>();
			this.createdManagersList = new List<ManagerBase>();
			this.createdSettingsList = new List<SettingsBase>();
			
			this.controllersHandler = new ControllersHandler();
			
			this.thisActiveUpdate = this as IActiveUpdate;
			this.thisActiveFixedUpdate = this as IActiveFixedUpdate;
			this.thisActiveLateUpdate = this as IActiveLateUpdate;
			
			this.canvasesList = new List<CanvasBase>();
			for (int i = 0; i < this.canvasesContainers.Length; i++)
			{
				CanvasBase[] canvasBase = this.canvasesContainers[i].GetComponentsInChildren<CanvasBase>(false);
				for (int j = 0; j < canvasBase.Length; j++)
				{
					this.canvasesList.Add(canvasBase[j]);
				}
				
				PanelBase[] panelBase = this.canvasesContainers[i].GetComponentsInChildren<PanelBase>(false);
				for (int j = 0; j < panelBase.Length; j++)
				{
					this.PreparePanel(panelBase[j]);
				}
			}
			
			this.FindIndependentControllers();
			
			this.InitializeInherit();
		}
		
		private void Update()
		{
			if (this.thisActiveUpdate != null)
			{
				this.thisActiveUpdate.UpdateInherit();
			}
			
			for (int i = this.createdManagersList.Count - 1; i > -1; i--)
			{
				var manager = this.createdManagersList[i];
				if (manager.IfReadyForDestroy())
				{
					manager.Destroy();
					this.createdManagersList.RemoveAt(i);
				}
				else
				{
					if (!manager.IfPaused())
					{
						manager.Update();
					}
				}
			}
			
			this.controllersHandler.Update();
		}
		
		private void FixedUpdate()
		{
			if (this.thisActiveFixedUpdate != null)
			{
				this.thisActiveFixedUpdate.FixedUpdateInherit();
			}
			
			for (int i = this.createdManagersList.Count - 1; i > -1; i--)
			{
				var manager = this.createdManagersList[i];
				if (!manager.IfReadyForDestroy() && !manager.IfPaused())
				{
					manager.FixedUpdate();
				}
			}
			
			this.controllersHandler.FixedUpdate();
		}
		
		private void LateUpdate()
		{
			if (this.thisActiveLateUpdate != null)
			{
				this.thisActiveLateUpdate.LateUpdateInherit();
			}
			
			for (int i = this.createdManagersList.Count - 1; i > -1; i--)
			{
				var manager = this.createdManagersList[i];
				if (!manager.IfReadyForDestroy() && !manager.IfPaused())
				{
					manager.LateUpdate();
				}
			}
			
			this.controllersHandler.LateUpdate();
		}
		
		private void OnDestroy()
		{
			this.DestroyInherit();
			
			foreach (var manager in this.createdManagersList)
			{
				manager.Destroy();
			}
			
			this.createdManagersList = new List<ManagerBase>();
			
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
		
		public virtual void Pause()
		{
			
		}
		
		public virtual void Resume()
		{
			
		}
		
		public bool IfPaused()
		{
			return false;
		}
		
		private void FindIndependentControllers()
		{
			var objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			for (int i = objects.Length - 1; i > -1; i--)
			{
				var controllers = objects[i].transform.FindChildrenWithComponentOnlyTop<ControllerBase>();
				for (int j = controllers.Count - 1; j > -1; j--)
				{
					var c = controllers[j];
					
					this.controllersHandler.AddController(c);
					
					c.Initialize();
				}
			}
		}
		
		protected T GetManager<T>() where T : ManagerBase
		{
			// Search among created.
			foreach (var manager in this.createdManagersList)
			{
				if (!manager.IfReadyForDestroy() && manager is T)
				{
					return (T) Convert.ChangeType(manager, typeof(T));
				}
			}
			
			// Create a new one.
			var instance = (ManagerBase) Activator.CreateInstance(typeof(T));
			instance.OnFuncGetSettings += this.GetSettings;
			instance.OnFuncGetCanvas += this.GetCanvas;
			instance.OnFuncAddPanel += panelType => this.AddPanel(panelType);
			
			instance.Initialize();
			this.createdManagersList.Add(instance);
			
			return (T) instance;
		}
		
		protected T GetSettings<T>() where T : SettingsBase
		{
			return this.GetSettings(typeof(T)) as T;
		}
		
		private SettingsBase GetSettings(Type settingsType)
		{
			// Search among created.
			var count = this.createdSettingsList.Count;
			for (int i = 0; i < count; i++)
			{
				if (settingsType.IsInstanceOfType(this.createdSettingsList[i]))
				{
					return this.createdSettingsList[i];
				}
			}
			
			var instance = Resources.Load<SettingsBase>("Settings/" + settingsType.Name);
			
			this.createdSettingsList.Add(instance);
			instance.Initialize();
			
			return instance;
		}
		
		protected T GetCanvas<T>() where T : CanvasBase
		{
			return this.GetCanvas(typeof(T)) as T;
		}
		
		private CanvasBase GetCanvas(Type canvasType)
		{
			var count = this.canvasesList.Count;
			for (int i = 0; i < count; i++)
			{
				if (canvasType.IsInstanceOfType(this.canvasesList[i]))
				{
					return this.canvasesList[i];
				}
			}
			
			return null;
		}
		
		private PanelBase AddPanel(Type panelType, Vector3 position = default(Vector3))
		{
			return this.AddPanel(this.GetCanvas<CanvasDefault>(), this.GetLinkPanel(panelType).GetPanel(), position);
		}
		
		protected T AddPanel<T>(Vector3 position = default(Vector3)) where T : PanelBase
		{
			return this.AddPanel<T>(this.GetCanvas<CanvasDefault>(), (T) this.GetLinkPanel<T>().GetPanel(), position);
		}
		
		protected T AddPanel<T>(CanvasBase canvas, Vector3 position = default(Vector3)) where T : PanelBase
		{
			return this.AddPanel<T>(canvas, (T) this.GetLinkPanel<T>().GetPanel(), position);
		}
		
		private T AddPanel<T>(CanvasBase canvas, T panelPrefab, Vector3 position = default(Vector3)) where T : PanelBase
		{
			var newPanel = this._AddPanel(canvas, panelPrefab, position);
			return newPanel.GetComponent<T>();
		}
		
		private PanelBase _AddPanel(CanvasBase canvas, PanelBase panelPrefab, Vector3 position = default(Vector3))
		{
			RectTransform prefabRt = panelPrefab.GetComponent<RectTransform>();
			if (position == default(Vector3))
			{
				position = new Vector3();
			}
			
			var clone = prefabRt.Instantiate(
				canvas.rectTransform,
				new Vector3(),
				true
			);
			
			PanelBase newPanel = clone.GetComponent<PanelBase>();
			
			RectTransform rT = newPanel.rectTransform;
			rT.sizeDelta = prefabRt.sizeDelta;
			rT.anchoredPosition3D = position;
			
			this.PreparePanel(newPanel);
			
			return newPanel;
		}
		
		private void PreparePanel(PanelBase panel)
		{
			panel.OnActionDestroy += delegate
			{
				this.panels.Remove(panel);
			};
			
			this.panels.Add(panel);
			panel.Initialize();
		}
		
		private LinkPanel GetLinkPanel<T>() where T : PanelBase
		{
			return this.GetLinkPanel(typeof(T));
		}
		
		private LinkPanel GetLinkPanel(Type panelType)
		{
			string path = "LinkPanels/Link" + panelType.Name;
			
			// We don't add the LinkPanels to an array, because links for panels are not tracked.
			// Then the garbage collector itself will clear the memory after deleting the panel.
			LinkPanel linkPanel = Resources.Load<LinkPanel>(path);
			if (linkPanel == null)
			{
				Debug.LogWarning("GetLinkPanel. FILE ABSENT, " + ("filePath: " + path) + ("  type: " + panelType) + "\r\n");
				
				return null;
			}
			
			return linkPanel;
		}
	}
}