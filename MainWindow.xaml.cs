using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace FutureSimulator;
using static FutureSimulator.Util;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private int rows;
	private int columns;
	private double cellWidth;
	private double cellHeight;
	private int[,] caStates;
	private SeriesCollection chartSeries;

	public MainWindow()
	{
		InitializeComponent();
		rows = TbToInt(txtMRows);
		columns = TbToInt(txtNColls);
		cellWidth = CellsCanvas.Width / columns;
		cellHeight = CellsCanvas.Height / rows;

		caStates = new int[rows, columns];

		// Fill the array with zeros
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				caStates[i, j] = 0;
			}
		}

		DrawCaStatesCanvas(caStates);
		InitializeChart();
	}

	private void ReadCAStates_Checked(object sender, RoutedEventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		if ((bool)checkBox.IsChecked)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (openFileDialog.ShowDialog() == true)
			{
				string selectedFilePath = openFileDialog.FileName;
				string[] lines = File.ReadAllLines(selectedFilePath);

				caStates = new int[lines.Length, lines[0].Split(' ').Length];
				for (int i = 0; i < lines.Length; i++)
				{
					string[] values = lines[i].Split(' ');
					for (int j = 0; j < values.Length; j++)
					{
						caStates[i, j] = int.Parse(values[j]);
					}
				}

				DrawCaStatesCanvas(caStates);
			}
		}
	}

	private void DrawCaStatesCanvas(int[,] caStates)
	{
		CellsCanvas.Children.Clear();
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				Rectangle r = new()
				{
					Width = cellWidth,
					Height = cellHeight,
					Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0))
				};

				r.Fill = caStates[i, j] switch
				{
					0 => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
					1 =>
						// There will be new statement about pauper, fair, rich later but now every one is poor
						new SolidColorBrush(Color.FromRgb(255, 255, 0)),
					2 => new SolidColorBrush(Color.FromRgb(238, 130, 238)),
					3 => new SolidColorBrush(Color.FromRgb(0, 0, 255)),
					4 => new SolidColorBrush(Color.FromRgb(0, 0, 205)),
					5 => new SolidColorBrush(Color.FromRgb(0, 0, 139)),
					_ => r.Fill
				};

				Canvas.SetTop(r, j * cellHeight);
				Canvas.SetLeft(r, i * cellWidth);
				CellsCanvas.Children.Add(r);
			}
		}
	}

	private void InitializeChart()
	{
		//Define chart series
		chartSeries =
		[
			new LineSeries
			{
				Title = "Poor", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("blue"))
			},
			new LineSeries
			{
				Title = "Fair", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("green"))
			},
			new LineSeries
			{
				Title = "Rich", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("red"))
			},
			new LineSeries
			{
				Title = "Init Capital", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("black"))
			}
		];
		Chart.Series = chartSeries;
		Chart.ChartLegend = new DefaultLegend();
		
		//Sample data
		chartSeries[0].Values.Add(12.0);
		chartSeries[1].Values.Add(14.0);
		chartSeries[2].Values.Add(18.0);
		chartSeries[3].Values.Add(Util.TbToDouble(init_capt));
	}
}