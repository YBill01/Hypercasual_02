using GameName.Core;
using GameName.Data;
using System.Collections;
using System.Collections.Generic;

public class StockSlot : ISaveable<PlayerData.StockSlot>, IEnumerable<StockItem>
{
	private StockSlotData _config;

	private ItemsFilter _itemsFilter;

	public int Capacity => _config.capacity;
	public int Count => _items.Count;

	public bool IsEmpty => _items.Count == 0;
	public bool IsFull => _items.Count >= _config.capacity;

	private List<StockItem> _items;

	public StockSlot(StockSlotData config)
	{
		_config = config;

		_itemsFilter = _config.itemsFilter;
		_items = new List<StockItem>();
	}

	public bool TryAdd(StockItem stockItem, out int index)
	{
		index = -1;

		if (!HasEmpty(stockItem))
		{
			return false;
		}

		_items.Add(stockItem);

		index = _items.Count - 1;

		return true;
	}
	public bool TryTake(StockItem stockItem, out int index)
	{
		index = -1;

		if (!Has(stockItem))
		{
			return false;
		}

		for (int i = _items.Count - 1; i >= 0; i--)
		{
			if (_items[i].Equals(stockItem))
			{
				_items.RemoveAt(i);

				index = i;

				return true;
			}
		}

		return false;
	}

	public bool Has(StockItem stockItem)
	{
		return !IsEmpty && !stockItem.inactive && _items.Contains(stockItem);
	}
	public bool HasEmpty(StockItem stockItem)
	{
		return !IsFull && _itemsFilter.Has(stockItem.type);
	}

	public void Insert(StockItem stockItem, int index)
	{
		_items[index] = stockItem;
	}

	public bool Clear()
	{
		if (IsEmpty)
		{
			return false;
		}

		_items.Clear();

		return true;
	}

	public PlayerData.StockSlot GetSaveData()
	{
		PlayerData.StockSlot slot = new PlayerData.StockSlot();
		slot.items = new PlayerData.StockItem[_items.Count];

		for (int i = 0; i < _items.Count; i++)
		{
			slot.items[i] = new PlayerData.StockItem()
			{
				type = (int)_items[i].type,
				inactive = _items[i].inactive
			};
		}

		return slot;
	}
	public bool SetSaveData(PlayerData.StockSlot data)
	{
		try
		{
			_items = new List<StockItem>(data.items.Length);

			for (int i = 0; i < data.items.Length; i++)
			{
				_items.Add(new StockItem
				{
					type = (ItemType)data.items[i].type,
					inactive = data.items[i].inactive
				});
			}

			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}

	public IEnumerator<StockItem> GetEnumerator() => _items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}