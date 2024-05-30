namespace FutureSimulator;

public partial class MainWindow
{
	private record AgentActivity
	{
		private int agentID;
		private int agentGlobalID;
		private bool positionChanged;
		private double currentCapital;
		private bool capitalIncreased;
		private ChangeReason increaseReason;
		private bool capitalDecreased;
		private ChangeReason decreaseReason;
		private bool capitalNotChange;
		private bool diseaseSuspendBusiness;
		private int emergencyHops;
	}

	private record BusinessActivity
	{
		private int businessID;
		private int businessGlobalID;
		private int type;
		private int emergencyHops;
	}

	private record DiseaseActivity
	{
		private int diseaseID;
		private int diseaseGlobalID;
		private int emergencyHops;
	}

	public enum CAState
	{
		Empty,
		Agent,
		Disease,
		Business1,
		Business2,
		Business3
	}

	private enum CellPosition
	{
		TopLeftCorner,
		TopRightCorner,
		BottomLeftCorner,
		BottomRightCorner,
		TopEdge,
		RightEdge,
		BottomEdge,
		LeftEdge,
		Inside
	}

	private enum ChangeReason
	{
		NoChange,
		AfterBusiness1,
		AfterBusiness2,
		AfterBusiness3,
		Disease
	}
}