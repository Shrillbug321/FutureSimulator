using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace FutureSimulator;

public static class Util
{
	public static bool[] ToBinArray(string bin)
	{
		int numOfBytes = bin.Length;
		bool[] bytes = new bool[numOfBytes];
		for (int i = 0; i < numOfBytes; ++i)
		{
			bytes[i] = bin[i] == '1';
		}

		return bytes;
	}

	public static string Format(double number)
	{
		return $"{Math.Round(number, 3):f}".Replace(',', '.');
	}

	public static double TbToDouble(TextBox control)
	{
		return double.Parse(control.Text.Replace('.', ','));
	}

	public static int TbToInt(TextBox control)
	{
		return int.Parse(control.Text);
	}
}