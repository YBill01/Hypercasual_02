using R3;

public class ProductionEvents
{
	public readonly Subject<ProductionBehaviour> OnProducting = new();
	public readonly Subject<ProductionBehaviour> OnStockEmpty = new();
	public readonly Subject<ProductionBehaviour> OnStockFull = new();
}