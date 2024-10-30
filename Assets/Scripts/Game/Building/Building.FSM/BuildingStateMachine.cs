using YB.HFSM;

namespace GameName.BuildingFSM
{
	public class BuildingStateMachine : StateMachine
	{
		public BuildingStateMachine(params State[] states) : base(states)
		{
		}
	}
}