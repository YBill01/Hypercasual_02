using GameName.Core;
using GameName.Data;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CPlayer : MonoBehaviour, IPausable, IUpdatable, ISaveable<PlayerData.PlayerStats>, IStocked
{
	[SerializeField]
	private PlayerConfigData m_config;

	[field: SerializeField, Space]
	public VCameraTarget CameraTarget { get; private set; }

	[Header("Stock")]
	[SerializeField]
	private Stock m_stock;

	public Stock Stock => m_stock;

	private List<StockLink> _stockLinks;

	private CPlayerController _controller;

	public bool IsCollecting => _stockLinks.Count > 0;
	public bool IsCollectingCooldown => _collectingCooldown > 0.0f;

	private float _collectingCooldown = 0.0f;

	private IObjectResolver _resolver;

	private StockEvents _stockEvents;

	[Inject]
	public void Construct(
		IObjectResolver resolver,
		StockEvents stockEvents)
	{
		_resolver = resolver;
		_stockEvents = stockEvents;

		_resolver.InjectGameObject(m_stock.gameObject);
	}

	private void Awake()
	{
		_controller = GetComponent<CPlayerController>();

		_stockLinks = new List<StockLink>();
	}

	public void SetPause(bool pause)
	{
		_controller.SetPause(pause);
	}

	public void OnUpdate(float deltaTime)
	{
		m_stock.OnUpdate(deltaTime);

		if (IsCollectingCooldown)
		{
			_collectingCooldown -= deltaTime;
		}

		if (IsCollecting && !IsCollectingCooldown)
		{
			if (TrySendCollecting())
			{
				_collectingCooldown = m_config.collectingDuration;

				_controller.Collect();
			}
		}

		_controller.OnUpdate(deltaTime);
	}

	public void SetOrientation(Vector3 position, Quaternion rotation)
	{
		_controller.SetOrientation(position, rotation);
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

	private bool TrySendCollecting()
	{
		for (int i = 0; i < _stockLinks.Count; i++)
		{
			foreach (StockItem item in _stockLinks[i].stockIn)
			{
				if (!item.inactive && _stockLinks[i].stockOut.HasEmpty(item))
				{
					_stockEvents.OnTransfer.OnNext(new StockTransfer
					{
						item = item,
						stockIn = _stockLinks[i].stockIn,
						stockOut = _stockLinks[i].stockOut
					});

					return true;
				}
			}
		}

		return false;
	}

	public PlayerData.PlayerStats GetSaveData()
	{
		PlayerData.PlayerStats stats = new PlayerData.PlayerStats();

		stats.stock = m_stock.GetSaveData();

		return stats;
	}
	public bool SetSaveData(PlayerData.PlayerStats data)
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