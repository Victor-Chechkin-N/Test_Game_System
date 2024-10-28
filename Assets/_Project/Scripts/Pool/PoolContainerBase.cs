using System.Collections.Generic;

namespace _Project
{
	public abstract class PoolContainerBase
	{
		protected readonly List<IPoolObject> poolObjects;
		private event System.Func<IPoolObject> OnFuncCreteObject;
		
		public PoolContainerBase(System.Func<IPoolObject> OnFuncCreteObject)
		{
			this.poolObjects = new List<IPoolObject>();
			this.OnFuncCreteObject = OnFuncCreteObject;
		}
		
		public List<IPoolObject> GetAllPoolObjects()
		{
			return this.poolObjects;
		}
		
		protected IPoolObject CreateObject()
		{
			return this.OnFuncCreteObject?.Invoke();
		}
	}
}
