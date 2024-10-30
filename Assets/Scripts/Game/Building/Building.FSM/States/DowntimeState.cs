using GameName.Data;
using R3;
using System.Collections.Generic;
using System.Linq;
using YB.HFSM;

namespace GameName.BuildingFSM
{
	public class DowntimeState : State
	{
		private ProductionBehaviour _productionBehaviour;

		private List<RecipeData> _recipes;

		private Stock _innerStock;
		private List<StockLink> _externalStockLinks;

		private BuildingIcon _icon;

		private StockEvents _stockEvents;

		private CompositeDisposable _compositeDisposable;

		public DowntimeState(ProductionBehaviour productionBehaviour, RecipeData[] recipes, Stock stock, List<StockLink> externalStockLinks, StockEvents stockEvents, BuildingIcon icon)
		{
			_productionBehaviour = productionBehaviour;

			_recipes = recipes.ToList();
			_recipes.Sort((x, y) => y.priority.CompareTo(x.priority));

			_innerStock = stock;
			_externalStockLinks = externalStockLinks;
			_stockEvents = stockEvents;

			_icon = icon;
		}

		protected override void OnEnter()
		{
			_compositeDisposable = new CompositeDisposable();

			_innerStock.OnAdd
				.Subscribe(_ => InnerStockOnAdd())
				.AddTo(_compositeDisposable);

			foreach (StockLink stockLink in _externalStockLinks)
			{
				stockLink.stockOut.OnAdd
					.Subscribe(_ => InnerStockOnAdd())
					.AddTo(_compositeDisposable);
			}

			InnerStockOnAdd();

			_icon.Progress(0.0f);
		}
		protected override void OnExit()
		{
			_compositeDisposable.Dispose();
		}

		private void InnerStockOnAdd()
		{
			if (!TryStartProduction())
			{
				if (CheckExternalPrice(out RecipeData recipe))
				{
					if (TryFillInnerStock(recipe))
					{

					}
				}
				else
				{
					_productionBehaviour.OnStockIsDeficiently.OnNext(_externalStockLinks.Select(x => x.stockOut).ToList());
				}
			}
		}

		private bool CheckExternalPrice(out RecipeData recipe)
		{
			recipe = null;

			foreach (RecipeData recipeData in _recipes)
			{
				List<StockItem> price = GetRecipePrice(recipeData);

				HasPrice(price, _innerStock, out List<StockItem> newPrice, true);

				foreach (StockLink stockLink in _externalStockLinks)
				{
					if (HasPrice(newPrice, stockLink.stockOut, out newPrice))
					{
						recipe = recipeData;

						return true;
					}
				}
			}

			return false;
		}

		private bool TryFillInnerStock(RecipeData recipe)
		{
			List<StockItem> price = GetRecipePrice(recipe);

			if (!HasPrice(price, _innerStock, out List<StockItem> newPrice, true))
			{
				foreach (StockItem priceStockItem in newPrice)
				{
					foreach (StockLink stockLink in _externalStockLinks)
					{
						if (stockLink.stockOut.Has(priceStockItem))
						{
							if (stockLink.stockIn.HasEmpty(priceStockItem))
							{
								_stockEvents.OnTransfer.OnNext(new StockTransfer
								{
									item = priceStockItem,
									stockIn = stockLink.stockOut,
									stockOut = stockLink.stockIn
								});

								return true;
							}
						}
					}
				}
			}

			return false;
		}

		private bool TryStartProduction()
		{
			if (TryGetRecipe(out RecipeData recipe))
			{
				_productionBehaviour.StartProduction(recipe);

				_compositeDisposable.Dispose();

				return true;
			}

			return false;
		}

		private bool TryGetRecipe(out RecipeData recipe)
		{
			recipe = null;

			foreach (RecipeData recipeData in _recipes)
			{
				if (TryTakePrice(recipeData))
				{
					recipe = recipeData;

					return true;
				}
			}

			return false;
		}

		private bool TryTakePrice(RecipeData recipe)
		{
			List<StockItem> price = GetRecipePrice(recipe);

			if (HasPrice(price, _innerStock, out List<StockItem> newPrice))
			{
				foreach (StockItem priceStockItem in price)
				{
					_innerStock.TryTake(priceStockItem);
				}

				return true;
			}

			return false;
		}

		private bool HasPrice(List<StockItem> price, Stock stock, out List<StockItem> newPrice, bool allowInactive = false)
		{
			newPrice = price.ToList();

			foreach (StockItem stockItem in stock)
			{
				for (int i = 0; i < newPrice.Count; i++)
				{
					if (allowInactive)
					{
						if (stockItem.type == newPrice[i].type)
						{
							newPrice.RemoveAt(i);

							break;
						}
					}
					else
					{
						if (stockItem.Equals(newPrice[i]))
						{
							newPrice.RemoveAt(i);

							break;
						}
					}
				}

				if (newPrice.Count == 0)
				{
					return true;
				}
			}

			return newPrice.Count == 0;
		}

		private List<StockItem> GetRecipePrice(RecipeData recipe)
		{
			List<StockItem> price = new List<StockItem>();

			foreach (RecipeData.RecipeItem recipeItem in recipe.itemsIn)
			{
				for (int i = 0; i < recipeItem.count; i++)
				{
					price.Add(new StockItem
					{
						type = recipeItem.type
					});
				}
			}

			return price;
		}
	}
}