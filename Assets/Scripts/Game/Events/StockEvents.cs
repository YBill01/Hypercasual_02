using R3;

public class StockEvents
{
	public readonly Subject<StockLink> OnLink = new();
	public readonly Subject<StockLink> OnUnlink = new();

	public readonly Subject<StockTransfer> OnTransfer = new();
}