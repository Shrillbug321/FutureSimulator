using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
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
	private int[,] caStates;

	public MainWindow()
	{
		InitializeComponent();
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

                switch (caStates[i, j])
                {
                    case 0:
                        r.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        break;
                    case 1:
                        // There will be new statement about pauper, fair, rich later but now every one is poor
                        r.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        break;
                    case 2:
                        r.Fill = new SolidColorBrush(Color.FromRgb(238, 130, 238));
                        break;
                    case 3:
                        r.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                        break;
                    case 4:
                        r.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 205));
                        break;
                    case 5:
                        r.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 139));
                        break;
                }

                Canvas.SetTop(r, j * cellHeight);
                Canvas.SetLeft(r, i * cellWidth);
                CellsCanvas.Children.Add(r);
            }
        }
    }
}