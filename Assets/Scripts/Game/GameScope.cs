using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
	protected override void Configure(IContainerBuilder builder)
	{
		builder.RegisterComponentInHierarchy<VCamera>();
		builder.RegisterComponentInHierarchy<UIControls>();

		builder.RegisterComponentInHierarchy<InputPlayerControl>();

		builder.RegisterComponentInHierarchy<CPlayer>();
		builder.RegisterComponentInHierarchy<CPlayerController>();

		builder.RegisterComponentInHierarchy<Game>();
		builder.RegisterComponentInHierarchy<GameWorld>();

		builder.Register<StockEvents>(Lifetime.Scoped);
		builder.Register<CollectingEvents>(Lifetime.Scoped);
		builder.Register<StockBehaviour>(Lifetime.Scoped);

		builder.RegisterEntryPoint<GameFlow>();
	}
}