using GameName.Core;
using GameName.Data;
using UnityEngine;
using VContainer;

public class Storage : MonoBehaviour, IUpdatable, ISaveable<PlayerData.StorageStats>, IStocked
{
	[SerializeField]
	private StorageConfigData m_config;

	[Header("Stock")]
	[SerializeField]
	private Stock m_stock;

	public Stock Stock => m_stock;

	[Space]
	[SerializeField]
	private LayerMask m_interactLayerMask;

	[SerializeField]
	private StorageInteractType m_interactType;

	[Space]
	[SerializeField]
	private SpriteRenderer m_spriteIn;
	[SerializeField]
	private SpriteRenderer m_spriteOut;

	private StockEvents _stockEvents;

	[Inject]
	public void Construct(
		StockEvents stockEvents)
	{
		_stockEvents = stockEvents;
	}

	private void Start()
	{
		switch (m_interactType)
		{
			case StorageInteractType.None:
				m_spriteIn.gameObject.SetActive(false);
				m_spriteOut.gameObject.SetActive(false);

				break;
			case StorageInteractType.In:
				m_spriteIn.gameObject.SetActive(true);
				m_spriteOut.gameObject.SetActive(false);

				break;
			case StorageInteractType.Out:
				m_spriteIn.gameObject.SetActive(false);
				m_spriteOut.gameObject.SetActive(true);

				break;
			default:
				break;
		}
	}

	public void OnUpdate(float deltaTime)
	{
		m_stock.OnUpdate(deltaTime);
	}

	public bool TryStockLink(StockLink link)
	{
		throw new System.NotImplementedException();
	}
	public bool TryStockUnlink(StockLink link)
	{
		throw new System.NotImplementedException();
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (IsInteract(collision) && m_interactType != StorageInteractType.None)
		{
			if (collision.TryGetComponent(out IStocked stockedComponent))
			{
				_stockEvents.OnLink.OnNext(GetStockLink(stockedComponent));
			}
		}
	}
	private void OnTriggerExit(Collider collision)
	{
		if (IsInteract(collision) && m_interactType != StorageInteractType.None)
		{
			if (collision.TryGetComponent(out IStocked stockedComponent))
			{
				_stockEvents.OnUnlink.OnNext(GetStockLink(stockedComponent));
			}
		}
	}

	private bool IsInteract(Collider collision)
	{
		return (m_interactLayerMask & (1 << collision.gameObject.layer)) != 0;
	}

	private StockLink GetStockLink(IStocked stockedComponent)
	{
		return m_interactType switch
		{
			StorageInteractType.In => new StockLink
			{
				handler = stockedComponent,
				stockIn = stockedComponent.Stock,
				stockOut = Stock
			},
			StorageInteractType.Out => new StockLink
			{
				handler = stockedComponent,
				stockIn = Stock,
				stockOut = stockedComponent.Stock
			},
			_ => new StockLink
			{
				handler = stockedComponent,
				stockIn = stockedComponent.Stock,
				stockOut = Stock
			},
		};
	}

	public PlayerData.StorageStats GetSaveData()
	{
		PlayerData.StorageStats stats = new PlayerData.StorageStats();

		stats.stock = m_stock.GetSaveData();

		return stats;
	}
	public bool SetSaveData(PlayerData.StorageStats data)
	{
		try
		{
			if (!m_stock.SetSaveData(data.stock))
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