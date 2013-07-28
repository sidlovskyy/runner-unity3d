using System;

public class Location
{
	public int X { get; private set; }
	
	public int Y { get; private set; }
	
	public Location(int x, int y)
	{
		X = x;
		Y = y;
	}
}