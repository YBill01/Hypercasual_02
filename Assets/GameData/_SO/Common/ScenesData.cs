using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Common/ScenesData", fileName = "Scenes", order = 0)]
	public class ScenesData : ScriptableObject
	{
		public enum SceneId
		{
			EMPTY	= 0,
			CORE	= 1,
			GAME	= 2
		}

		public Scene[] scenes;

		[Serializable]
		public struct Scene
		{
			public SceneId id;

			public AssetReferenceT<UnityEngine.Object> sceneAsset;
			public LoadSceneMode loadMode;
		}

		public Scene GetSceneDataAt(SceneId id)
		{
			return scenes.Where(scene => scene.id == id)
				.FirstOrDefault();
		}
	}
}