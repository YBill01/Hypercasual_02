using GameName.Core;
using GameName.Data;
using R3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stock : MonoBehaviour, IUpdatable, ISaveable<PlayerData.Stock>, IEnumerable<StockItem>
{
	public readonly Subject<Unit> OnAdd = new();
	public readonly Subject<Unit> OnTake = new();
	
	[SerializeField]
	private StockData m_data;

	[Space]
	[SerializeField]
	private StockView m_view;

	public int Capacity => _slots.Sum(x => x.Capacity);
	public int Count => _slots.Sum(x => x.Count);

	public bool IsEmpty => Count == 0;
	public bool IsFull => Count >= Capacity;

	public float TransferDuration => m_data.transferDuration;

	private List<StockSlot> _slots;

	private List<TransferHandle> _transferHandles;

	private void Awake()
	{
		_slots = new List<StockSlot>();
		_transferHandles = new List<TransferHandle>();

		for (int i = 0; i < m_data.slots.Length; i++)
		{
			_slots.Add(new StockSlot(m_data.slots[i]));
		}

		m_view.Init(m_data);
	}

	public void OnUpdate(float deltaTime)
	{
		for (int i = _transferHandles.Count - 1; i >= 0; i--)
		{
			if (_transferHandles[i].Process(deltaTime))
			{
				DoneTransferHandle(_transferHandles[i]);

				_transferHandles.RemoveAt(i);
			}
		}

		m_view.OnUpdate(deltaTime);
	}

	public bool TryAdd(StockItem stockItem, float duration = 0.0f)
	{
		if (TryAdd(stockItem, out StockItem newItockItem, out int slotIndex, out int itemIndex, duration))
		{
			if (!m_view.TryAdd(slotIndex, itemIndex, newItockItem))
			{
				// TODO log
			}

			return true;
		}

		return false;
	}
	public bool TryAdd(StockItem stockItem, out StockItem newItockItem, out int slotIndex, out int itemIndex, float duration = 0.0f)
	{
		slotIndex = -1;
		itemIndex = -1;

		newItockItem = new StockItem
		{
			type = stockItem.type,
			inactive = duration > 0.0f
		};

		for (int i = 0; i < _slots.Count; i++)
		{
			if (_slots[i].TryAdd(newItockItem, out int index))
			{
				if (duration > 0.0f)
				{
					AddTranferHandles(stockItem, i, index, duration);
				}

				slotIndex = i;
				itemIndex = index;

				OnAdd.OnNext(Unit.Default);

				return true;
			}
		}

		return false;
	}
	public bool TryAdd(StockItem stockItem, out StockCollectingInfo collectingInfo, float duration = 0.0f)
	{
		collectingInfo = default;

		if (TryAdd(stockItem, out StockItem newItockItem, out int slotIndex, out int itemIndex, duration))
		{
			if (!m_view.TryAdd(slotIndex, itemIndex, newItockItem, out collectingInfo))
			{
				// TODO log
			}

			return true;
		}

		return false;
	}

	public bool TryTake(StockItem stockItem)
	{
		if (TryTake(stockItem, out int slotIndex, out int itemIndex))
		{
			if (!m_view.TryTake(slotIndex, itemIndex, stockItem))
			{
				// TODO log
			}

			OnTake.OnNext(Unit.Default);

			return true;
		}

		return false;
	}
	public bool TryTake(StockItem stockItem, out StockCollectingInfo collectingInfo)
	{
		collectingInfo = default;

		if (TryTake(stockItem, out int slotIndex, out int itemIndex))
		{
			if (!m_view.TryTake(slotIndex, itemIndex, stockItem, out collectingInfo))
			{
				// TODO log
			}

			OnTake.OnNext(Unit.Default);

			return true;
		}

		return false;
	}
	private bool TryTake(StockItem stockItem, out int slotIndex, out int itemIndex)
	{
		slotIndex = -1;
		itemIndex = -1;

		for (int i = 0; i < _slots.Count; i++)
		{
			if (_slots[i].TryTake(stockItem, out int index))
			{
				slotIndex = i;
				itemIndex = index;

				CorrectionTranferHandles(i, index);

				return true;
			}
		}

		return false;
	}

	public bool Has(StockItem stockItem)
	{
		if (stockItem.inactive)
		{
			return false;
		}

		for (int i = 0; i < _slots.Count; i++)
		{
			if (_slots[i].Has(stockItem))
			{
				return true;
			}
		}

		return false;
	}
	public bool HasEmpty(StockItem stockItem)
	{
		for (int i = 0; i < _slots.Count; i++)
		{
			if (_slots[i].HasEmpty(stockItem))
			{
				return true;
			}
		}

		return false;
	}

	public bool Clear()
	{
		foreach (StockSlot slot in _slots)
		{
			slot.Clear();
		}

		return true;
	}

	public PlayerData.Stock GetSaveData()
	{
		PlayerData.Stock stock = new PlayerData.Stock
		{
			slots = new PlayerData.StockSlot[_slots.Count],
			transferHandles = new PlayerData.StockTransferHandle[_transferHandles.Count]
		};

		for (int i = 0; i < _slots.Count; i++)
		{
			stock.slots[i] = _slots[i].GetSaveData();
		}

		for (int i = 0; i < _transferHandles.Count; i++)
		{
			stock.transferHandles[i] = new PlayerData.StockTransferHandle
			{
				stockItem = new PlayerData.StockItem
				{
					type = (int)_transferHandles[i].stockItem.type,
					inactive = _transferHandles[i].stockItem.inactive
				},
				slotIndex = _transferHandles[i].slotIndex,
				itemIndex = _transferHandles[i].itemIndex,
				time = _transferHandles[i].time,
				duration = _transferHandles[i].duration
			};
		}

		return stock;
	}
	public bool SetSaveData(PlayerData.Stock data)
	{
		try
		{
			for (int i = 0; i < data.slots.Length; i++)
			{
				if (!_slots[i].SetSaveData(data.slots[i]))
				{
					return false;
				}
			}

			for (int i = 0; i < data.transferHandles.Length; i++)
			{
				AddTranferHandles(new StockItem
				{
					type = (ItemType)data.transferHandles[i].stockItem.type,
					inactive = data.transferHandles[i].stockItem.inactive
				},
				data.transferHandles[i].slotIndex,
				data.transferHandles[i].itemIndex,
				data.transferHandles[i].duration);
			}

			m_view.Filling(_slots);

			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}

	private void AddTranferHandles(StockItem stockItem, int slotIndex, int itemIndex, float duration)
	{
		_transferHandles.Add(new TransferHandle
		{
			stockItem = stockItem,
			slotIndex = slotIndex,
			itemIndex = itemIndex,
			duration = duration
		});
	}
	private void CorrectionTranferHandles(int slotIndex, int itemIndex)
	{
		foreach (TransferHandle handle in _transferHandles)
		{
			if (handle.slotIndex == slotIndex && handle.itemIndex > itemIndex)
			{
				handle.itemIndex--;
			}
		}
	}
	private void DoneTransferHandle(TransferHandle handle)
	{
		_slots[handle.slotIndex].Insert(handle.stockItem, handle.itemIndex);

		if (!m_view.TryAdd(handle.slotIndex, handle.itemIndex, handle.stockItem))
		{
			// TODO log
		}

		OnAdd.OnNext(Unit.Default);
	}

	public IEnumerator<StockItem> GetEnumerator()
	{
		foreach(StockSlot slot in _slots)
		{
			foreach (StockItem item in slot)
			{
				yield return item;
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private class TransferHandle
	{
		public StockItem stockItem;

		public int slotIndex;
		public int itemIndex;

		public float time = 0.0f;
		public float duration;

		public bool Process(float deltaTime)
		{
			time += deltaTime;

			return time >= duration;
		}
	}
}