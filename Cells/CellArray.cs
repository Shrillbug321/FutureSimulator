namespace FutureSimulator.Cells;

using static Global;
using static Util;
 
/*
public class CellArray
{
	private Cell[,] cells;
	public CellArray(int columns, int rows)
	{
		cells = new Cell[columns, rows];
	}
}
*/

/* In this class IDE hint me to using .NET 8 new feature — primary constructor.
 It is a similar to Kotlin's constructor. Traditional form is in comment above.*/
public class CellArray(int columns, int rows)
{
	private Cell[,] cells = new Cell[columns, rows];

	public Cell this[int point1D]
	{
		get
		{
			IntPoint p = window.Convert1DPointTo2D(point1D);
			return cells[p.X, p.Y];
		}
		set
		{
			IntPoint p = window.Convert1DPointTo2D(point1D);
			cells[p.X, p.Y] = value;
		}
	}

	public Cell this[int column, int row]
	{
		get => cells[column, row];
		set => cells[column, row] = value;
	}

	public Cell this[IntPoint point]
	{
		get => cells[point.X, point.Y];
		set => cells[point.X, point.Y] = value;
	}

	public List<Cell> this[List<int> points] => points.Select(point => window.Convert1DPointTo2D(point))
		.Select(p => cells[p.X, p.Y]).ToList();
}