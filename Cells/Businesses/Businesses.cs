namespace FutureSimulator.Cells.Businesses;

public class Business1: Business
{
	public static double Availability { get; set; }
	
	public Business1(Cell cell)
	{
		Id = cell.Id;
		GlobalId = cell.GlobalId;
	}
}

public class Business2: Business
{
	public static double Availability { get; set; }
	
	public Business2(Cell cell)
	{
		Id = cell.Id;
		GlobalId = cell.GlobalId;
	}
}

public class Business3: Business
{
	public static double Availability { get; set; }
	
	public Business3(Cell cell)
	{
		Id = cell.Id;
		GlobalId = cell.GlobalId;
	}
}