using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FutureSimulator.Agents;
using FutureSimulator.Business;
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
	private int cellsNumber;
	private List<BusinessAbstract> businessTypes;

	public MainWindow()
	{
		CultureInfo ci = new CultureInfo("en-GB");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		InitializeComponent();
		caStates = InitializeCAState(caStates);
		DrawCaStatesCanvas(caStates);
		InitializeChart();
		Randomizer = rb_custom_seed.IsChecked == true
			? new Random(TbToInt(custom_seed))
			: new Random();
		window = this;
		businessTypes = [new Business1(), new Business2(), new Business3()];
		/*for (int i = 0; i < 100; i++)
		{
			new Agent();
		}*/
	}

	private CAState[,] InitializeCAState(CAState[,] caStates)
	{
		rows = TbToInt(txt_m_rows);
		columns = TbToInt(txt_n_colls);
		cellWidth = cells_canvas.Width / columns;
		cellHeight = cells_canvas.Height / rows;
		cellsNumber = rows * columns;
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
		cells_canvas.Children.Clear();
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
				cells_canvas.Children.Add(r);
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
				Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255))
			},
			new LineSeries
			{
				Title = "Fair", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0))
			},
			new LineSeries
			{
				Title = "Rich", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0))
			},
			new LineSeries
			{
				Title = "Init Capital", Values = new ChartValues<double>(),
				Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0))
			}
		];
		chart.Series = chartSeries;
		chart.ChartLegend = new DefaultLegend();

		//Sample data
		chartSeries[0].Values.Add(12.0);
		chartSeries[1].Values.Add(14.0);
		chartSeries[2].Values.Add(18.0);
		chartSeries[3].Values.Add(TbToDouble(init_capt));
	}

	private void StartButton_OnClick(object sender, RoutedEventArgs e)
	{
		Randomizer = rb_custom_seed.IsChecked == true
			? new Random(TbToInt(custom_seed))
			: new Random();
		agents = [];
		occupiedCells = [];
		caStates = InitializeCAState(caStates);
		GenerateCAStates(TbToInt(txtn_of_A), CAState.Agent);
		GenerateCAStates(TbToInt(txtn_of_D), CAState.Disease);
		GenerateCAStates(TbToInt(txtn_of_B), CAState.Business1);
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
			await Task.Delay(400);
			List<int> newOccupied = occupiedCells[..agents.Count];
			CAState[,] newCAStates = new CAState[rows, columns];
			newCAStates = InitializeCAState(newCAStates);
			newCAStates = RedrawAgents(caStates, newCAStates);

			foreach (int cell in occupiedCells[agents.Count..])
			{
				List<int> neighbours = GetCellNeighbourhood(cell);
				int newCell=-1;
				bool moved = false;
				
				//Move cell to neighbour
				while (!moved)
				{
					newCell = neighbours[Randomizer.Next(0, neighbours.Count)];
					if (newOccupied.Contains(newCell))
						neighbours.Remove(newCell);
					else
					{
						moved = true;
						newOccupied.Add(newCell);
					}
				} 

				//Move cell somewhere
				//(IMO in this case cell should not move)
				while (!moved)
				{
					newCell = Randomizer.Next(0, cellsNumber);
					if (!newOccupied.Contains(newCell))
					{
						moved = true;
						newOccupied.Add(newCell);
					}
				}

				IntPoint p = Convert1DPointTo2D(cell);
				IntPoint p2 = Convert1DPointTo2D(newCell);

				newCAStates[p2.X, p2.Y] = caStates[p.X, p.Y];
			}

			occupiedCells = newOccupied;
			caStates = newCAStates;
			DrawCaStatesCanvas(caStates);
		}
	}

	/// <summary>
	/// Generate random state of CA after initialization.
	/// </summary>
	/// <param name="iterations">Number of cells in state</param>
	/// <param name="state">State of cell</param>
	private void GenerateCAStates(int iterations, CAState state)
	{
		for (int i = 0; i < iterations; i++)
		{
			int cell = Randomizer.Next(0, cellsNumber);

			while (occupiedCells.Contains(cell))
				cell = Randomizer.Next(0, cellsNumber);

			occupiedCells.Add(cell);
			IntPoint p = Convert1DPointTo2D(cell);
			caStates[p.X, p.Y] = state;

			switch (state)
			{
				case CAState.Agent:
				{
					agents.Add(new Agent
					{
						Id = i, GlobalId = cell
					});
					break;
				}
				case CAState.Business1 or CAState.Business2 or CAState.Business3:
					double rand = Randomizer.NextDouble();
					caStates[p.X, p.Y] = rand <= Business1.Availability ? CAState.Business1 :
						rand <= Business2.Availability ? CAState.Business2 : CAState.Business3;
					continue;
			}
		}
	}

	/// <summary>
	/// Redraw agents from iteration <i>n-1</i> to iteration <i>n</i>
	/// </summary>
	/// <param name="oldStates">Array of states for previous iteration</param>
	/// <param name="newStates">Array of states for current iteration</param>
	/// <returns>Array with agents</returns>
	private CAState[,] RedrawAgents(CAState[,] oldStates, CAState[,] newStates)
	{
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if (oldStates[i, j] == CAState.Agent)
				{
					newStates[i, j] = CAState.Agent;
				}
			}
		}
		return newStates;
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
	///<b>Example:</b> if <i>row=6</i>, <i>columns=6</i> and <i>point=10</i>
	/// then function return <i>Point=(1,3)</i>
	private IntPoint Convert1DPointTo2D(int point)
	{
		return new IntPoint
		{
			X = point % rows,
			Y = point / columns
		};
	}

	///Convert 2D point to 1D point.<br/>
	///<b>Example:</b> if <i>row=6</i>, <i>columns=6</i> and <i>Point=(1,3)</i>
	/// then function return <i>10</i>
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

	private record AgentsActivity
	{
		private int agentID;
		private int agentGlobalID;
		private bool positionChanged;
		private double currentCapital;
		private bool capitalIncreased;
		private ChangeReason increaseReason;
		private bool capitalDecreased;
		private ChangeReason decreaseReason;
		private bool capitalNotChange;
		private bool diseaseSuspendBusiness;
		private int emergencyHops;
	}

	private record BusinessesActivity
	{
		private int businessID;
		private int businessGlobalID;
		private int type;
		private int emergencyHops;
	}
	private record DiseaseActivity
	{
		private int diseaseID;
		private int diseaseGlobalID;
		private int emergencyHops;
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

	private enum ChangeReason
	{
		NoChange, AfterBusiness1, AfterBusiness2, AfterBusiness3, Disease
	}
}