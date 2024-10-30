using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/BuildingConfigData", fileName = "BuildingConfig", order = 31)]
	public class BuildingConfigData : ScriptableObject
	{
		public StockData stock;

		[Space]
		public RecipeData[] recipes;
	}
}