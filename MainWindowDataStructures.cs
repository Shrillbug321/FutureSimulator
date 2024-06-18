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

		public override string ToString()
		{
			return $" {agentID}	{agentGlobalID}		{(positionChanged?1:0)}	" +
			       $"{currentCapital:0.00}	{(capitalIncreased ? 1 : 0)}		" +
			       $"{ChangeReasonToNumber(increaseReason)}	{(capitalDecreased?1:0)}		" +
			       $"{ChangeReasonToNumber(decreaseReason)}	{(capitalNotChange?1:0)}		" +
			       $"{diseaseSuspendBusiness}	{emergencyHops}";
		}
	}

	private record BusinessActivity
	{
		public int businessID;
		public int businessGlobalID;
		public int type;
		public int emergencyHops;

		public override string ToString()
		{
			return $" {businessID}	{businessGlobalID}		{type}	{emergencyHops}";
		}
	}

	private record DiseaseActivity
	{
		public int diseaseID;
		public int diseaseGlobalID;
		public int emergencyHops;

		public override string ToString()
		{
			return $" {diseaseID}	{diseaseGlobalID}		{emergencyHops}";
		}
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

	private static int ChangeReasonToNumber(ChangeReason reason)
	{
		return reason switch
		{
			ChangeReason.NoChange => 0,
			ChangeReason.AfterBusiness1 => 1,
			ChangeReason.AfterBusiness2 => 2,
			ChangeReason.AfterBusiness3 => 3,
			ChangeReason.Disease => 4,
			_ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
		};
	}
}