namespace FutureSimulator;
using static Util;
using static Global;

public class Business
{
	public double InitCapitalThreshold;
	public double InvestedCapital;
	public double IncreaseCapital;
	public double FailureRisc;
	public static List<double> Avaibilities = new();

	public static List<Business> Businesses = 
	[
		new Business
		{
			InitCapitalThreshold = TbToDouble(window.txt_p_b1t1),
			InvestedCapital = TbToDouble(window.txt_p_b1t2),
			IncreaseCapital = TbToInt(window.txt_p_b1t3),
			FailureRisc = TbToDouble(window.txt_p_b1t4),
		},
		new Business
		{
			InitCapitalThreshold = TbToDouble(window.txt_p_b2t1),
			InvestedCapital = TbToDouble(window.txt_p_b2t2),
			IncreaseCapital = TbToInt(window.txt_p_b2t3),
			FailureRisc = TbToDouble(window.txt_p_b2t4),
		},
		new Business
		{
			InitCapitalThreshold = TbToDouble(window.txt_p_b3t1),
			InvestedCapital = TbToDouble(window.txt_p_b3t2),
			IncreaseCapital = TbToInt(window.txt_p_b3t3),
			FailureRisc = TbToDouble(window.txt_p_b3t4),
		},
	];

	public Business()
	{
		Avaibilities =
		[
			TbToDouble(window.txt_p_b1t5), TbToDouble(window.txt_p_b1t5) + TbToDouble(window.txt_p_b2t5),
			TbToDouble(window.txt_p_b1t5) + TbToDouble(window.txt_p_b2t5) + TbToDouble(window.txt_p_b3t5)
		];
	}
}