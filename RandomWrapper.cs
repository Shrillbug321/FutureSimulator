using System.Diagnostics;
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
		index = 0;
	}

	public new int Next(int minValue, int maxValue)
	{
        if (!fromFile) return random.Next(minValue, maxValue);
		double step = 1.0 / maxValue;

        if (index >= NumbersFromFile.Count)
            index = 0;
        double value = NumbersFromFile[index];
		index++;

        for (int i = 0; i < maxValue; i++)
		{
			if (i * step > value)
                return i;
		}

		return maxValue - 1;
	}
	
	public new double NextDouble()
	{
        if (!fromFile || NumbersFromFile.Count == 0)
            return random.NextDouble();
        if (index >= NumbersFromFile.Count)
            index = 0;
        double value = NumbersFromFile[index];
		index++;

        return value;
    }
}