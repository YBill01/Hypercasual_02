using R3;
using YB.HFSM;

namespace GameName.BuildingFSM
{
	public class ProductionState : State
	{
		private ProductionBehaviour _productionBehaviour;

		private BuildingIcon _icon;

		private CompositeDisposable _compositeDisposable;

		public ProductionState(ProductionBehaviour productionBehaviour, BuildingIcon icon)
		{
			_productionBehaviour = productionBehaviour;

			_icon = icon;
		}

		protected override void OnEnter()
		{
			_compositeDisposable = new CompositeDisposable();

			_productionBehaviour.ProductionHandle.Progress
				.Subscribe(value => ProductionHandleProgress(value))
				.AddTo(_compositeDisposable);

		}
		protected override void OnExit()
		{
			_compositeDisposable.Dispose();
		}

		private void ProductionHandleProgress(float value)
		{
			_icon.Progress(value);

			_productionBehaviour.OnProductionProgress.OnNext(value);
		}
	}
}