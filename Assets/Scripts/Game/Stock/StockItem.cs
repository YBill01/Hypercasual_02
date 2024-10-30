using System;

public struct StockItem : IEquatable<StockItem>
{
	public ItemType type;

	public bool inactive;

	public bool Equals(StockItem other)
	{
		return type == other.type && inactive == other.inactive;
	}
}