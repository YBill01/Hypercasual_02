using System;

public struct StockLink : IEquatable<StockLink>
{
	public IStocked handler;

	public Stock stockIn;
	public Stock stockOut;

	public bool Equals(StockLink other)
	{
		return handler.Equals(other.handler) && stockIn.Equals(other.stockIn) && stockOut.Equals(other.stockOut);
	}
}