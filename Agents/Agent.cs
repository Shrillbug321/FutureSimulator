namespace FutureSimulator.Agents;
using static Global;
public class Agent:AgentAbstract
{
	public int Id;
	public int GlobalId;
	public int Iq;
	public IqState IqState;
	public HealthState HealthState;
	public double Illness;
	public WealthState WealthState = WealthState.Poor;
	public double Mobility;

	public Agent()
	{
		Iq = (int)Randomizer.Gauss(Mean, IqRangeMin, IqRangeMax);
		
		dynamic rand = Randomizer.Next();
		IqState = rand <= IqStateThresholds[IqState.Stupid] ? IqState.Stupid :
			rand <= IqStateThresholds[IqState.Standard] ? IqState.Standard : IqState.Clever;
		
		rand = Randomizer.NextDouble();
		HealthState = rand <= HealthStateThresholds[HealthState.Weak] ? HealthState.Weak :
			rand <= HealthStateThresholds[HealthState.Standard] ? HealthState.Standard : HealthState.Good;
		
		Illness = IllnessThresholds[HealthState];
		Mobility = MobilityThresholds[IqState];
	}
}