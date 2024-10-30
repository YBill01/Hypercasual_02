using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Game/ItemViewData", fileName = "ItemView", order = 10)]
	public class ItemViewData : ScriptableObject
	{
		public int id;
		public string itemName;

		public Sprite icon;
		public GameObject prefab;
	}
}