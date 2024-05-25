namespace FutureSimulator.Agents;
using static Util;
using static Global;
public abstract class AgentAbstract
{
	public enum HealthState
	{
		Weak, Standard, Good
	}
	public enum IqState
	{
		Stupid, Standard, Clever
	}
	public enum WealthState
	{
		Poor, Fair, Rich
	}
	
	protected int IterationOnBusiness;
	protected double DecreaseInitCapitalOnDisease;
	protected int IqRangeMin;
	protected int IqRangeMax;
	
	protected Dictionary<HealthState, double> HealthStateThresholds = new();
	protected Dictionary<HealthState, double> IllnessThresholds = new();
	protected Dictionary<IqState, double> IqStateThresholds = new();
	//badziewiaste nazwy, nie pamiętam czym było p_ac
	protected List<Dictionary<IqState, double>> BusinessAccepts = new();
	protected Dictionary<IqState, double> MobilityThresholds = new();
	protected Dictionary<WealthState, double> WealthThresholds = new();

	protected double Mean;
	protected double StdDev;

	protected AgentAbstract()
	{
		GetValuesFromInputs();
		Mean = (double)(IqRangeMax + IqRangeMin) / 2;
		Console.WriteLine(Randomizer.Gauss(Mean, IqRangeMin, IqRangeMax));
		
		//StdDev = 
	}

	private void GetValuesFromInputs()
	{
		IterationOnBusiness = TbToInt("txt_n_iter_susp_B");
		DecreaseInitCapitalOnDisease = TbToDouble("txt_D_IC_decr_range");
		IqRangeMin = TbToInt("txt_min_iq");
		IqRangeMax = TbToInt("txt_max_iq");
		HealthStateThresholds = new Dictionary<HealthState, double>
		{
			[HealthState.Weak] = TbToDouble("txt_p_HS1"),//łatwiej będzie losować stan zdrowia przy zsumowanych progach
			[HealthState.Standard] = TbToDouble("txt_p_HS1")+TbToDouble("txt_p_HS2"),
			[HealthState.Good] = TbToDouble("txt_p_HS1")+TbToDouble("txt_p_HS2")+TbToDouble("txt_p_HS3"),
		};
		IllnessThresholds = new Dictionary<HealthState, double>
		{
			[HealthState.Weak] = TbToDouble("txt_p_ill1"),
			[HealthState.Standard] = TbToDouble("txt_p_ill2"),
			[HealthState.Good] = TbToDouble("txt_p_ill3"),
		};
		IqStateThresholds = new Dictionary<IqState, double>
		{
			[IqState.Stupid] = 0,
			[IqState.Standard] = TbToInt("txt_p_iqr"),
			[IqState.Clever] = TbToInt("txt_p_iqm"),
		};
		BusinessAccepts =
		[
			new Dictionary<IqState, double>
			{
				[IqState.Stupid] = TbToDouble("txt_p_b1p1"),
				[IqState.Standard] = TbToDouble("txt_p_b1p2"),
				[IqState.Clever] = TbToDouble("txt_p_b1p3"),
			},

			new Dictionary<IqState, double>
			{
				[IqState.Stupid] = TbToDouble("txt_p_b2p1"),
				[IqState.Standard] = TbToDouble("txt_p_b2p2"),
				[IqState.Clever] = TbToDouble("txt_p_b2p3"),
			},

			new Dictionary<IqState, double>
			{
				[IqState.Stupid] = TbToDouble("txt_p_b3p1"),
				[IqState.Standard] = TbToDouble("txt_p_b3p2"),
				[IqState.Clever] = TbToDouble("txt_p_b3p3"),
			}
		];
		MobilityThresholds = new Dictionary<IqState, double>
		{
			[IqState.Stupid] = TbToDouble("txt_p_pmob1"),
			[IqState.Standard] = TbToDouble("txt_p_pmob2"),
			[IqState.Clever] = TbToDouble("txt_p_pmob3"),
		};
		WealthThresholds = new Dictionary<WealthState, double>
		{
			[WealthState.Poor] = TbToInt("txt_p_poor"),
			[WealthState.Fair] = TbToInt("txt_p_fair"),
			[WealthState.Rich] = TbToInt("txt_p_rich")
		};
	}
}