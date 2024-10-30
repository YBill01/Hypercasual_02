using GameName.Data;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public abstract class StockView : MonoBehaviour, IUpdatable, ICollectingSource<StockCollectingInfo>
{
	[SerializeField]
	protected CollectingData m_collectingData;

	[Space]
	[SerializeField]
	protected Transform m_itemsContainer;

	[Space]
	[SerializeField]
	protected Transform m_slotsContainer;
	[SerializeField]
	protected StockSlotView m_slotPrefab;

	protected StockSlotView[] _slots;

	protected StockData _data;

	protected GameObjectFactories _factories;

	protected SharedViewData _viewData;

	[Inject]
	public void Construct(
		SharedViewData viewData)
	{
		_viewData = viewData;
	}

	private void Awake()
	{
		_factories = new GameObjectFactories(m_itemsContainer);
	}

	public void Init(StockData data)
	{
		_data = data;

		GenerateSlots();
	}

	public void OnUpdate(float deltaTime)
	{
		foreach (StockSlotView slot in _slots)
		{
			slot.OnUpdate(deltaTime);
		}
	}

	protected virtual void GenerateSlots()
	{
		_slots = new StockSlotView[_data.slots.Length];

		for (int i = 0; i < _data.slots.Length; i++)
		{
			StockSlotView slot = Instantiate(m_slotPrefab, m_slotsContainer, false);
			slot.Init(_data.slots[i], _viewData);

			_slots[i] = slot;
		}
	}

	public virtual void Filling(List<StockSlot> slots)
	{
		for (int i = 0; i < slots.Count; i++)
		{
			int index = 0;
			foreach (StockItem item in slots[i])
			{
				StockSlotView.Cell cell = _slots[i].GetCell(index++);

				if (item.inactive)
				{
					_slots[i].FillCell(cell, _factories.Instantiate(_viewData.GetItemViewData(ItemType.ItemGhost).prefab, cell.target.position, cell.target.rotation, Vector3.one), true);
				}
				else
				{
					_slots[i].FillCell(cell, _factories.Instantiate(_viewData.GetItemViewData(item.type).prefab, cell.target.position, cell.target.rotation, Vector3.one), false);
				}
			}
		}
	}

	public virtual bool TryAdd(int slotIndex, int itemIndex, StockItem stockItem)
	{
		StockSlotView.Cell cell = _slots[slotIndex].GetCell(itemIndex);

		if (cell.isOccupied && !cell.isGhost)
		{
			return false;
		}

		if (cell.isGhost)
		{
			_factories.Dispose(_viewData.GetItemViewData(ItemType.ItemGhost).prefab, cell.gameObject);
		}

		if (stockItem.inactive)
		{
			_slots[slotIndex].FillCell(cell, _factories.Instantiate(_viewData.GetItemViewData(ItemType.ItemGhost).prefab, cell.target.position, cell.target.rotation, Vector3.one), true);

			return true;
		}

		_slots[slotIndex].FillCell(cell, _factories.Instantiate(_viewData.GetItemViewData(stockItem.type).prefab, cell.target.position, cell.target.rotation, Vector3.one), false);

		return true;
	}
	public virtual bool TryAdd(int slotIndex, int itemIndex, StockItem stockItem, out StockCollectingInfo collectingInfo)
	{
		collectingInfo = default;

		if (TryAdd(slotIndex, itemIndex, stockItem))
		{
			collectingInfo = new StockCollectingInfo
			{
				stockItem = stockItem,
				source = this,
				transform = _slots[slotIndex].GetCell(itemIndex).gameObject.transform
			};

			return true;
		}

		return false;
	}
	public virtual bool TryTake(int slotIndex, int itemIndex, StockItem stockItem)
	{
		StockSlotView.Cell cell = _slots[slotIndex].GetCell(itemIndex);

		if (!cell.isOccupied)
		{
			return false;
		}

		_factories.Dispose(_viewData.GetItemViewData(stockItem.type).prefab, cell.gameObject);

		_slots[slotIndex].ClearCell(cell);

		_slots[slotIndex].UpdateOrder(itemIndex);

		return true;
	}
	public virtual bool TryTake(int slotIndex, int itemIndex, StockItem stockItem, out StockCollectingInfo collectingInfo)
	{
		collectingInfo = default;

		if (TryTake(slotIndex, itemIndex, stockItem))
		{
			collectingInfo = new StockCollectingInfo
			{
				stockItem = stockItem,
				source = this,
				transform = _slots[slotIndex].GetCell(itemIndex).target
			};

			return true;
		}

		return false;
	}

	public virtual CollectingData CollectingData()
	{
		return m_collectingData;
	}
	public virtual void CollectingStart(StockCollectingInfo collectingInfo)
	{

	}
	public virtual void CollectingEnd(StockCollectingInfo collectingInfo)
	{

	}
}