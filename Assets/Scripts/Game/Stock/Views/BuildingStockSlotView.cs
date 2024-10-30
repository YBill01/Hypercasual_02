public class BuildingStockSlotView : StockSlotView
{
	protected override void GenerateCells()
	{
		_cells = new Cell[_data.capacity];
	}
}