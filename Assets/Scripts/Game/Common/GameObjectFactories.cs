using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactories
{
	private Transform _container;

	private Dictionary<GameObject, GameObjectFactory> _factories;

	public GameObjectFactories(Transform container)
	{
		_container = container;

		_factories = new Dictionary<GameObject, GameObjectFactory>();
	}

	public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		return GetOrCreateFactory(prefab).Instantiate(position, rotation, scale);
	}
	public void Dispose(GameObject prefab, GameObject gameObject)
	{
		GetOrCreateFactory(prefab).Dispose(gameObject);
	}

	private GameObjectFactory GetOrCreateFactory(GameObject prefab)
	{
		if (_factories.TryGetValue(prefab, out GameObjectFactory factory))
		{
			return factory;
		}
		else
		{
			_factories.Add(prefab, new GameObjectFactory(prefab, _container));

			return _factories[prefab];
		}
	}
}