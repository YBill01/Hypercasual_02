using GameName.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CoreScope : LifetimeScope
{
	[Space]
	[SerializeField]
	private GameConfigData m_gameConfig;

	protected override void Configure(IContainerBuilder builder)
	{
		builder.RegisterInstance(m_gameConfig);
		builder.RegisterInstance(m_gameConfig.viewData);

		builder.RegisterComponentInHierarchy<UICore>();

		builder.RegisterEntryPoint<CoreFlow>()
			.AsSelf();
	}
}