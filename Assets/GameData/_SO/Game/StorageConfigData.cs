using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/StorageConfigData", fileName = "StorageConfig", order = 32)]
	public class StorageConfigData : ScriptableObject
	{
		public StockData stock;
	}
}