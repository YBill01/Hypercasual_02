using BayatGames.SaveGameFree.Types;
using System;

namespace GameName.Core
{
	[Serializable]
	public class PlayerData : IProfileData
	{
		public DateTime localTime;

		public PlayerStats playerStats;

		public BuildingStats[] buildings;
		public StorageStats[] storages;

		public void SetDefault()
		{
			localTime = new DateTime();

			playerStats = new()
			{
				stock = new()
				{
					slots = {},
					transferHandles = {}
				}
			};

			buildings = Array.Empty<BuildingStats>();
			storages = Array.Empty<StorageStats>();
		}

		[Serializable]
		public class PlayerStats
		{
			public Vector3Save position;
			public QuaternionSave quaternion;
			public float cameraZoom;

			public Stock stock;
		}

		[Serializable]
		public class BuildingStats
		{
			public Stock stock;
			public Production production;
		}

		[Serializable]
		public class StorageStats
		{
			public Stock stock;
		}

		[Serializable]
		public class Stock
		{
			public StockSlot[] slots;
			public StockTransferHandle[] transferHandles;
		}
		[Serializable]
		public class StockSlot
		{
			public StockItem[] items;
		}
		[Serializable]
		public class StockItem
		{
			public int type;
			public bool inactive;
		}
		[Serializable]
		public class StockTransferHandle
		{
			public StockItem stockItem;

			public int slotIndex;
			public int itemIndex;

			public float time;
			public float duration;
		}

		[Serializable]
		public class Production
		{
			public float time;
			public float duration;

			public StockItem[] rewards;
		}
	}
}