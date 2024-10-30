using GameName.Core;
using System;
using VContainer;
using VContainer.Unity;

public class GameFlow : IStartable, IPostStartable, IDisposable
{
	private Profile _profile;
	private CoreFlow _coreFlow;
	private VCamera _vCamera;
	private CPlayer _player;
	private Game _game;
	private GameWorld _gameWorld;

	private IObjectResolver _resolver;

	public GameFlow(
		IObjectResolver resolver,
		Profile profile,
		CoreFlow coreFlow,
		VCamera vCamera,
		CPlayer player,
		Game game,
		GameWorld gameWorld)
	{
		_resolver = resolver;

		_profile = profile;
		_coreFlow = coreFlow;
		_vCamera = vCamera;
		_player = player;
		_game = game;
		_gameWorld = gameWorld;
	}

	public void Start()
	{
		PlayerData playerData = _profile.Get<PlayerData>().data;

		_vCamera.distanceValue = playerData.playerStats.cameraZoom;

		_coreFlow.OnStatePreChange += CoreFlowOnStatePreChange;
		_coreFlow.OnPauseChanged += CoreFlowOnPauseChanged;
	}

	public void PostStart()
	{
		_game.StartGame().Forget();
	}

	private void CoreFlowOnStatePreChange(CoreFlow.CoreState state)
	{
		if (state == CoreFlow.CoreState.Game)
		{
			SaveStats();
		}
	}
	private void CoreFlowOnPauseChanged(bool value)
	{
		_game.SetPause(value);
	}

	private void SaveStats()
	{
		PlayerData playerData = _gameWorld.GetSaveData();
	}

	public void Dispose()
	{
		_coreFlow.OnStatePreChange -= CoreFlowOnStatePreChange;
		_coreFlow.OnPauseChanged -= CoreFlowOnPauseChanged;
	}
}