using UnityEngine;
using UnityEngine.Pool;

public class GameObjectFactory
{
	private GameObject _prefab;
	private Transform _container;

	private IObjectPool<GameObject> _objectPool;

	public GameObjectFactory(GameObject prefab, Transform container)
	{
		_prefab = prefab;
		_container = container;

		_objectPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy, false, 10, int.MaxValue);
	}

	private GameObject OnCreate()
	{
		return UnityEngine.Object.Instantiate(_prefab, _container);
	}

	private void OnGet(GameObject gameObject)
	{
		gameObject.SetActive(true);
	}

	private void OnRelease(GameObject gameObject)
	{
		gameObject.SetActive(false);
	}

	private void OnDestroy(GameObject gameObject)
	{
		UnityEngine.Object.Destroy(gameObject);
	}

	public GameObject Instantiate(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject gameObject = _objectPool.Get();

		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = scale;

		return gameObject;
	}
	public void Dispose(GameObject gameObject)
	{
		_objectPool.Release(gameObject);
	}
}