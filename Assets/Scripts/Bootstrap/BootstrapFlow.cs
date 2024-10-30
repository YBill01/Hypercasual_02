using GameName.Data;
using GameName.Core;
using System;
using UnityEngine;
using VContainer.Unity;

public class BootstrapFlow : IStartable
{
	private LoaderService _loaderService;
	private ScenesData _scenes;
	private Profile _profile;

	public BootstrapFlow(
		LoaderService loaderService,
		ScenesData scenes,
		Profile profile)
	{
		_loaderService = loaderService;
		_scenes = scenes;
		_profile = profile;
	}

	public void Start()
	{
		AppData appData = _profile.Get<AppData>().Load();
		appData.lastEntryDate = DateTime.UtcNow;
		PlayerData playerData = _profile.Get<PlayerData>().Load();

		ScenesData.Scene scene = _scenes.GetSceneDataAt(ScenesData.SceneId.CORE);
		_loaderService.LoadAddressableScene(scene.sceneAsset, scene.loadMode)
			.OnComplete(() =>
			{
				Debug.Log($"Loading scene: {scene.id} - complete");
			});
	}
}