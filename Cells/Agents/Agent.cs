namespace FutureSimulator.Cells.Agents;

using static Global;

public class Agent : AgentAbstract
{
	public int Iq;
	public IqState IqState;
	public HealthState HealthState;
	public double Disease;
	public WealthState WealthState = WealthState.Poor;
	public double Mobility;
	public static double InitialCapital;
	public double Capital;
	public int SuspendedCounter = 0;
	public Dictionary<string, double> BusinessesAccept;

	public Agent()
	{
		Iq = (int)Randomizer.Gauss(IqMean, IqRangeMin, IqRangeMax);

		dynamic rand = Randomizer.Next();
		IqState = rand <= IqStateThresholds[IqState.Stupid] ? IqState.Stupid :
			rand <= IqStateThresholds[IqState.Standard] ? IqState.Standard : IqState.Clever;

		rand = Randomizer.NextDouble();
		HealthState = rand <= HealthStateThresholds[HealthState.Weak] ? HealthState.Weak :
			rand <= HealthStateThresholds[HealthState.Standard] ? HealthState.Standard : HealthState.Good;

		Disease = DiseaseThresholds[HealthState];
		Mobility = MobilityThresholds[IqState];
		Capital = InitialCapital;
		
		BusinessesAccept = new Dictionary<string, double>
		{
			{ "Business1", BusinessesAccepts[0][IqState] },
			{ "Business2", BusinessesAccepts[1][IqState] },
			{ "Business3", BusinessesAccepts[2][IqState] }
		};
	}

	public Agent(Cell element) : this()
	{
		Id = element.Id;
		GlobalId = element.GlobalId;
	}

	public void UpdateWealthState()
	{
		double thresholdPoor = WealthThresholds[WealthState.Poor] * InitialCapital;
		double thresholdFair = WealthThresholds[WealthState.Fair] * InitialCapital;
		WealthState = Capital <= thresholdPoor ? WealthState.Poor :
			Capital > thresholdPoor && Capital <= thresholdFair ? WealthState.Fair : WealthState.Rich;
	}

	public bool IsPoor()
	{
		double threshold = WealthThresholds[WealthState.Poor] * InitialCapital;
		return Capital <= threshold;
	}

	public bool IsFair()
	{
		double thresholdPoor = WealthThresholds[WealthState.Poor] * InitialCapital;
		double thresholdFair = WealthThresholds[WealthState.Fair] * InitialCapital;
		return Capital > thresholdPoor && Capital <= thresholdFair;
	}

	//To nie jest ten Rich z Mody na Sukces
	public bool IsRich()
	{
		double threshold = WealthThresholds[WealthState.Fair] * InitialCapital;
		return Capital > threshold;
	}
}