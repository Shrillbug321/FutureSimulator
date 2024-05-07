namespace FutureSimulator;
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
	//badziewiaste nazwy, nie pamiętam czym było p_ac = new()c
	protected List<Dictionary<IqState, double>> BusinessAccepts = new();
	protected Dictionary<IqState, double> MobilityThresholds = new();
	protected Dictionary<WealthState, double> WealthThresholds = new();

	protected double Mean;
	protected double StdDev;

	protected AgentAbstract()
	{
		GetValuesFromInputs();
		Mean = (double)(IqRangeMax + IqRangeMin) / 2;
		Console.WriteLine(RandomGauss(Mean, IqRangeMin, IqRangeMax));
		
		//StdDev = 
	}

	private void GetValuesFromInputs()
	{
		IterationOnBusiness = TbToInt(window.txt_n_iter_susp_B);
		DecreaseInitCapitalOnDisease = TbToDouble(window.txt_D_IC_decr_range);
		IqRangeMin = TbToInt(window.txt_min_iq);
		IqRangeMax = TbToInt(window.txt_max_iq);
		HealthStateThresholds = new Dictionary<HealthState, double>
		{
			[HealthState.Weak] = TbToDouble(window.txt_p_HS1),//łatwiej będzie losować stan zdrowia przy zsumowanych prograch
			[HealthState.Standard] = TbToDouble(window.txt_p_HS1)+TbToDouble(window.txt_p_HS2),
			[HealthState.Good] = TbToDouble(window.txt_p_HS1)+TbToDouble(window.txt_p_HS2)+TbToDouble(window.txt_p_HS3),
		};
		IllnessThresholds = new Dictionary<HealthState, double>
		{
			[HealthState.Weak] = TbToDouble(window.txt_p_ill1),
			[HealthState.Standard] = TbToDouble(window.txt_p_ill2),
			[HealthState.Good] = TbToDouble(window.txt_p_ill3),
		};
		IqStateThresholds = new Dictionary<IqState, double>
		{
			[IqState.Stupid] = 0,
			[IqState.Standard] = TbToInt(window.txt_p_iqr),
			[IqState.Clever] = TbToInt(window.txt_p_iqm),
		};
		BusinessAccepts = new List<Dictionary<IqState, double>>
		{
			new()
			{
				[IqState.Stupid] = TbToDouble(window.txt_p_b1p1),
				[IqState.Standard] = TbToDouble(window.txt_p_b1p2),
				[IqState.Clever] = TbToDouble(window.txt_p_b1p3),
			},
			new()
			{
				[IqState.Stupid] = TbToDouble(window.txt_p_b2p1),
				[IqState.Standard] = TbToDouble(window.txt_p_b2p2),
				[IqState.Clever] = TbToDouble(window.txt_p_b2p3),
			},
			new()
			{
				[IqState.Stupid] = TbToDouble(window.txt_p_b3p1),
				[IqState.Standard] = TbToDouble(window.txt_p_b3p2),
				[IqState.Clever] = TbToDouble(window.txt_p_b3p3),
			},
		};
		MobilityThresholds = new Dictionary<IqState, double>
		{
			[IqState.Stupid] = TbToDouble(window.txt_p_pmob1),
			[IqState.Standard] = TbToDouble(window.txt_p_pmob2),
			[IqState.Clever] = TbToDouble(window.txt_p_pmob3),
		};
		WealthThresholds = new Dictionary<WealthState, double>
		{
			[WealthState.Poor] = TbToInt(window.txt_p_poor),
			[WealthState.Fair] = TbToInt(window.txt_p_fair),
			[WealthState.Rich] = TbToInt(window.txt_p_rich)
		};
	}
}