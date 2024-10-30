using GameName.Data;
using R3;
using System.Collections.Generic;
using UnityEngine;

public class ProductionHandle
{
	public ReadOnlyReactiveProperty<float> Progress => _t;
	public bool IsComplete => _t.Value >= 1.0f;

	private List<StockItem> _rewards;
	public List<StockItem> Rewards => _rewards;

	private float _time = 0.0f;
	public float Time => _time;

	private ReactiveProperty<float> _t = new ReactiveProperty<float>(0.0f);

	private float _duration;
	public float Duration => _duration;

	public ProductionHandle(RecipeData recipe)
	{
		_rewards = new List<StockItem>();
		foreach (RecipeData.RecipeItem recipeItem in recipe.itemsOut)
		{
			for (int i = 0; i < recipeItem.count; i++)
			{
				_rewards.Add(new StockItem
				{
					type = recipeItem.type
				});
			}
		}

		_duration = recipe.productionDuration;
	}

	public ProductionHandle(float time, float duration, List<StockItem> rewards)
	{
		_time = time;
		_duration = duration;
		_rewards = rewards;
	}

	public bool Process(float deltaTime)
	{
		_time += deltaTime;

		_t.Value = Mathf.Clamp01(_time / _duration);

		return _time >= _duration;
	}
}