using System;
using System.Linq;

[Serializable]
public struct ItemsFilter
{
	public ItemType[] items;

	public bool Has(ItemType itemType)
	{
		return items.Contains(itemType);
	}
}