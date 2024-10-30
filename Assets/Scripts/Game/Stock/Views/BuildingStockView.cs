using System;
using System.Collections.Generic;

public class BuildingStockView : StockView
{
	protected override void GenerateSlots()
	{
		_slots = Array.Empty<StockSlotView>();
	}

	public override void Filling(List<StockSlot> slots)
	{
		
	}

	public override bool TryAdd(int slotIndex, int itemIndex, StockItem stockItem)
	{
		return true;
	}
	public override bool TryAdd(int slotIndex, int itemIndex, StockItem stockItem, out StockCollectingInfo collectingInfo)
	{
		collectingInfo = new StockCollectingInfo
		{
			stockItem = stockItem,
			source = this,
			transform = transform
		};

		return true;
	}

	public override bool TryTake(int slotIndex, int itemIndex, StockItem stockItem)
	{
		return true;
	}
	public override bool TryTake(int slotIndex, int itemIndex, StockItem stockItem, out StockCollectingInfo collectingInfo)
	{
		collectingInfo = new StockCollectingInfo
		{
			stockItem = stockItem,
			source = this,
			transform = transform
		};

		return true;
	}
}