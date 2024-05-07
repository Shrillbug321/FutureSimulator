using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace FutureSimulator;

using static Util;
using static Global;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private int rows;
	private int columns;
	private double cellWidth;
	private double cellHeight;
	private CAState[,] caStates;
	private SeriesCollection chartSeries;
	private List<Agent> agents;
	private List<int> occupiedCells = [];
	private int cells;

	public MainWindow()
	{
		InitializeComponent();
		caStates = InitializeCAState(caStates);
		DrawCaStatesCanvas(caStates);
		InitializeChart();
		Randomizer = rb_custom_seed.IsChecked == true
			? new Random(TbToInt(window.custom_seed))
			: new Random();
		window = this;
		/*for (int i = 0; i < 100; i++)
		{
			new Agent();
		}*/
	}

	private CAState[,] InitializeCAState(CAState[,] caStates)
	{
		rows = TbToInt(txtMRows);
		columns = TbToInt(txtNColls);
		cellWidth = CellsCanvas.Width / columns;
		cellHeight = CellsCanvas.Height / rows;
		cells = rows * columns;
		caStates = new CAState[rows, columns];

		// Fill the array with zeros
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				caStates[i, j] = CAState.Empty;
			}
		}

		return caStates;
	}

	private void ReadCAStates_Checked(object sender, RoutedEventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		if ((bool)checkBox.IsChecked)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (openFileDialog.ShowDialog() == true)
			{
				string selectedFilePath = openFileDialog.FileName;
				string[] lines = File.ReadAllLines(selectedFilePath);

				caStates = new CAState[lines.Length, lines[0].Split(' ').Length];
				for (int i = 0; i < lines.Length; i++)
				{
					string[] values = lines[i].Split(' ');
					for (int j = 0; j < values.Length; j++)
					{
						caStates[i, j] = (CAState)Enum.Parse(typeof(CAState), values[j]);
					}
				}

				DrawCaStatesCanvas(caStates);
			}
		}
	}

	// private void UpdateCAState()
	// {
	// 	for (int i = 0; i < rows; i++)
	// 	{
	// 		for (int j = 0; j < columns; j++)
	// 		{
	// 			caStates[i, j] = (CAState)Enum.Parse(typeof(CAState), values[j]);
	// 		}
	// 	}
	//
	// 	DrawCaStatesCanvas(caStates);
	// }

	private void DrawCaStatesCanvas(CAState[,] caStates)
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
					CAState.Empty => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
					CAState.Agent =>
						// There will be new statement about pauper, fair, rich later but now every one is poor
						new SolidColorBrush(Color.FromRgb(255, 255, 0)),
					CAState.Disease => new SolidColorBrush(Color.FromRgb(238, 130, 238)),
					CAState.Business1 => new SolidColorBrush(Color.FromRgb(50, 60, 255)),
					CAState.Business2 => new SolidColorBrush(Color.FromRgb(0, 0, 155)),
					CAState.Business3 => new SolidColorBrush(Color.FromRgb(0, 0, 79)),
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
		chartSeries[3].Values.Add(TbToDouble(init_capt));
	}

	private enum CAState
	{
		Empty,
		Agent,
		Disease,
		Business1,
		Business2,
		Business3
	}

	private void StartButton_OnClick(object sender, RoutedEventArgs e)
	{
		Randomizer = rb_custom_seed.IsChecked == true
			? new Random(TbToInt(window.custom_seed))
			: new Random();
		agents = [];
		caStates = InitializeCAState(caStates);
		Console.WriteLine("===");
		for (int i = 0; i < TbToInt(txtn_of_A); i++)
		{
			int cell = Randomizer.Next(0, cells);

			while (occupiedCells.Contains(cell))
				cell = Randomizer.Next(0, cells);

			occupiedCells.Add(cell);

			agents.Add(new Agent
			{
				Id = i, GlobalId = cell
			});
			IntPoint p = Convert1DPointTo2D(cell);
			caStates[p.X, p.Y] = CAState.Agent;
		}

		for (int i = 0; i < TbToInt(txtn_of_D); i++)
		{
			int cell = Randomizer.Next(0, cells);

			while (occupiedCells.Contains(cell))
				cell = Randomizer.Next(0, cells);

			occupiedCells.Add(cell);

			IntPoint p = Convert1DPointTo2D(cell);
			caStates[p.X, p.Y] = CAState.Disease;
		}

		for (int i = 0; i < TbToInt(txtn_of_B); i++)
		{
			int cell = Randomizer.Next(0, cells);

			while (occupiedCells.Contains(cell))
				cell = Randomizer.Next(0, cells);

			occupiedCells.Add(cell);

			IntPoint p = Convert1DPointTo2D(cell);

			double rand = Randomizer.NextDouble();

			caStates[p.X, p.Y] = rand <= Business.Avaibilities[0] ? CAState.Business1 :
				rand <= Business.Avaibilities[1] ? CAState.Business2 : CAState.Business3;
		}

		DrawCaStatesCanvas(caStates);

		RunSimulation();

		if (chb_debug.IsChecked == true && rb_test1.IsChecked == true)
		{
		}
	}

	private async Task RunSimulation()
	{
		for (int i = 0; i < TbToInt(txtn_of_iter); i++)
		{
			await Task.Delay(1000);
			List<int> occupiedByAgents = occupiedCells[..agents.Count];
			List<int> newOccupied = [];
			CAState[,] newCAStates = new CAState[rows, columns];
			newCAStates = InitializeCAState(newCAStates);
			for (int j = 0; j < agents.Count; j++)
			{
				IntPoint p = Convert1DPointTo2D(occupiedByAgents[j]);
				newCAStates[p.X, p.Y] = CAState.Agent;
			}

			newOccupied.AddRange(occupiedByAgents);
			foreach (int cell in occupiedCells[agents.Count..])
			{
				List<int> neighbours = GetCellNeighbourhood(cell);
				int newCell;
				bool changed = false;
				do
				{
					newCell = neighbours[Randomizer.Next(0, neighbours.Count)];
					if (newOccupied.Contains(newCell))
						neighbours.Remove(newCell);
					else
					{
						changed = true;
						newOccupied.Add(newCell);
					}
				} while (neighbours.Count > 0 && !changed);

				IntPoint p = Convert1DPointTo2D(cell);
				IntPoint p2 = Convert1DPointTo2D(newCell);

				if (changed)
					newCAStates[p2.X, p2.Y] = caStates[p.X, p.Y];
				else
					newCAStates[p.X, p.Y] = caStates[p.X, p.Y];
			}

			occupiedCells = newOccupied;
			caStates = newCAStates;
			DrawCaStatesCanvas(caStates);
		}
	}

	private List<int> GetCellNeighbourhood(int point)
	{
		return DetectCellPosition(point) switch
		{
			CellPosition.TopLeftCorner => [1, columns + 1, columns],
			CellPosition.TopRightCorner => [columns * 2 - 1, columns * 2 - 2, columns - 2],
			CellPosition.BottomLeftCorner => [(rows - 2) * columns, (rows - 2) * columns + 1, (rows - 1) * columns + 1],
			CellPosition.BottomRightCorner => [(rows - 1) * columns - 1, rows * columns - 2, (rows - 1) * columns - 2],
			CellPosition.TopEdge => [point + 1, point + columns + 1, point + columns, point + columns - 1, point - 1],
			CellPosition.RightEdge => [point - columns, point - 1, point - columns - 1],
			CellPosition.BottomEdge =>
				[point - columns, point - columns + 1, point + 1, point - 1, point - columns - 1],
			CellPosition.LeftEdge =>
				[point - columns, point - columns + 1, point + 1, point + columns + 1, point + columns],
			CellPosition.Inside =>
			[
				point - columns, point - columns + 1, point + 1, point + columns + 1, point + columns,
				point + columns - 1, point - 1, point - columns - 1
			]
		};
	}

	///Convert 1D point to 2D point.<br/>
	///For example: if row=6 and columns=6 and point=10
	/// then function return Point=(1,3)
	private IntPoint Convert1DPointTo2D(int point)
	{
		return new IntPoint
		{
			X = point / columns,
			Y = point % rows
		};
	}

	///Convert 2D point to 1D point.<br/>
	///For example: if row=6 and columns=6 and Point=(1,3)
	/// then function return 10
	private int Convert2DPointTo1D(IntPoint point)
	{
		return point.X * columns + point.Y + 1;
	}

	private CellPosition DetectCellPosition(int point)
	{
		if (point == 0)
			return CellPosition.TopLeftCorner;

		if (point == columns - 1)
			return CellPosition.TopRightCorner;

		if (point == (rows - 1) * columns)
			return CellPosition.BottomLeftCorner;

		if (point == rows * columns - 1)
			return CellPosition.BottomRightCorner;

		for (int i = 1; i < rows - 1; i++)
			if (point == rows * i)
				return CellPosition.LeftEdge;

		for (int i = 2; i <= rows - 1; i++)
			if (point == rows * i - 1)
				return CellPosition.RightEdge;

		for (int i = 1; i < columns - 1; i++)
			if (point == i)
				return CellPosition.TopEdge;

		for (int i = 1; i < columns - 1; i++)
			if (point == rows * columns - i - 1)
				return CellPosition.BottomEdge;

		return CellPosition.Inside;
	}

	private enum CellPosition
	{
		TopLeftCorner,
		TopRightCorner,
		BottomLeftCorner,
		BottomRightCorner,
		TopEdge,
		RightEdge,
		BottomEdge,
		LeftEdge,
		Inside
	}
}