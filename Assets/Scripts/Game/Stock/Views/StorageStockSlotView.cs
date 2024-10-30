using UnityEngine;

public class StorageStockSlotView : StockSlotView
{
	[Space]
	[SerializeField]
	private SpriteRenderer m_spriteIcon;

	private void Start()
	{
		m_spriteIcon.sprite = _viewData.GetItemViewData(_data.itemsFilter.items[0]).icon;
	}
}