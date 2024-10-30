using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Core/SharedViewData", fileName = "SharedView", order = 6)]
	public class SharedViewData : ScriptableObject
	{
		public ItemView[] itemsViewData;

		private Dictionary<ItemType, ItemViewData> _items;

		[Serializable]
		public struct ItemView
		{
			public ItemType type;
			public ItemViewData view;
		}

		public void HashingData()
		{
			_items = new Dictionary<ItemType, ItemViewData>();

			for (int i = 0; i < itemsViewData.Length; i++)
			{
				_items.Add(itemsViewData[i].type, itemsViewData[i].view);
			}
		}

		public ItemViewData GetItemViewData(ItemType type)
		{
			if (_items.TryGetValue(type, out ItemViewData value))
			{
				return value;
			}

			return default;
		}
	}
}