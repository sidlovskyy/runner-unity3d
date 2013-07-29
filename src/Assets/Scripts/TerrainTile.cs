using UnityEngine;

public class TerrainTile
{
	public bool isBorderTail;
	public bool hasObstacle;

	public GameObject GameObject { get; private set; }

	public Direction Direction { get; private set; }

	public int X { get; private set; }

	public int Y { get; private set; }

	public int DirectTilesBefore { get; private set; }

	public TerrainTileType Type { get; private set; }	

	public TerrainTile(GameObject gameObject, int x, int y, Direction direction, int directTilesBefore, TerrainTileType type)
	{
		GameObject = gameObject;
		X = x;
		Y = y;
		DirectTilesBefore = directTilesBefore;
		Direction = direction;
		Type = type;
		hasObstacle = false;
	}

	public void Destroy()
	{
		Object.Destroy(GameObject);
	}
}