using UnityEngine;

public class StorageStockView : StockView
{
	[Space]
	[SerializeField]
	protected float m_padding;
	[SerializeField]
	protected float m_spacing;

	protected override void GenerateSlots()
	{
		base.GenerateSlots();

		for (int i = 0; i < _slots.Length; i++)
		{
			_slots[i].transform.localPosition = new Vector3(m_padding + (i * -m_spacing), 0.0f, 0.0f);
		}
	}
}