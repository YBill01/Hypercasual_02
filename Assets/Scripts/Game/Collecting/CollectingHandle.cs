using UnityEngine;

public class CollectingHandle<T> : ICollectingHandle where T : struct
{
	public CollectingBehaviour.CollectingObjectInfo CollectingObjectInfo => _info.collectingObject;
	
	private CollectingBehaviour.CollectingInfo<T> _info;

	private float _time = 0.0f;

	private Transform _instanceObjectTransform;

	public CollectingHandle(CollectingBehaviour.CollectingInfo<T> info)
	{
		_info = info;

		_instanceObjectTransform = _info.collectingObject.instanceObject.transform;
	}

	public bool Process(float deltaTime)
	{
		_time += deltaTime;

		float t = Mathf.Clamp01(_info.data.timeCurve.Evaluate(_time / _info.duration));

		Vector3 position = Vector3.Lerp(_info.origin.position, _info.target.transform.position, _info.data.positionCurve.Evaluate(t));
		position.y += _info.data.height * _info.data.heightCurve.Evaluate(t);

		Quaternion rotation = Quaternion.Lerp(_info.origin.rotation, _info.target.transform.rotation, _info.data.rotationCurve.Evaluate(t));
		Vector3 scale = Vector3.Lerp(_info.origin.scale, _info.target.scale, _info.data.scaleCurve.Evaluate(t));

		_instanceObjectTransform.SetPositionAndRotation(position, rotation);
		_instanceObjectTransform.localScale = scale;

		return _time >= _info.duration;
	}

	public void SendSourceStart()
	{
		_info.origin.source.CollectingStart(_info.origin.collectingInfo);
	}
	public void SendSourceEnd()
	{
		_info.target.source.CollectingEnd(_info.target.collectingInfo);
	}
}