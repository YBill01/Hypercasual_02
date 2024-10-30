using GameName.Core;
using GameName.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BootstrapScope : LifetimeScope
{
	[Space]
	[SerializeField]
	private App m_app;
	
	[SerializeField]
	private ScenesData m_scenes;

	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(this);
	}

	protected override void Configure(IContainerBuilder builder)
	{
		builder.Register<LoaderService>(Lifetime.Singleton);

		builder.RegisterComponent(m_app);

		builder.RegisterInstance(m_scenes);

		builder.Register<Profile>(Lifetime.Singleton);

		builder.RegisterEntryPoint<BootstrapFlow>();
	}
}