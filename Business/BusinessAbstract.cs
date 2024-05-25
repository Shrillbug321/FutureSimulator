using System.Windows.Controls;
using static FutureSimulator.Global;
using static FutureSimulator.Util;
namespace FutureSimulator.Business;

public abstract class BusinessAbstract
{
	public double InitCapitalThreshold;
	public double InvestedCapital;
	public int IncreaseCapital;
	public double FailureRisc;

	private Dictionary<string, List<double>> Values = new()
	{
		{"Business1", [
			TbToDouble("txt_p_b1t1"),
			TbToDouble("txt_p_b1t2"),
			TbToInt("txt_p_b1t3"),
			TbToDouble("txt_p_b1t4"),
			TbToDouble("txt_p_b1t5"),
		]},
		{"Business2", [
			TbToDouble("txt_p_b2t1"),
			TbToDouble("txt_p_b2t2"),
			TbToInt("txt_p_b2t3"),
			TbToDouble("txt_p_b2t4"),
			TbToDouble("txt_p_b1t5")+TbToDouble("txt_p_b2t5")
		]},
		{"Business3", [
			TbToDouble("txt_p_b3t1"),
			TbToDouble("txt_p_b3t2"),
			TbToInt("txt_p_b3t3"),
			TbToDouble("txt_p_b3t4"),
			TbToDouble("txt_p_b1t5")+TbToDouble("txt_p_b2t5")+TbToDouble("txt_p_b3t5")
		]}
	};

	protected BusinessAbstract()
	{
		InitCapitalThreshold = Values[GetType().Name][0];
		InvestedCapital = Values[GetType().Name][1];
		IncreaseCapital = (int)Values[GetType().Name][2];
		FailureRisc = Values[GetType().Name][3];
	}
}