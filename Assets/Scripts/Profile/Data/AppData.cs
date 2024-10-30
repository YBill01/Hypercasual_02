using System;
using UnityEngine.Device;

namespace GameName.Core
{
	[Serializable]
	public class AppData : IProfileData
	{
		internal DateTime firstEntryDate;
		internal DateTime lastEntryDate;

		private string _name;

		internal float musicVolume;
		internal float soundVolume;

		internal bool firstPlay;

		public string Name
		{
			get => _name;
			set
			{
				// validation...
				if (value.Length < 1)
				{
					_name = SystemInfo.deviceName;
				}
				else
				{
					_name = value;
				}
			}
		}

		public void SetDefault()
		{
			firstEntryDate = DateTime.UtcNow;
			lastEntryDate = DateTime.UtcNow;

			Name = "";

			musicVolume = 1.0f;
			soundVolume = 1.0f;

			firstPlay = true;
		}
	}
}