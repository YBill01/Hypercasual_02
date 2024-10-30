using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/StockSlotFallData", fileName = "StockSlotFall", order = 52)]
	public class StockSlotFallData : ScriptableObject
	{
		public AnimationCurve positionCurve;

		public float delay;
		public float duration;
	}
}