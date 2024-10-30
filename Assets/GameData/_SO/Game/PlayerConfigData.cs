using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/PlayerConfigData", fileName = "PlayerConfig", order = 30)]
	public class PlayerConfigData : ScriptableObject
	{
		public StockData stock;

		public float collectingDuration;
	}
}