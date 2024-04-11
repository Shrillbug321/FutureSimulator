using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FutureSimulator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private int rows = 6;
	private int columns = 6;
	private double cellWidth;
	private double cellHeight;

	public MainWindow()
	{
		InitializeComponent();
		cellWidth = CellsCanvas.Width / columns;
		cellHeight = CellsCanvas.Height / rows;
		
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				Rectangle r = new()
				{
					Width = cellWidth,
					Height = cellHeight,
					Fill = new SolidColorBrush(Color.FromRgb((byte)(7*i), (byte)(8*j), (byte)(20+i))),
					Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255))
				};
				Canvas.SetTop(r, i * cellHeight);
				Canvas.SetLeft(r, j * cellWidth);
				CellsCanvas.Children.Add(r);
			}
		}
	}
}