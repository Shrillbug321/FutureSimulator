namespace FutureSimulator.Cells;
using static Global;
public class Cell
{
	public int Id {get; set;}
	public int GlobalId {get; set;}
	public int EmergencyHops { get; set; }

	public Cell()
	{
	}

	public Cell(int column, int row)
	{
		GlobalId = window.Convert2DPointTo1D(new Util.IntPoint{X=column, Y=row});
	}
}