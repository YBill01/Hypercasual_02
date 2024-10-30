using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using System;
using System.Collections.Generic;

namespace GameName.Core
{
	public class Profile
	{
		private Dictionary<Type, IProfileController> _profileDic;

		public Profile()
		{
			SaveGame.Serializer = new SaveGameBinarySerializer();

			_profileDic = new Dictionary<Type, IProfileController>();

			_profileDic.Add(typeof(AppData), new ProfileController<AppData>("_app.dat"));
			_profileDic.Add(typeof(PlayerData), new ProfileController<PlayerData>("_player.dat"));
			//...
		}

		public ProfileController<T> Get<T>() where T : class, IProfileData, new()
		{
			if (_profileDic.TryGetValue(typeof(T), out IProfileController profileData))
			{
				return (ProfileController<T>)profileData;
			}

			return default;
		}
	}
}