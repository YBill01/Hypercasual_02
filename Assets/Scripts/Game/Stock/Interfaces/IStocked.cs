public interface IStocked
{
	Stock Stock { get; }

	bool TryStockLink(StockLink link);
	bool TryStockUnlink(StockLink link);
}