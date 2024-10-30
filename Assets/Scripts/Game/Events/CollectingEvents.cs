using R3;

public class CollectingEvents
{
	public readonly Subject<StockCollectingTransfer> OnTransfer = new();
}