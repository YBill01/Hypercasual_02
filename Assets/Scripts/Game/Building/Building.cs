using GameName.BuildingFSM;
using GameName.Core;
using GameName.Data;
using R3;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class Building : MonoBehaviour, IUpdatable, ISaveable<PlayerData.BuildingStats>, IStocked
{
	[SerializeField]
	private BuildingConfigData m_config;

	[Header("Stock")]
	[SerializeField]
	private Stock m_stock;

	public Stock Stock => m_stock;

	[Space]
	[SerializeField]
	private Storage m_storageIn;
	[SerializeField]
	private Storage m_storageOut;

	[Space]
	[SerializeField]
	private BuildingIcon m_icon;

	private List<StockLink> _stockLinks;

	private ProductionBehaviour _productionBehaviour;
	private ProductionBehaviour Production => _productionBehaviour;



	private BuildingStateMachine _buildingStateMachine;

	private CompositeDisposable _compositeDisposable;


	private IObjectResolver _resolver;
	private StockEvents _stockEvents;
	protected SharedViewData _viewData;

	[Inject]
	public void Construct(
		IObjectResolver resolver,
		StockEvents stockEvents,
		SharedViewData viewData)
	{
		_resolver = resolver;
		_stockEvents = stockEvents;
		_viewData = viewData;
	}

	private void Awake()
	{
		_stockLinks = new List<StockLink>();

		_productionBehaviour = new ProductionBehaviour();

		DowntimeState downtimeState = new DowntimeState(_productionBehaviour, m_config.recipes, m_stock, _stockLinks, _stockEvents, m_icon);
		ProductionState productionState = new ProductionState(_productionBehaviour, m_icon);
		AwaitCollectionState awaitCollectionState = new AwaitCollectionState(_productionBehaviour, m_stock, _stockLinks, _stockEvents, m_icon);

		_buildingStateMachine = new BuildingStateMachine(downtimeState, productionState, awaitCollectionState);

		downtimeState.AddTransition(awaitCollectionState, _productionBehaviour.IsProductComplete);
		downtimeState.AddTransition(productionState, _productionBehaviour.IsBusy);
		productionState.AddTransition(awaitCollectionState, _productionBehaviour.IsProductComplete);
		awaitCollectionState.AddTransition(downtimeState, _productionBehaviour.IsAwait);
	}

	private void Start()
	{
		if (m_storageIn)
		{
			_stockEvents.OnLink.OnNext(new StockLink
			{
				handler = this,
				stockIn = m_stock,
				stockOut = m_storageIn.Stock
			});
		}
		
		if (m_storageOut)
		{
			_stockEvents.OnLink.OnNext(new StockLink
			{
				handler = this,
				stockIn = m_stock,
				stockOut = m_storageOut.Stock
			});
		}

		_buildingStateMachine.Init();

		m_icon.SetData(m_config.recipes[0].itemsOut[0], _viewData);
	}

	public void OnUpdate(float deltaTime)
	{
		_productionBehaviour.OnUpdate(deltaTime);
		_buildingStateMachine.Update();

		m_stock.OnUpdate(deltaTime);
	}

	public bool TryStockLink(StockLink link)
	{
		if (_stockLinks.Contains(link))
		{
			return false;
		}

		_stockLinks.Add(link);

		return true;
	}
	public bool TryStockUnlink(StockLink link)
	{
		if (_stockLinks.Contains(link))
		{
			return _stockLinks.Remove(link);
		}

		return false;
	}

	public PlayerData.BuildingStats GetSaveData()
	{
		PlayerData.BuildingStats stats = new PlayerData.BuildingStats();

		stats.stock = m_stock.GetSaveData();
		stats.production = _productionBehaviour.GetSaveData();

		return stats;
	}
	public bool SetSaveData(PlayerData.BuildingStats data)
	{
		try
		{
			if (!m_stock.SetSaveData(data.stock))
			{
				return false;
			}

			if (!_productionBehaviour.SetSaveData(data.production))
			{
				return false;
			}

			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}
}