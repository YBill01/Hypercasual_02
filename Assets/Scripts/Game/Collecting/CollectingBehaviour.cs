using GameName.Data;
using System.Collections.Generic;
using UnityEngine;

public class CollectingBehaviour : IUpdatable
{
	private Transform _container;

	private GameObjectFactories _factories;
	private List<ICollectingHandle> _handles;

	public CollectingBehaviour(Transform container)
	{
		_container = container;

		_factories = new GameObjectFactories(_container);
		_handles = new List<ICollectingHandle>();
	}

	public void OnUpdate(float deltaTime)
	{
		for (int i = _handles.Count - 1; i >= 0; i--)
		{
			if (_handles[i].Process(deltaTime))
			{
				CollectingObjectInfo collectingObject = _handles[i].CollectingObjectInfo;
				_factories.Dispose(collectingObject.prefab, collectingObject.instanceObject);

				_handles[i].SendSourceEnd();

				_handles.RemoveAt(i);
			}
		}
	}

	public struct CollectingInfo<T> where T : struct
	{
		public CollectingObjectInfo collectingObject;
		
		public CollectingOriginInfo<T> origin;
		public CollectingTargetInfo<T> target;

		public CollectingData data;

		public float duration;
	}
	public struct CollectingObjectInfo
	{
		public GameObject prefab;
		public GameObject instanceObject;
	}
	public struct CollectingOriginInfo<T> where T : struct
	{
		public ICollectingSource<T> source;

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;

		public T collectingInfo;
	}
	public struct CollectingTargetInfo<T> where T : struct
	{
		public ICollectingSource<T> source;

		public Transform transform;
		public Vector3 scale;

		public T collectingInfo;
	}

	public void Create<T>(
		T originInfo,
		T targetInfo,
		GameObject prefab,
		ICollectingSource<T> originSource,
		ICollectingSource<T> targetSource,
		Transform originTransform,
		Transform targetTransform,
		float duration) where T : struct
	{
		CollectingData originCollectingData = originSource.CollectingData();
		CollectingData targetCollectingData = targetSource.CollectingData();

		CollectingInfo<T> collectingInfo = new CollectingInfo<T>
		{
			collectingObject = new CollectingObjectInfo
			{
				prefab = prefab,
				instanceObject = _factories.Instantiate(prefab, originTransform.position, originTransform.rotation, originCollectingData.scale)
			},

			origin = new CollectingOriginInfo<T>
			{
				source = originSource,
				position = originTransform.position,
				rotation = originTransform.rotation,
				scale = originCollectingData.scale,
				collectingInfo = originInfo
			},
			target = new CollectingTargetInfo<T>
			{
				source = targetSource,
				transform = targetTransform,
				scale = targetCollectingData.scale,
				collectingInfo = targetInfo
			},

			data = targetCollectingData,

			duration = duration
		};

		CollectingHandle<T> handle = new CollectingHandle<T>(collectingInfo);

		handle.SendSourceStart();

		_handles.Add(handle);
	}
}