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

	public static double RandomGauss(double mean, double min, double max)
	{
		Random rand = new(); //reuse this if you are generating many
		double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
		double u2 = 1.0-rand.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
		                       Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		double randNormal =
			mean + 14 * randStdNormal; //random normal(mean,stdDev^2)
		double std = (randNormal - min) / (max - min);
		return std*(max-min)+min;
	}
	
	// public static int IntFromEnum<T>(string parameter) where T: Enum
	// {
	// 	T a = (T)Enum.Parse(typeof(T), parameter);
	// 	return (int)a;
	// }

	public struct IntPoint 
	{
		public int X {get;set;}
		public int Y {get;set;}
	}
}