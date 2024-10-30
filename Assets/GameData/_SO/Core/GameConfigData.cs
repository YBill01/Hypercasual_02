using UnityEngine;

namespace GameName.Data
{
	[CreateAssetMenu(menuName = "GameName/Core/GameConfigData", fileName = "GameConfig", order = 5)]
	public class GameConfigData : ScriptableObject
	{
		public SharedViewData viewData;

		[Space]
		public Vector3 startPlayerPosition;
		public float startPlayerRotation;
		[Range(0.0f, 1.0f)]
		public float startPlayerCameraZoom = 0.5f;



		//[Space]
		//public PlayerConfigData player;

		//[Space]
		//public BuildingConfigData[] buildings;
		
		//[Space]
		//public StorageConfigData[] storages;
	}
}