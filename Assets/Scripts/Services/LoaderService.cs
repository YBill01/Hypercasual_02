using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoaderService : IService
{
	public Action<float> LoadProgress;
	public Action LoadComplete;
	
	public Action<float> LoadProgressAsync;
	public Action<SceneInstance> LoadCompleteAsync;

	public int IndexScene { get; private set; }

	public LoaderState State { get; private set; }
	public enum LoaderState
	{
		None,
		Loading,
		Completed
	}

	public struct SceneLoadData
	{
		public AssetReference sceneReference;
		public LoadSceneMode loadSceneMode;

		public Action<float> LoadProgress;
		public Action<SceneInstance> LoadComplete;
	}

	public void Clear()
	{
		State = LoaderState.None;
		IndexScene = -1;
	}

	public bool IsValid()
	{
		if (State == LoaderState.Loading)
		{
			Debug.LogError($"Loader is already using {LoaderState.Loading} state.");

			return false;
		}

		return true;
	}

	public LoaderService LoadAddressableScene(SceneLoadData[] sceneLoadData)
	{
		LoadAddressableSceneAsync(sceneLoadData).Forget();

		return this;
	}
	private async UniTask LoadAddressableSceneAsync(SceneLoadData[] sceneLoadData)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		for (int i = 0; i < sceneLoadData.Length; i++)
		{
			var handle = Addressables.LoadSceneAsync(sceneLoadData[i].sceneReference, sceneLoadData[i].loadSceneMode);
			await handle.ToUniTask(Progress.Create<float>(v => LoadProgress?.Invoke(v)));
			
			sceneLoadData[i].LoadComplete?.Invoke(handle.Result);
		}

		await UniTask.NextFrame();

		Clear();
	}

	public LoaderService LoadAddressableScene(AssetReference sceneReference, LoadSceneMode loadSceneMode)
	{
		if (!IsValid())
		{
			return this;
		}
		State = LoaderState.Loading;

		LoadAddressableSceneAsync(sceneReference, loadSceneMode).Forget();

		return this;
	}
	private async UniTask LoadAddressableSceneAsync(AssetReference sceneReference, LoadSceneMode loadSceneMode)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		var handle = Addressables.LoadSceneAsync(sceneReference, loadSceneMode);
		await handle.ToUniTask(Progress.Create<float>(v => LoadProgressAsync?.Invoke(v)));

		await UniTask.NextFrame();

		State = LoaderState.Completed;

		LoadCompleteAsync?.Invoke(handle.Result);

		Clear();

		LoadProgressAsync = null;
		LoadCompleteAsync = null;
	}

	public LoaderService UnloadAddressableScene(SceneInstance sceneInstance)
	{
		if (!sceneInstance.Scene.isLoaded)
		{
			return this;
		}

		UnloadAddressableSceneAsync(sceneInstance).Forget();

		return this;
	}
	private async UniTask UnloadAddressableSceneAsync(SceneInstance sceneInstance)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		await Addressables.UnloadSceneAsync(sceneInstance);
	}

	public LoaderService LoadScene(int indexScene, float delay = 0.0f)
	{
		if (!IsValid())
		{
			return this;
		}
		State = LoaderState.Loading;

		LoadSceneAsync(indexScene, delay).Forget();

		return this;
	}
	private async UniTask LoadSceneAsync(int indexScene, float delay = 0.0f)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		await SceneManager.LoadSceneAsync(indexScene)
			.ToUniTask(Progress.Create<float>(v => LoadProgress?.Invoke(v)));

		await UniTask.WaitForSeconds(delay);
		await UniTask.NextFrame();

		State = LoaderState.Completed;

		LoadComplete?.Invoke();

		Clear();

		LoadProgress = null;
		LoadComplete = null;
	}
}

public static class LoaderServiceExtensions
{
	public static T OnProgress<T>(this T t, Action<float> action) where T : LoaderService
	{
		t.LoadProgress = action;

		return t;
	}
	public static T OnComplete<T>(this T t, Action action) where T : LoaderService
	{
		t.LoadComplete = action;

		return t;
	}

	public static T OnProgressAsync<T>(this T t, Action<float> action) where T : LoaderService
	{
		t.LoadProgressAsync = action;

		return t;
	}
	public static T OnCompleteAsync<T>(this T t, Action<SceneInstance> action) where T : LoaderService
	{
		t.LoadCompleteAsync = action;

		return t;
	}
}