using GameName.Core;
using R3;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameWorld : MonoBehaviour, IPausable, IUpdatable, ISaveable<PlayerData>, IDisposable
{
	[Header("Objects")]
	[Space]
	[SerializeField]
	private Building[] m_buildings;

	[Space]
	[SerializeField]
	private Storage[] m_storages;

	[Space]
	[SerializeField]
	private Transform m_collectingContainer;

	private CollectingBehaviour _collectingBehaviour;

	private CompositeDisposable _compositeDisposable;

	private IObjectResolver _resolver;

	private Profile _profile;
	private VCamera _vCamera;
	private CPlayer _player;
	private StockEvents _stockEvents;
	private CollectingEvents _collectingEvents;
	private StockBehaviour _stockBehaviour;

	[Inject]
	public void Construct(
		IObjectResolver resolver,
		Profile profile,
		VCamera vCamera,
		CPlayer player,
		StockEvents stockEvents,
		CollectingEvents collectingEvents,
		StockBehaviour stockBehaviour)
	{
		_resolver = resolver;

		_profile = profile;
		_vCamera = vCamera;
		_player = player;
		_stockEvents = stockEvents;
		_collectingEvents = collectingEvents;
		_stockBehaviour = stockBehaviour;

		foreach (Building building in m_buildings)
		{
			_resolver.InjectGameObject(building.gameObject);
		}
		
		foreach (Storage storage in m_storages)
		{
			_resolver.InjectGameObject(storage.gameObject);
		}
	}

	private void Start()
	{
		_collectingBehaviour = new CollectingBehaviour(m_collectingContainer);
		
		_compositeDisposable = new CompositeDisposable();

		_collectingEvents.OnTransfer
			.Subscribe(x => CollectingOnTransfer(x))
			.AddTo(_compositeDisposable);
	}

	public void SetPause(bool pause)
	{
		_player.SetPause(pause);
	}

	public void OnUpdate(float deltaTime)
	{
		_collectingBehaviour.OnUpdate(deltaTime);

		_player.OnUpdate(deltaTime);
		
		foreach (Building building in m_buildings)
		{
			building.OnUpdate(deltaTime);
		}

		foreach (Storage storage in m_storages)
		{
			storage.OnUpdate(deltaTime);
		}
	}

	private void CollectingOnTransfer(StockCollectingTransfer transfer)
	{
		_collectingBehaviour.Create<StockCollectingInfo>(
			transfer.originInfo,
			transfer.targetInfo,
			transfer.prefab,
			transfer.originInfo.source,
			transfer.targetInfo.source,
			transfer.originInfo.transform,
			transfer.targetInfo.transform,
			transfer.duration
			);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}

	public PlayerData GetSaveData()
	{
		PlayerData playerData = _profile.Get<PlayerData>().data;

		playerData.playerStats = _player.GetSaveData();

		playerData.playerStats.cameraZoom = _vCamera.distanceValue;
		playerData.playerStats.position = _player.transform.position;
		playerData.playerStats.quaternion = _player.transform.rotation;

		playerData.buildings = new PlayerData.BuildingStats[m_buildings.Length];
		for (int i = 0; i < m_buildings.Length; i++)
		{
			playerData.buildings[i] = m_buildings[i].GetSaveData();
		}

		playerData.storages = new PlayerData.StorageStats[m_storages.Length];
		for (int i = 0; i < m_storages.Length; i++)
		{
			playerData.storages[i] = m_storages[i].GetSaveData();
		}

		return playerData;
	}
	public bool SetSaveData(PlayerData data)
	{
		try
		{
			_player.SetOrientation(data.playerStats.position, data.playerStats.quaternion);

			_vCamera.distanceValue = data.playerStats.cameraZoom;
			_vCamera.SetFollowTarget(_player.CameraTarget);

			_player.SetSaveData(data.playerStats);

			for (int i = 0; i < m_buildings.Length; i++)
			{
				m_buildings[i].SetSaveData(data.buildings[i]);
			}

			for (int i = 0; i < m_storages.Length; i++)
			{
				m_storages[i].SetSaveData(data.storages[i]);
			}

			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}
}