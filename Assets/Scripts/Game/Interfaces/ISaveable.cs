public interface ISaveable<T> where T : class
{
	T GetSaveData();
	bool SetSaveData(T data);
}