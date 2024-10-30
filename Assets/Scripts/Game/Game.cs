using Cysharp.Threading.Tasks;
using GameName.Core;
using GameName.Data;
using UnityEngine;
using VContainer;

public class Game : MonoBehaviour, IPausable
{
	private bool _isPlayGame = false;
	public bool IsPlayGame => _isPlayGame;

	private bool _isPaused = false;
	public bool IsPaused => _isPaused;

	private Profile _profile;
	private GameConfigData _gameConfig;
	private GameWorld _gameWorld;

	[Inject]
	public void Construct(
		Profile profile,
		GameConfigData gameConfig,
		GameWorld gameWorld)
	{
		_profile = profile;
		_gameConfig = gameConfig;
		_gameWorld = gameWorld;
	}

	private void Update()
	{
		if (!_isPlayGame || _isPaused)
		{
			return;
		}

		float deltaTime = Time.deltaTime;

		_gameWorld.OnUpdate(deltaTime);
	}

	public async UniTaskVoid StartGame()
	{
		await UniTask.NextFrame();

		_gameWorld.SetSaveData(_profile.Get<PlayerData>().data);
		
		await UniTask.NextFrame();

		_isPlayGame = true;
	}
	public void SetPause(bool pause)
	{
		_isPaused = pause;

		_gameWorld.SetPause(pause);
	}
	public void EndGame()
	{
		_isPlayGame = false;
	}
}