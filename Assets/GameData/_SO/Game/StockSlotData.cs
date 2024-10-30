using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/StockSlotData", fileName = "StockSlot", order = 51)]
	public class StockSlotData : ScriptableObject
	{
		public ItemsFilter itemsFilter;

		public int capacity;

		[Space]
		public StockSlotFallData fallData;
	}
}