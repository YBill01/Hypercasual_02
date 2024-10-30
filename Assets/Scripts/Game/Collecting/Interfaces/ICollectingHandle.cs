public interface ICollectingHandle
{
	CollectingBehaviour.CollectingObjectInfo CollectingObjectInfo { get; }
	bool Process(float deltaTime);

	void SendSourceStart();
	void SendSourceEnd();
}