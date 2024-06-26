﻿using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FutureSimulator.Cells;
using FutureSimulator.Cells.Agents;
using FutureSimulator.Cells.Businesses;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Geared;
using System.Diagnostics;

namespace FutureSimulator;

using static Util;
using static Global;
using static AgentAbstract;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private int rows;
	private int columns;
	private double cellWidth;
	private double cellHeight;
	private int cellsNumber;
	private int iteration;
	private CellArray caStates;
	private SeriesCollection chartSeries;

	private List<Agent> agents = [];
	private List<Business> businesses = [];
	private List<Disease> diseases = [];
	private Dictionary<int, List<AgentActivity>> agentsActivities;
	private Dictionary<int, List<BusinessActivity>> businessesActivities;
	private Dictionary<int, List<DiseaseActivity>> diseasesActivities;

	public MainWindow()
	{
		CultureInfo ci = new CultureInfo("en-GB");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		InitializeComponent();
		GetValuesFromHUD();
		window = this;
		InitializeChart();
		caStates = InitializeCAState(caStates);
		DrawCaStatesCanvas(caStates);
	}

	private CellArray InitializeCAState(CellArray caStates)
	{
		caStates = new CellArray(columns, rows);

		// Fill the array with zeros
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				caStates[i, j] = new Empty(i, j);
			}
		}

		return caStates;
	}

	/// <summary>
	/// Generate random state of CA after initialization.
	/// </summary>
	/// <param name="numberOfCells">Number of cells in state</param>
	/// <param name="state">State of cell</param>
	private List<int> GenerateCAStates(CAState state, int numberOfCells, List<int> occupiedCells)
	{
		for (int i = 0; i < numberOfCells; i++)
		{
			int cell = Randomizer.Next(0, cellsNumber);

			while (occupiedCells.Contains(cell))
				cell = Randomizer.Next(0, cellsNumber);

			occupiedCells.Add(cell);
			CreateCaElementFromState(state, cell, i);
		}

		return occupiedCells;
	}

	private void ReadCAStates_Checked(object sender, RoutedEventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		if ((bool)checkBox.IsChecked)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Title = "Read CA states"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				string selectedFilePath = openFileDialog.FileName;
				string[] lines = File.ReadAllLines(selectedFilePath);

				caStates = new CellArray(lines.Length, lines[0].Split(' ').Length);
				GetValuesFromFile(lines.Length, lines[0].Split(' ').Length);

				int id = 0;
				for (int i = 0; i < lines.Length; i++)
				{
					string[] values = lines[i].Split(' ');
					for (int j = 0; j < values.Length; j++)
					{
						CreateCaElementFromState((CAState)Enum.Parse(typeof(CAState), values[j]), i, j, id++);
					}
				}

				DrawCaStatesCanvas(caStates);
			}
		}
	}

    private void ReadRandNum_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;
        if ((bool)checkBox.IsChecked)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Read Random Numbers"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(selectedFilePath);

                List<double> values = [];
                foreach (string number in lines)
                {
                    if (double.TryParse(number, out double value))
                    {
                        values.Add(value);
                    }
                }
                RandomWrapper.NumbersFromFile = [..values];
            }
        }
    }

    private void ReadAProfile_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;
        if ((bool)checkBox.IsChecked)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Read A Profiles"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(selectedFilePath);

				int agentIndex = 0;
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					agents[agentIndex].Iq = int.Parse(values[2]);
					agents[agentIndex].HealthState = (HealthState)int.Parse(values[3])-1;
                    agents[agentIndex].BusinessesAccept = new Dictionary<string, double>
					{
						{ "Business1", double.Parse(values[4]) },
						{ "Business2", double.Parse(values[5]) },
						{ "Business3", double.Parse(values[6]) }
					};
					agents[agentIndex].Mobility = double.Parse(values[7]);
					agents[agentIndex].UpdateThresholds();
                }
            }
        }
    }

    private void DrawCaStatesCanvas(CellArray caStates)
	{
		cells_canvas.Children.Clear();
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				Rectangle r = new()
				{
					Width = cellWidth,
					Height = cellHeight,
					Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
					StrokeThickness = cellsNumber < 24 * 24 ? 1 :
						cellsNumber < 48 * 48 ? 0.5 :
						cellsNumber < 72 * 72 ? 0.25 :
						cellsNumber < 90 * 90 ? 0.1 : 0
				};

				r.Fill = caStates[i, j] switch
				{
					Empty => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
					Agent a =>
						a.WealthState switch
						{
							WealthState.Poor => new SolidColorBrush(Color.FromRgb(255, 255, 0)),
							WealthState.Fair => new SolidColorBrush(Color.FromRgb(180, 180, 50)),
							WealthState.Rich => new SolidColorBrush(Color.FromRgb(120, 120, 100)),
						},
					Disease => new SolidColorBrush(Color.FromRgb(238, 130, 238)),
					Business1 => new SolidColorBrush(Color.FromRgb(50, 60, 255)),
					Business2 => new SolidColorBrush(Color.FromRgb(0, 0, 155)),
					Business3 => new SolidColorBrush(Color.FromRgb(0, 0, 79)),
					_ => r.Fill
				};

				Canvas.SetTop(r, j * cellHeight);
				Canvas.SetLeft(r, i * cellWidth);
				cells_canvas.Children.Add(r);
			}
		}
	}

	private void CreateCaElementFromState(CAState state, int row, int column, int id)
	{
		CreateCaElementFromState(state, Convert2DPointTo1D(new IntPoint { X = row, Y = column }), id);
	}

	private void CreateCaElementFromState(CAState state, int point1d, int id)
	{
		Cell cell = new()
		{
			Id = id, GlobalId = point1d
		};
		switch (state)
		{
			case CAState.Agent:
				Agent agent = new(cell, (bool)chb_read_a_profile.IsChecked);
				caStates[point1d] = agent;
				agents.Add(agent);
				break;
			case CAState.Business1 or CAState.Business2 or CAState.Business3:
				double rand = Randomizer.NextDouble();
				Business business = rand <= Business1.Availability ? new Business1(cell) :
					rand <= Business2.Availability ? new Business2(cell) : new Business3(cell);
				businesses.Add(business);
				caStates[point1d] = business;
				break;
			case CAState.Disease:
				Disease disease = new(cell);
				caStates[point1d] = disease;
				diseases.Add(disease);
				break;
			default:
				caStates[point1d] = new Empty(cell);
				break;
		}
	}

	private void StartButton_OnClick(object sender, RoutedEventArgs e)
	{
		GetValuesFromHUD();
		if (CheckThatSeredyńskiEnterTooManyCells())
			return;

		List<int> occupiedCells = [];
		agents = [];
		businesses = [];
		diseases = [];
		agentsActivities = [];
		businessesActivities = [];
		diseasesActivities = [];
		ResetChart();
		Randomizer = new RandomWrapper();

		if (chb_debug.IsChecked == true)
		{
			if (rb_test1.IsChecked == true)
			{
                ReadRandNum_Checked(chb_read_rand_num, null);
                ReadCAStates_Checked(chb_read_ca_states, null);
				ReadAProfile_Checked(chb_read_a_profile, null);
            }
		}
		else
		{
			caStates = InitializeCAState(caStates);
			occupiedCells.AddRange(GenerateCAStates(CAState.Agent, TbToInt(txtn_of_A), occupiedCells));
			occupiedCells.AddRange(GenerateCAStates(CAState.Disease, TbToInt(txtn_of_D), occupiedCells));
			GenerateCAStates(CAState.Business1, TbToInt(txtn_of_B), occupiedCells);
			DrawCaStatesCanvas(caStates);
		}

		RunSimulation();
	}

	private bool CheckThatSeredyńskiEnterTooManyCells()
	{
		int sum = TbToInt(txtn_of_A) + TbToInt(txtn_of_B) + TbToInt(txtn_of_D);
		if (sum <= cellsNumber) return false;
		MessageBox.Show(
			$"Sum of agents, businesses and diseases ({sum}) is greater than number of automata cells ({cellsNumber}).",
			"Mój panie, do obsługi tego urządzenia potrzeba kilku wyszkolonych inżynierów!", MessageBoxButton.OK,
			MessageBoxImage.Error);
		return true;
	}

    private async Task RunSimulation()
    {
        string filePath = "result.txt";

        string header = "#\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10\t11\t12\t13\n";
        header += "#\t\tpoorest\tpoorest\tpoorest\trichest\trichest\trichest\tpoorest\tfair\trichest\t%of\t%of\t%of\n";
        header += "#\titer\tCAP\tA_id\tglob_ID\tCAP\tA_id\tglob_ID\tavCAP\tavCAP\tavCAP\tpoorest\tfair\trichest\n";
        File.WriteAllText(filePath, header);

        for (int i = 0; i < TbToInt(txtn_of_iter); i++)
        {
            await Task.Delay(TbToInt(txt_simulation_speed));

            iteration = i;
            CellArray newCAStates = new(columns, rows);
            CreateActivitiesLists();
            newCAStates = InitializeCAState(newCAStates);
            MoveCells(newCAStates);
            caStates = newCAStates;
            DrawCaStatesCanvas(caStates);

            AgentsInteractionWithEnvironment();
            UpdateAgentsWealthState();
            UpdateChart();

            // Result.txt
            SaveResults(filePath);
            SaveActivities();
        }
    }

    private void CreateActivitiesLists()
	{
		agentsActivities.Add(iteration, []);
		businessesActivities.Add(iteration, []);
		diseasesActivities.Add(iteration, []);
	}
	
	private void MoveCells(CellArray newCAStates)
	{
		List<int> newOccupied = [];
		foreach (Cell cell in agents.Concat<Cell>(businesses).Concat(diseases))
		{
			List<int> neighbours = GetCellNeighbourhoodPoints(cell.GlobalId);
			int newPoint = -1;
			bool moved = false;
			bool agentMoved = false;

			if (cell is Agent a)
			{
				if (Randomizer.NextDouble() > a.Mobility)
				{
					moved = true;
					agentMoved = true;
					newPoint = a.GlobalId;
				}
			}

			//Move cell to neighbour
			while (!moved && neighbours.Count > 0)
			{
				newPoint = neighbours[Randomizer.Next(0, neighbours.Count)];
				if (newOccupied.Contains(newPoint))
					neighbours.Remove(newPoint);
				else
					moved = true;
			}

			//Move cell somewhere
			//(IMO in this case cell should not move)
			while (!moved)
			{
				newPoint = Randomizer.Next(0, cellsNumber - 1);
				if (newOccupied.Contains(newPoint)) continue;
				moved = true;
				cell.EmergencyHops++;
			}

			cell.GlobalId = newPoint;
			newOccupied.Add(newPoint);
			newCAStates[newPoint] = cell;

			AddActivity(cell, agentMoved);
		}
	}

	private void AddActivity(Cell cell, bool agentMoved)
	{
		switch (cell)
		{
			case Agent agent:
				agentsActivities[iteration].Add(new AgentActivity
				{
					agentID = agent.Id,
					agentGlobalID = agent.GlobalId,
					positionChanged = agentMoved,
					currentCapital = agent.Capital,
					capitalIncreased = false,
					increaseReason = ChangeReason.NoChange,
					capitalDecreased = false,
					decreaseReason = ChangeReason.NoChange,
					capitalNotChange = true,
					diseaseSuspendBusiness = 0,
					emergencyHops = agent.EmergencyHops,
				});
				break;
			case Business business:
				businessesActivities[iteration].Add(new BusinessActivity
				{
					businessID = business.Id,
					businessGlobalID = business.GlobalId,
					type = business switch
					{
						Business1 => 1,
						Business2 => 2,
						_ => 3
					},
					emergencyHops = business.EmergencyHops,
				});
				break;
			case Disease disease:
				diseasesActivities[iteration].Add(new DiseaseActivity
				{
					diseaseID = disease.Id,
					diseaseGlobalID = disease.GlobalId,
					emergencyHops = disease.EmergencyHops,
				});
				break;
		}
	}
	
	private void AgentsInteractionWithEnvironment()
	{
		foreach (Agent agent in agents)
		{
			AgentActivity activity = agentsActivities[iteration].First(a=>a.agentID == agent.Id);
			if (agent.SuspendedCounter > 0)
			{
				AgentActivity prev = agentsActivities[iteration-1].First(a=>a.agentID == agent.Id);
				agent.SuspendedCounter--;
				activity = prev;
				activity.diseaseSuspendBusiness = agent.SuspendedCounter;
				continue;
			}

			List<Cell> neighbours = GetCellNeighbourhood(agent.GlobalId);
			Cell? first = neighbours.FirstOrDefault(n => n is not Empty, null);
			switch (first)
			{
				case Business b:
					if (agent.Capital >= b.InvestedCapital * Agent.InitialCapital)
					{
						if (Randomizer.NextDouble() < agent.BusinessesAccept[b.GetType().Name])
						{
							agent.SuspendedCounter = agent.IterationsOnBusiness;
							if (Randomizer.NextDouble() >= b.FailureRisc)
							{
								agent.Capital += b.IncreaseCapital * b.InvestedCapital;
								
								activity.capitalIncreased = true;
								activity.increaseReason = FindChangeReason(b);
								activity.capitalDecreased = false;
								activity.capitalNotChange = false;
							}
							else
							{
								activity.capitalIncreased = false;
								activity.capitalDecreased = true;
								activity.decreaseReason = FindChangeReason(b);
								activity.capitalNotChange = false;
							}
						}
					}
					break;
				
				case Disease:
					if (Randomizer.NextDouble() < agent.Disease)
					{
						agent.Capital -= agent.DecreaseInitialCapitalOnDisease * Agent.InitialCapital;
						agent.SuspendedCounter = agent.IterationsOnBusiness;
						
						activity.capitalIncreased = false;
						activity.capitalDecreased = true;
						activity.decreaseReason = ChangeReason.Disease;
						activity.capitalNotChange = false;
					}
					break;
				default: continue;
			}
			activity.currentCapital = agent.Capital;
			activity.diseaseSuspendBusiness = agent.SuspendedCounter;
		}
	}
	
	private void UpdateAgentsWealthState()
	{
		foreach (Agent agent in agents)
			agent.UpdateWealthState();
	}

	private void SaveResults(string filePath)
	{
		string data = $"\t{iteration+1} ";
		Agent poorestAgent = agents.OrderBy(a => a.Capital).First();
		Agent richestAgent = agents.OrderByDescending(a => a.Capital).First();
		int poorCount = agents.Count(a => a.IsPoor());
		int fairCount = agents.Count(a => a.IsFair());
		int richCount = agents.Count(a => a.IsRich());
		double poorAvgCap = poorCount > 0 ? agents.Where(a => a.IsPoor()).Average(a => a.Capital) : 0;
		double fairAvgCap = fairCount > 0 ? agents.Where(a => a.IsFair()).Average(a => a.Capital) : 0;
		double richAvgCap = richCount > 0 ? agents.Where(a => a.IsRich()).Average(a => a.Capital) : 0;
		double poorPercentage = poorCount / (double)agents.Count;
		double fairPercentage = fairCount / (double)agents.Count;
		double richPercentage = richCount / (double)agents.Count;
		data += $"\t{poorestAgent.Capital:0.00}\t{poorestAgent.Id+1}\t{poorestAgent.GlobalId+1}\t" +
		        $"{richestAgent.Capital:0.00}\t{richestAgent.Id+1}\t{richestAgent.GlobalId+1}\t" +
		        $"{poorAvgCap:0.00}\t{fairAvgCap:0.00}\t{richAvgCap:0.00}\t" +
		        $"{poorPercentage:0.00}\t{fairPercentage:0.00}\t{richPercentage:0.00}\n";

		File.AppendAllText(filePath, data);
	}

	private void SaveActivities()
	{
		string agentsPath = "agentsActivity.txt";
		string businessesPath = "businessesActivity.txt";
		string diseasesPath = "diseasesActivity.txt";

		if (iteration == 0)
		{
			string header = """
			                #1	2		3	4	5		6	7		8	9		10	11
			                #	current		position	curr	CAP		increase	CAP		decrease	CAP		D_susp	cr_A_of
			                #A_id	A_glob_id	changed	CAP	increased	reason	decreased	reason	not_change	business	emerg_hops
			                """;
			
			File.WriteAllText(agentsPath, header);
			
			header = """
	                #1	2		3	4
	                #	current			cr_B_of
	                #B_id	B_glob_id	B_type	emergency_hops
	                """;
			File.WriteAllText(businessesPath, header);
			
			header = """
			         #1	2		3
			         #	current		cr_D_of
			         #D_id	D_glob_id	emergency_hops
			         """;
			File.WriteAllText(diseasesPath, header);
		}

		string iterationInfo = $"\n	Iteration {iteration+1}\n";
		string agentData="", businessData="", diseaseData = "";
		
		foreach (AgentActivity activity in agentsActivities[iteration])
			agentData += $"{activity}\n";
		
		foreach (BusinessActivity activity in businessesActivities[iteration])
			businessData += $"{activity}\n";
		
		foreach (DiseaseActivity activity in diseasesActivities[iteration])
			diseaseData += $"{activity}\n";
		
		File.AppendAllText(agentsPath, iterationInfo+agentData);
        File.AppendAllText(businessesPath, iterationInfo+businessData);
        File.AppendAllText(diseasesPath, iterationInfo+diseaseData);
	}
	
	private ChangeReason FindChangeReason(Cell cell)
	{
		return cell switch
		{
			Business1 => ChangeReason.AfterBusiness1,
			Business2 => ChangeReason.AfterBusiness2,
			Business3 => ChangeReason.AfterBusiness3,
			Disease => ChangeReason.Disease,
			_ => ChangeReason.NoChange
		};
	}
	private List<int> GetCellNeighbourhoodPoints(int point)
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

	private List<Cell> GetCellNeighbourhood(int point, CellArray? cells = null)
	{
		cells ??= caStates;
		List<int> points = GetCellNeighbourhoodPoints(point);
		return cells[points];
	}

	private double FindAgentsByWealth(WealthState state)
	{
		List<Agent> filteredAgents = agents.FindAll(a =>
			state switch
			{
				WealthState.Poor => a.IsPoor(),
				WealthState.Fair => a.IsFair(),
				_ => a.IsRich()
			});
		return filteredAgents.Count > 0 ? filteredAgents.Average(a => a.Capital) : Agent.InitialCapital;
	}

	private void InitializeChart()
	{
		//Define chart series
		List<string> titles = ["Poor", "Fair", "Rich", "Init Capital"];
		List<SolidColorBrush> colors =
		[
			new SolidColorBrush(Color.FromRgb(0, 0, 255)), new SolidColorBrush(Color.FromRgb(0, 255, 0)),
			new SolidColorBrush(Color.FromRgb(255, 0, 0)), new SolidColorBrush(Color.FromRgb(0, 0, 0))
		];
		chartSeries = [];
		foreach ((string title, SolidColorBrush color) in titles.Zip(colors))
		{
			chartSeries.Add(new GLineSeries
			{
				Title = title, Values = new GearedValues<double> { Agent.InitialCapital },
				Stroke = color
			});
		}
		
		chart.Series = chartSeries;
		chart.ChartLegend = new DefaultLegend();
	}

	private void UpdateChart()
	{
		chartSeries[0].Values.Add(FindAgentsByWealth(WealthState.Poor));
		chartSeries[1].Values.Add(FindAgentsByWealth(WealthState.Fair));
		chartSeries[2].Values.Add(FindAgentsByWealth(WealthState.Rich));
		chartSeries[3].Values.Add(Agent.InitialCapital);
	}

	private void ResetChart()
	{
		for (int i = 0; i < 4; i++)
			chartSeries[i].Values.Clear();
	}

	///Convert 1D point to 2D point.<br/>
	///<b>Example:</b> if <i>row=6</i>, <i>columns=6</i> and <i>point=10</i>
	/// then function return <i>Point=(1,3)</i>
	public IntPoint Convert1DPointTo2D(int point)
	{
		return new IntPoint
		{
			X = point % columns,
			Y = point / columns
		};
	}

	///Convert 2D point to 1D point.<br/>
	///<b>Example:</b> if <i>row=6</i>, <i>columns=6</i> and <i>Point=(1,3)</i>
	/// then function return <i>10</i>
	public int Convert2DPointTo1D(IntPoint point)
	{
		return point.X * columns + point.Y;
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
			if (point == columns * i)
				return CellPosition.LeftEdge;

		for (int i = 2; i <= rows - 1; i++)
			if (point == columns * i - 1)
				return CellPosition.RightEdge;

		for (int i = 1; i < columns - 1; i++)
			if (point == i)
				return CellPosition.TopEdge;

		for (int i = 1; i < columns - 1; i++)
			if (point == rows * columns - i - 1)
				return CellPosition.BottomEdge;

		return CellPosition.Inside;
	}

	private void GetValuesFromHUD()
	{
		rows = TbToInt(txt_m_rows);
		columns = TbToInt(txt_n_colls);
		cellWidth = cells_canvas.Width / columns;
		cellHeight = cells_canvas.Height / rows;
		cellsNumber = rows * columns;
		Business1.Availability = TbToDouble(txt_p_b1t5);
		Business2.Availability = TbToDouble(txt_p_b1t5) + TbToDouble(txt_p_b2t5);
		Business3.Availability = TbToDouble(txt_p_b1t5) + TbToDouble(txt_p_b2t5) + TbToDouble(txt_p_b3t5);
		Agent.InitialCapital = TbToDouble(init_capt);
	}

	private void GetValuesFromFile(int rows, int columns)
	{
		txt_m_rows.Text = rows.ToString();
		txt_n_colls.Text = columns.ToString();
		GetValuesFromHUD();
	}
}