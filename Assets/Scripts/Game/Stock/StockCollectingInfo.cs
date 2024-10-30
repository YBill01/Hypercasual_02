using UnityEngine;

public struct StockCollectingInfo
{
	public StockItem stockItem;

	public ICollectingSource<StockCollectingInfo> source;

	public Transform transform;
}