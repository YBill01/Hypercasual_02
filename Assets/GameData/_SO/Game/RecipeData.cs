using System;
using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/RecipeData", fileName = "Recipe", order = 40)]
	public class RecipeData : ScriptableObject
	{
		public int priority;

		[Space]
		public RecipeItem[] itemsIn;
		public RecipeItem[] itemsOut;

		[Space]
		public float productionDuration;

		[Serializable]
		public struct RecipeItem
		{
			public ItemType type;
			public int count;
		}
	}
}