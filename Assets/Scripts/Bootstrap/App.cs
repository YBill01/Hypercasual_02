using UnityEngine;
using VContainer;

namespace GameName.Core
{
	public class App : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize()
		{
			Application.targetFrameRate = 60;
			Application.runInBackground = true;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		private Profile _profile;

		[Inject]
		public void Construct(Profile profile)
		{
			_profile = profile;
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public void ProfileSave()
		{
			_profile.Get<AppData>().Save();
			_profile.Get<PlayerData>().Save();
		}

		private void OnDestroy()
		{
#if UNITY_EDITOR
			ProfileSave();
#endif
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
			{
				ProfileSave();
			}
		}

		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}