using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/StockData", fileName = "Stock", order = 50)]
	public class StockData : ScriptableObject
	{
		public StockSlotData[] slots;

		[Space]
		public float transferDuration;
	}
}