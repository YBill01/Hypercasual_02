using GameName.Data;
using R3;
using System;
using UnityEngine;

public class StockBehaviour : IDisposable
{
	private SharedViewData _viewData;
	private StockEvents _stockEvents;
	private CollectingEvents _collectingEvents;

	private CompositeDisposable _compositeDisposable;

	public StockBehaviour(
		SharedViewData viewData,
		StockEvents stockEvents,
		CollectingEvents collectingEvents)
	{
		_viewData = viewData;
		_stockEvents = stockEvents;
		_collectingEvents = collectingEvents;

		_compositeDisposable = new CompositeDisposable();

		_stockEvents.OnLink
			.Subscribe(x => OnLink(x))
			.AddTo(_compositeDisposable);

		_stockEvents.OnUnlink
			.Subscribe(x => OnUnlink(x))
			.AddTo(_compositeDisposable);

		_stockEvents.OnTransfer
			.Subscribe(x => OnTransfer(x))
			.AddTo(_compositeDisposable);
	}

	private void OnLink(StockLink link)
	{
		if (link.handler.TryStockLink(link))
		{
			Debug.Log($"linked");
		}
		else
		{
			Debug.Log($"linked error");
		}
	}
	private void OnUnlink(StockLink link)
	{
		if (link.handler.TryStockUnlink(link))
		{
			Debug.Log($"unlinked");
		}
		else
		{
			Debug.Log($"unlinked error");
		}
	}

	private void OnTransfer(StockTransfer transfer)
	{
		if (transfer.stockIn.TryTake(transfer.item, out StockCollectingInfo takeCollectingInfo))
		{
			float duration = transfer.stockIn.TransferDuration + transfer.stockOut.TransferDuration;

			if (transfer.stockOut.TryAdd(transfer.item, out StockCollectingInfo addCollectingInfo, duration))
			{
				if (duration > 0.0f)
				{
					_collectingEvents.OnTransfer.OnNext(new StockCollectingTransfer
					{
						originInfo = takeCollectingInfo,
						targetInfo = addCollectingInfo,
						
						prefab = _viewData.GetItemViewData(transfer.item.type).prefab,
						
						duration = duration
					});
				}
			}
		}
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}