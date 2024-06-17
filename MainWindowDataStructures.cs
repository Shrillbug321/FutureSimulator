namespace FutureSimulator;

public partial class MainWindow
{
	private record AgentActivity
	{
		public int agentID;
		public int agentGlobalID;
		public bool positionChanged;
		public double currentCapital;
		public bool capitalIncreased;
		public ChangeReason increaseReason;
		public bool capitalDecreased;
		public ChangeReason decreaseReason;
		public bool capitalNotChange;
		public int diseaseSuspendBusiness;
		public int emergencyHops;
	}

	private record BusinessActivity
	{
		public int businessID;
		public int businessGlobalID;
		public int type;
		public int emergencyHops;
	}

	private record DiseaseActivity
	{
		public int diseaseID;
		public int diseaseGlobalID;
		public int emergencyHops;
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