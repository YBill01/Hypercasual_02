using UnityEngine;
using UnityEngine.Pool;

internal class UIScreenFactory
{
	private UIScreen _prefab;
	private RectTransform _container;

	private IObjectPool<UIScreen> _objectPool;

	internal UIScreenFactory(UIScreen prefab, RectTransform container)
	{
		_prefab = prefab;
		_container = container;

		_objectPool = new ObjectPool<UIScreen>(CreatePooledItem, actionOnDestroy : OnDestroyPoolObject);
	}

	private UIScreen CreatePooledItem()
	{
		return Object.Instantiate(_prefab, _container);
	}
	private void OnDestroyPoolObject(UIScreen screen)
	{
		Object.Destroy(screen.gameObject);
	}

	internal UIScreen Instantiate()
	{
		return _objectPool.Get();
	}
	internal void Dispose(UIScreen screen)
	{
		_objectPool.Release(screen);
	}

	internal void Destroy()
	{
		_objectPool.Clear();
	}
}