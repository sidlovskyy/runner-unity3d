public class Location
{
	public int x;

	public int y;

	public Location(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public bool IsZero
	{
		get
		{
			return x == 0 && y == 0;
		}
	}
}