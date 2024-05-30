namespace FutureSimulator.Cells;

public class Empty : Cell
{
	public Empty()
	{
		Id = -1;
		GlobalId = -1;
		EmergencyHops = -1;
	}

	public Empty(int i, int j) : base(i, j)
	{
	}
	
	public Empty(Cell cell) : this()
	{
		Id = cell.Id;
		GlobalId = cell.GlobalId;
	}
}