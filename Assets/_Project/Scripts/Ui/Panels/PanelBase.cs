using UnityEngine;

namespace _Project
{
	public abstract class PanelBase : MonoBehaviour
	{
		public event System.Action<PanelBase> OnActionDestroy;
		
		[HideInInspector]
		public RectTransform rectTransform
		{
			get
			{
				return this.hashRectTransform ??= this.GetComponent<RectTransform>();
			}
		}
		
		private RectTransform hashRectTransform;
		
		private IActiveUpdate thisActiveUpdate;
		private IActiveFixedUpdate thisActiveFixedUpdate;
		private IActiveLateUpdate thisActiveLateUpdate;
		
		private ControllersHandler controllersHandler;
		
		private bool isPaused;
		
		protected abstract void InitializeInherit();
		protected abstract void DestroyInherit();
		
		protected abstract void PauseInherit();
		protected abstract void ResumeInherit();
		
		public void Initialize()
		{
			this.controllersHandler = new ControllersHandler();
			
			this.thisActiveUpdate = this as IActiveUpdate;
			this.thisActiveFixedUpdate = this as IActiveFixedUpdate;
			this.thisActiveLateUpdate = this as IActiveLateUpdate;
			
			// Looking for nested controllers.
			this.FindInnerControllers();
			
			this.InitializeInherit();
		}
		
		public void Destroy()
		{
			this.DestroyInherit();
			
			this.OnActionDestroy?.Invoke(this);
		}
		
		public void Pause()
		{
			if (!this.IfPaused())
			{
				this.isPaused = true;
				
				this.controllersHandler.Pause();
				
				this.PauseInherit();
			}
		}
		
		public void Resume()
		{
			if (this.IfPaused())
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
		
		private void FindInnerControllers()
		{
			var controllers = this.transform.FindChildrenWithComponentOnlyTop<ControllerBase>();
			for (int j = controllers.Count - 1; j > -1; j--)
			{
				var c = controllers[j];
				
				this.controllersHandler.AddController(c);
				
				c.Initialize();
			}
		}
	}
}
