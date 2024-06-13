using System.Windows.Controls;
using static FutureSimulator.Global;

namespace FutureSimulator;

public static class Util
{
	public static string Format(double number)
	{
		return $"{Math.Round(number, 3):f}";
	}

	public static double TbToDouble(TextBox control)
	{
		return double.Parse(control.Text);
	}

	public static double TbToDouble(string control)
	{
		return double.Parse(GetWindowVariable<TextBox>(control).Text);
	}

	public static int TbToInt(TextBox control)
	{
		return int.Parse(control.Text);
	}

	public static int TbToInt(string control)
	{
		return int.Parse(GetWindowVariable<TextBox>(control).Text);
	}

	public static double Gauss(this Random random, double mean, double min, double max)
	{
		double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
		double u2 = 1.0 - random.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
		                       Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		return mean + 14 * randStdNormal; //random normal(mean,stdDev^2)
	}

	// public static int IntFromEnum<T>(string parameter) where T: Enum
	// {
	// 	T a = (T)Enum.Parse(typeof(T), parameter);
	// 	return (int)a;
	// }

	public struct IntPoint
	{
		public int X { get; set; }
		public int Y { get; set; }
	}

	public static T GetWindowVariable<T>(string variable) where T:Control
	{
		object control = variable switch
		{
			"txt_m_rows" => window.txt_m_rows,
			"txt_n_colls" => window.txt_n_colls,
			"txtn_of_A" => window.txtn_of_A,
			"txtn_of_D" => window.txtn_of_D,
			"txtn_of_B" => window.txtn_of_B,
			"txtn_of_iter" => window.txtn_of_iter,
			"txtn_of_exper" => window.txtn_of_exper,
			"init_capt" => window.init_capt,
			"chb_debug" => window.chb_debug,
			"chb_read_ca_states" => window.chb_read_ca_states,
			"chb_read_a_profile" => window.chb_read_a_profile,
			"chb_read_rand_num" => window.chb_read_rand_num,
			"rb_test1" => window.rb_test1,
			"rb_custom_seed" => window.rb_custom_seed,
			"custom_seed" => window.custom_seed,
			"txt_min_iq" => window.txt_min_iq,
			"txt_max_iq" => window.txt_max_iq,
			"txt_p_HS1" => window.txt_p_HS1,
			"txt_p_HS2" => window.txt_p_HS2,
			"txt_p_HS3" => window.txt_p_HS3,
			"txt_p_ill1" => window.txt_p_ill1,
			"txt_p_ill2" => window.txt_p_ill2,
			"txt_p_ill3" => window.txt_p_ill3,
			"txt_n_iter_susp_B" => window.txt_n_iter_susp_B,
			"txt_D_IC_decr_range" => window.txt_D_IC_decr_range,
			"txt_p_iqr" => window.txt_p_iqr,
			"txt_p_iqm" => window.txt_p_iqm,
			"txt_p_b1p1" => window.txt_p_b1p1,
			"txt_p_b1p2" => window.txt_p_b1p2,
			"txt_p_b1p3" => window.txt_p_b1p3,
			"txt_p_b2p1" => window.txt_p_b2p1,
			"txt_p_b2p2" => window.txt_p_b2p2,
			"txt_p_b2p3" => window.txt_p_b2p3,
			"txt_p_b3p1" => window.txt_p_b3p1,
			"txt_p_b3p2" => window.txt_p_b3p2,
			"txt_p_b3p3" => window.txt_p_b3p3,
			"txt_p_pmob1" => window.txt_p_pmob1,
			"txt_p_pmob2" => window.txt_p_pmob2,
			"txt_p_pmob3" => window.txt_p_pmob3,
			"txt_p_b1t1" => window.txt_p_b1t1,
			"txt_p_b1t2" => window.txt_p_b1t2,
			"txt_p_b1t3" => window.txt_p_b1t3,
			"txt_p_b1t4" => window.txt_p_b1t4,
			"txt_p_b1t5" => window.txt_p_b1t5,
			"txt_p_b2t1" => window.txt_p_b2t1,
			"txt_p_b2t2" => window.txt_p_b2t2,
			"txt_p_b2t3" => window.txt_p_b2t3,
			"txt_p_b2t4" => window.txt_p_b2t4,
			"txt_p_b2t5" => window.txt_p_b2t5,
			"txt_p_b3t1" => window.txt_p_b3t1,
			"txt_p_b3t2" => window.txt_p_b3t2,
			"txt_p_b3t3" => window.txt_p_b3t3,
			"txt_p_b3t4" => window.txt_p_b3t4,
			"txt_p_b3t5" => window.txt_p_b3t5,
			"txt_p_poor" => window.txt_p_poor,
			"txt_p_fair" => window.txt_p_fair,
			"txt_p_rich" => window.txt_p_rich,
			"cells_canvas" => window.cells_canvas,
			"chart" => window.chart,
			_ => throw new ArgumentException($"Variable {variable} does not exist.")
		};
		return (T)control;
	}
}