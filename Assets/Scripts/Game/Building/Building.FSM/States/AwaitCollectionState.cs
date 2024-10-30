using R3;
using System.Collections.Generic;
using System.Linq;
using YB.HFSM;

namespace GameName.BuildingFSM
{
	public class AwaitCollectionState : State
	{
		private ProductionBehaviour _productionBehaviour;

		private Stock _innerStock;
		private List<StockLink> _externalStockLinks;

		private BuildingIcon _icon;

		private StockEvents _stockEvents;

		private CompositeDisposable _compositeDisposable;

		public AwaitCollectionState(ProductionBehaviour productionBehaviour, Stock stock, List<StockLink> externalStockLinks, StockEvents stockEvents, BuildingIcon icon)
		{
			_productionBehaviour = productionBehaviour;

			_innerStock = stock;
			_externalStockLinks = externalStockLinks;
			_stockEvents = stockEvents;

			_icon = icon;
		}

		protected override void OnEnter()
		{
			_compositeDisposable = new CompositeDisposable();

			_innerStock.OnTake
				.Subscribe(_ => InnerStockOnTake())
				.AddTo(_compositeDisposable);

			foreach (StockLink stockLink in _externalStockLinks)
			{
				stockLink.stockOut.OnTake
					.Subscribe(_ => InnerStockOnTake())
					.AddTo(_compositeDisposable);
			}

			InnerStockOnTake();

			_icon.Progress(1.0f);
		}
		protected override void OnExit()
		{
			_compositeDisposable.Dispose();
		}

		private void InnerStockOnTake()
		{
			if (TryTakeReward())
			{
				if (TryReleaseInnerStock())
				{
					_productionBehaviour.EndProduction();

					_compositeDisposable.Dispose();
				}
				else
				{
					_productionBehaviour.OnStockIsFull.OnNext(_externalStockLinks.Select(x => x.stockOut).ToList());
				}
			}
			else
			{
				if (TryReleaseInnerStock())
				{

				}
				else
				{
					_productionBehaviour.OnStockIsFull.OnNext(_externalStockLinks.Select(x => x.stockOut).ToList());
				}
			}
		}

		private bool TryTakeReward()
		{
			List<StockItem> rewards = _productionBehaviour.ProductionHandle.Rewards;

			for (int i = rewards.Count - 1; i >= 0; i--)
			{
				if (_innerStock.TryAdd(rewards[i]))
				{
					rewards.RemoveAt(i);
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		private bool TryReleaseInnerStock()
		{
			foreach (StockLink stockLink in _externalStockLinks)
			{
				foreach (StockItem item in stockLink.stockIn)
				{
					if (stockLink.stockOut.HasEmpty(item))
					{
						_stockEvents.OnTransfer.OnNext(new StockTransfer
						{
							item = item,
							stockIn = stockLink.stockIn,
							stockOut = stockLink.stockOut
						});

						return _innerStock.IsEmpty;
					}
				}
			}

			return _innerStock.IsEmpty;
		}
	}
}