using System.Windows.Controls;
using static FutureSimulator.Util;

namespace FutureSimulator;

public class RandomWrapper : Random
{
	public static List<double> NumbersFromFile { get; set; } = [];
	private Random random;
	private bool fromFile;
	private int index;

	public RandomWrapper()
	{
		RadioButton radioButton = GetWindowVariable<RadioButton>("rb_custom_seed");
		fromFile = GetWindowVariable<CheckBox>("chb_read_rand_num").IsChecked == true;
		random = radioButton.IsChecked == true ? new Random(TbToInt("custom_seed")) : new Random();
	}

	public new int Next(int minValue, int maxValue)
	{
		if (!fromFile) return random.Next(minValue, maxValue);
		double step = 1.0 / maxValue;
		double value = NumbersFromFile[NumbersFromFile.Count % index++];
		for (int i = 0; i < maxValue; i++)
		{
			if (i * step > value)
				return i;
		}
		return maxValue;
	}
	
	public new double NextDouble()
	{
		return fromFile ? NumbersFromFile[NumbersFromFile.Count % index++] : random.NextDouble();
	}
}