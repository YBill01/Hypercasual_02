using GameName.Data;
using GameName.Core;
using VContainer.Unity;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.SceneManagement;

public class CoreFlow : IStartable
{
	public event Action<CoreState> OnStatePreChange;
	public event Action<CoreState> OnStateChanged;
	public event Action<bool> OnPauseChanged;

	private App _app;
	private LoaderService _loaderService;
	private ScenesData _scenes;
	private Profile _profile;
	private GameConfigData _gameConfig;

	private SceneInstance _sceneGame;

	public CoreState State { get; private set; }

	public enum CoreState
	{
		Meta,
		Game
	}

	public CoreFlow(
		App app,
		LoaderService loaderService,
		ScenesData scenes,
		Profile profile,
		GameConfigData gameConfig)
	{
		_app = app;
		_loaderService = loaderService;
		_scenes = scenes;
		_profile = profile;
		_gameConfig = gameConfig;
	}

	public void Start()
	{
		_gameConfig.viewData.HashingData();

		State = CoreState.Meta;
	}

	public void InitConfig(bool reset)
	{
		AppData appData = _profile.Get<AppData>().data;
		appData.lastEntryDate = DateTime.UtcNow;
		
		if (appData.firstPlay || reset)
		{
			_profile.Get<PlayerData>().Clear();

			PlayerData playerData = _profile.Get<PlayerData>().data;

			playerData.playerStats.position = _gameConfig.startPlayerPosition;
			playerData.playerStats.quaternion = Quaternion.AngleAxis(_gameConfig.startPlayerRotation, Vector3.up);
			playerData.playerStats.cameraZoom = _gameConfig.startPlayerCameraZoom;

			appData.firstPlay = false;
		}
	}

	public void StartGame()
	{
		OnStatePreChange?.Invoke(State);

		State = CoreState.Game;

		ScenesData.Scene scene = _scenes.GetSceneDataAt(ScenesData.SceneId.GAME);
		_loaderService.LoadAddressableScene(scene.sceneAsset, scene.loadMode)
			.OnCompleteAsync((s) =>
			{
				_sceneGame = s;
				SceneManager.SetActiveScene(_sceneGame.Scene);

				OnStateChanged?.Invoke(State);

				Debug.Log($"Loading scene: {scene.id} - complete");
			});
	}
	public void EndGame()
	{
		OnStatePreChange?.Invoke(State);

		State = CoreState.Meta;

		_loaderService.UnloadAddressableScene(_sceneGame);

		OnStateChanged?.Invoke(State);
	}

	public void SetPauseValue(bool value)
	{
		OnPauseChanged?.Invoke(value);
	}
}