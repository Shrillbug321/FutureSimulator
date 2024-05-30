namespace FutureSimulator.Cells;

public class Disease:Cell
{
	private Disease(){}
	public Disease(Cell element):this()
	{
		Id = element.Id;
		GlobalId = element.GlobalId;
	}
}