using GameName.Core;
using GameName.Data;
using R3;
using System.Collections.Generic;

public class ProductionBehaviour : IUpdatable, ISaveable<PlayerData.Production>
{
	public readonly Subject<bool> OnProduction = new();
	public readonly Subject<float> OnProductionProgress = new();
	public readonly Subject<List<Stock>> OnStockIsFull = new();
	public readonly Subject<List<Stock>> OnStockIsDeficiently = new();

	public bool IsAwait() => _productionHandle is null;
	public bool IsBusy() => _productionHandle is not null;
	public bool IsProductComplete() => IsBusy() && _productionHandle.IsComplete;
	
	private ProductionHandle _productionHandle;
	public ProductionHandle ProductionHandle => _productionHandle;

	public void OnUpdate(float deltaTime)
	{
		if (IsBusy() && !_productionHandle.IsComplete && _productionHandle.Process(deltaTime))
		{
			// complete...
		}
	}

	public ProductionHandle StartProduction(RecipeData recipe)
	{
		_productionHandle = new ProductionHandle(recipe);

		OnProduction.OnNext(true);

		return _productionHandle;
	}
	public void EndProduction()
	{
		_productionHandle = null;

		OnProduction.OnNext(false);
	}

	public PlayerData.Production GetSaveData()
	{
		if (IsBusy())
		{
			PlayerData.Production production = new PlayerData.Production();

			List<StockItem> rewardsList = _productionHandle.Rewards;
			PlayerData.StockItem[] rewards = new PlayerData.StockItem[rewardsList.Count];

			for (int i = 0; i < rewardsList.Count; i++)
			{
				rewards[i] = new PlayerData.StockItem
				{
					type = (int)rewardsList[i].type,
					inactive = rewardsList[i].inactive
				};
			}

			production.time = _productionHandle.Time;
			production.duration = _productionHandle.Duration;
			production.rewards = rewards;

			return production;
		}

		return null;
	}
	public bool SetSaveData(PlayerData.Production data)
	{
		try
		{
			List<StockItem> reward = new List<StockItem>(data.rewards.Length);

			for (int i = 0; i < data.rewards.Length; i++)
			{
				reward.Add(new StockItem
				{
					type = (ItemType)data.rewards[i].type,
					inactive = data.rewards[i].inactive
				});
			}

			_productionHandle = new ProductionHandle(data.time, data.duration, reward);

			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}
}