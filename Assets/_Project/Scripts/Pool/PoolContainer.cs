using System;
using System.Collections.Generic;

namespace _Project
{
	public class PoolContainer<T> : PoolContainerBase where T : IPoolObject
	{
		public PoolContainer(Func<IPoolObject> OnFuncCreteObject) : base(OnFuncCreteObject) {
			
		}
		
		public T GetObject(System.Action<T> OnActionNewInstanceCreated = null)
		{
			T _object = default(T);
			var count = this.poolObjects.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.poolObjects[i].PoolIfObjectActive())
				{
					_object = (T) this.poolObjects[i];
					break;
				}
			}
			
			if (_object == null)
			{
				IPoolObject newObject = this.CreateObject();
				if (newObject == null)
				{
					return default(T);
				}
				
				this.poolObjects.Add(newObject);
				_object = (T) newObject;
				
				OnActionNewInstanceCreated?.Invoke(_object);
			}
			
			return _object;
		}
	}
}
