using static FutureSimulator.Util;
namespace FutureSimulator.Business;

public class Business1: BusinessAbstract
{
	public static double Availability { get; set; }

	public Business1()
	{
		Availability = TbToDouble("txt_p_b1t5");
	}
}

public class Business2: BusinessAbstract
{
	public static double Availability { get; set; }

	public Business2()
	{
		Availability = TbToDouble("txt_p_b1t5") + TbToDouble("txt_p_b2t5");
	}
}

public class Business3: BusinessAbstract
{
	public static double Availability { get; set; }

	public Business3()
	{
		Availability = TbToDouble("txt_p_b1t5") + TbToDouble("txt_p_b2t5") + TbToDouble("txt_p_b3t5");
	}
}