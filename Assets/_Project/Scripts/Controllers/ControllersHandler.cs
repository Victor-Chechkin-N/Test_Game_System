using System.Collections.Generic;

namespace _Project
{
	public class ControllersHandler
	{
		private List<IActiveUpdate> controllersActiveUpdate;
		private List<IActiveFixedUpdate> controllerActiveFixedUpdate;
		private List<IActiveLateUpdate> controllerActiveLateUpdate;
		
		public ControllersHandler()
		{
			this.controllersActiveUpdate = new List<IActiveUpdate>();
			this.controllerActiveFixedUpdate = new List<IActiveFixedUpdate>();
			this.controllerActiveLateUpdate = new List<IActiveLateUpdate>();
		}
			
		public void AddController(ControllerBase c)
		{
			if (c.IfActiveUpdate(out IActiveUpdate instanceUpdate))
			{
				this.controllersActiveUpdate.Add(instanceUpdate);
			}
			
			if (c.IfActiveFixedUpdate(out IActiveFixedUpdate instanceFixedUpdate))
			{
				this.controllerActiveFixedUpdate.Add(instanceFixedUpdate);
			}
			
			if (c.IfActiveLateUpdate(out IActiveLateUpdate instanceLateUpdate))
			{
				this.controllerActiveLateUpdate.Add(instanceLateUpdate);
			}
			
			c.OnActionDestroy += delegate
			{
				if (c.IfActiveUpdate(out IActiveUpdate instanceUpdate))
				{
					this.controllersActiveUpdate.Remove(instanceUpdate);
				}
				
				if (c.IfActiveFixedUpdate(out IActiveFixedUpdate instanceFixedUpdate))
				{
					this.controllerActiveFixedUpdate.Remove(instanceFixedUpdate);
				}
				
				if (c.IfActiveLateUpdate(out IActiveLateUpdate instanceLateUpdate))
				{
					this.controllerActiveLateUpdate.Remove(instanceLateUpdate);
				}
			};
		}
		
		public void Pause()
		{
			for (int i = this.controllersActiveUpdate.Count - 1; i >= 0; i--)
			{
				this.controllersActiveUpdate[i].Pause();
			}
			
			for (int i = this.controllerActiveFixedUpdate.Count - 1; i >= 0; i--)
			{
				this.controllerActiveFixedUpdate[i].Pause();
			}
			
			for (int i = this.controllerActiveLateUpdate.Count - 1; i >= 0; i--)
			{
				this.controllerActiveLateUpdate[i].Pause();
			}
		}
		
		public void Resume()
		{
			for (int i = this.controllersActiveUpdate.Count - 1; i >= 0; i--)
			{
				this.controllersActiveUpdate[i].Resume();
			}
			
			for (int i = this.controllerActiveFixedUpdate.Count - 1; i >= 0; i--)
			{
				this.controllerActiveFixedUpdate[i].Resume();
			}
			
			for (int i = this.controllerActiveLateUpdate.Count - 1; i >= 0; i--)
			{
				this.controllerActiveLateUpdate[i].Resume();
			}
		}
		
		public void Update()
		{
			for (int i = this.controllersActiveUpdate.Count - 1; i >= 0; i--)
			{
				var controller = this.controllersActiveUpdate[i];
				if (!controller.IfPaused())
				{
					controller.UpdateInherit();
				}
			}
		}
		
		public void FixedUpdate()
		{
			for (int i = this.controllerActiveFixedUpdate.Count - 1; i >= 0; i--)
			{
				var controller = this.controllerActiveFixedUpdate[i];
				if (!controller.IfPaused())
				{
					controller.FixedUpdateInherit();
				}
			}
		}
		
		public void LateUpdate()
		{
			for (int i = this.controllerActiveLateUpdate.Count - 1; i >= 0; i--)
			{
				var controller = this.controllerActiveLateUpdate[i];
				if (!controller.IfPaused())
				{
					controller.LateUpdateInherit();
				}
			}
		}
	}
}
