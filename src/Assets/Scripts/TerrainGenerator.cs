using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
	public int minDirectTiles = 10;
	public int maxDirectTiles = 15;
	public int tailsWithoutObstaclesAfterTurn = 3;
	public GameManager gameManager;

	public GameObject trackPrefab;
	public GameObject obstaclePrefab;
	public GameObject coinPrefab;

	public float obstacleProbability = 0.9f;
	public float coinProbability = 0.2f;

	private List<TerrainTile> _terrainTiles;
	private Player _player;

	#region Events

	void Awake()
	{
		_terrainTiles = new List<TerrainTile>();
		_player = gameManager.Player;

		GenerateInitialTrack();
	}

	void Update()
	{
		UpdateTrack();
	}

	#endregion

	#region Public members

	public bool IsTurnTile(Vector3 point)
	{
		var location = CoordinatesToLocation(point);
		var tile = GetTile(location.x, location.y);
		if ((tile == null) || (tile.Type == TerrainTileType.Direct))
		{
			return false;
		}
		return true;
	}

	public bool ContainsPoint(Vector3 point)
	{
		var location = CoordinatesToLocation(point);		
		return (GetTile(location.x, location.y) != null);
	}

	public bool IsZeroTile(Vector3 point)
	{
		var location = CoordinatesToLocation(point);
		return location.IsZero;
	}

	#endregion

	#region Private Members

	#region Track Initialization

	private void GenerateInitialTrack()
	{
		for (int index = 0; index < 7; index++)
		{
			AddInitalTile(index);
		}

		_terrainTiles[_terrainTiles.Count - 1].isBorderTail = true;
	}

	private void AddInitalTile(int index)
	{
		var tileGameObject = (GameObject)Instantiate(trackPrefab);
		var newTileCoordinates = LocationToCoordinates(0, index);
		tileGameObject.transform.Translate(newTileCoordinates);

		var newTile = new TerrainTile(tileGameObject, 0, index, Direction.Up, index, TerrainTileType.Direct);
		_terrainTiles.Add(newTile);
	}

	#endregion

	private void UpdateTrack()
	{
		RemoveNotVisibleTiles();

		AddNewTiles();
	}

	#region Tiles Deletion

	private void RemoveNotVisibleTiles()
	{
		var tilesToRemove = new List<TerrainTile>();

		foreach (var terrainTile in _terrainTiles)
		{
			if (IsFarFromPlayer(terrainTile))
			{
				tilesToRemove.Add(terrainTile);
			}
		}

		foreach (var tile in tilesToRemove)
		{
			Delete(tile);
		}
	}

	private bool IsFarFromPlayer(TerrainTile terrainTile)
	{
		//NOTE: this is not the best implementation
		return Vector3.Distance(_player.transform.position, LocationToCoordinates(terrainTile.X, terrainTile.Y)) >= 60.0f;
	}

	private void Delete(TerrainTile tile)
	{
		_terrainTiles.Remove(tile);
		tile.Destroy();
	}

	#endregion

	#region Tiles Adding
	
	private void AddNewTiles()
	{
		var tilesProccessAdding = new List<TerrainTile>();

		foreach (var terrainTile in _terrainTiles)
		{
			if (terrainTile.isBorderTail && IsCloseToPlayer(terrainTile))
			{
				tilesProccessAdding.Add(terrainTile);
			}
		}

		foreach (var tile in tilesProccessAdding)
		{
			AddNextTileTo(tile);
		}
	}

	private bool IsCloseToPlayer(TerrainTile terrainTile)
	{
		//NOTE: this is not the best implementation
		return Vector3.Distance(_player.transform.position, LocationToCoordinates(terrainTile.X, terrainTile.Y)) < 50.0f;
	}

	private void AddNextTileTo(TerrainTile tile)
	{
		if (tile.Type != TerrainTileType.Direct)
		{
			AddNextDirectTileTo(tile);
		}
		else if (tile.DirectTilesBefore < minDirectTiles)
		{
			AddNextDirectTileTo(tile);
		}
		else if (tile.DirectTilesBefore >= maxDirectTiles)
		{
			AddNextTurnTileTo(tile);
		}
		else
		{
			if (CanAddTurnTile())
			{
				AddNextTurnTileTo(tile);
			}
			else
			{
				AddNextDirectTileTo(tile);
			}
		}
	}

	private bool CanAddTurnTile()
	{
		var mayAddTurnTile = Random.Range(0, maxDirectTiles - minDirectTiles);
		return mayAddTurnTile == 0;
	}

	private void AddNextDirectTileTo(TerrainTile prevTile)
	{
		if (prevTile.Type == TerrainTileType.TurnLeft || prevTile.Type == TerrainTileType.TurnLeftRight)
		{
			var newTileDirection = GetLeftDirectionTo(prevTile.Direction);
			AddNextTileTo(prevTile, newTileDirection, TerrainTileType.Direct, true);
		}
		if (prevTile.Type == TerrainTileType.TurnRight || prevTile.Type == TerrainTileType.TurnLeftRight)
		{
			var newTileDirection = GetRightDirectionTo(prevTile.Direction);
			AddNextTileTo(prevTile, newTileDirection, TerrainTileType.Direct, true);
		}
		if (prevTile.Type == TerrainTileType.Direct)
		{
			AddNextTileTo(prevTile, prevTile.Direction, TerrainTileType.Direct, false);
		}
	}

	private void AddNextTurnTileTo(TerrainTile prevTile)
	{
		var newTileType = GetRandomTurnType();
		AddNextTileTo(prevTile, prevTile.Direction, newTileType, false);
	}

	private void AddNextTileTo(
		TerrainTile tile,
		Direction newDirection,
		TerrainTileType newTileType,
		bool isFirstAfterTurn)
	{
		var newTileLocation = GetNextLocationTo(tile.X, tile.Y, newDirection);
		var newTileGameObject = (GameObject)Instantiate(trackPrefab);
		var newTileCoordinates = LocationToCoordinates(newTileLocation.x, newTileLocation.y);		
		newTileGameObject.transform.Translate(newTileCoordinates);

		var newTile = new TerrainTile(
			newTileGameObject,
			newTileLocation.x,
			newTileLocation.y,
			newDirection,
			isFirstAfterTurn ? 0 : tile.DirectTilesBefore + 1,
			newTileType);

		if (CanAddCoin(newTile))
		{
			AddCoin(newTile);
		}
		else if (CanAddObstacle(newTile, tile.hasObstacle))
		{
			AddObstacle(newTile);
		}

		tile.isBorderTail = false;
		newTile.isBorderTail = true;

		_terrainTiles.Add(newTile);
	}

	private bool CanAddObstacle(TerrainTile tile, bool previousTileHasObstacle)
	{
		if ((tile.DirectTilesBefore < tailsWithoutObstaclesAfterTurn) || (tile.Type != TerrainTileType.Direct) || previousTileHasObstacle)
		{
			return false;
		}

		var random = Random.Range(0, 1.0f);
		return random <= obstacleProbability;
	}

	private void AddObstacle(TerrainTile tile)
	{
		var obstacle = (GameObject)Instantiate(obstaclePrefab);
		obstacle.transform.Translate(tile.GameObject.transform.position);

		//NOTE: assume obstacle has sime size in all directions
		var halfObstacleSize = obstacle.collider.bounds.size.x / 2.0f;
		var possibleMoveRange = GlobalConstants.TerrainTileSize / 2.0f - halfObstacleSize;
		var shiftX = Random.Range(-possibleMoveRange, possibleMoveRange);
		var shiftY = Random.Range(-possibleMoveRange, possibleMoveRange);
		obstacle.transform.Translate(shiftX, 0, shiftY);

		//add as child to tile
		obstacle.transform.parent = tile.GameObject.transform;
		tile.hasObstacle = true;
	}

	private bool CanAddCoin(TerrainTile tile)
	{
		var random = Random.Range(0, 1.0f);
		return random <= coinProbability;
	}

	private void AddCoin(TerrainTile tile)
	{
		var coin = (GameObject)Instantiate(coinPrefab);
		coin.transform.Translate(tile.GameObject.transform.position);

		//NOTE: assume obstacle has sime size in all directions
		var halfObstacleSize = coin.collider.bounds.size.x / 2.0f;
		var possibleMoveRange = GlobalConstants.TerrainTileSize / 2.0f - halfObstacleSize;
		var shiftX = Random.Range(-possibleMoveRange, possibleMoveRange);
		var shiftY = Random.Range(-possibleMoveRange, possibleMoveRange);
		coin.transform.Translate(shiftX, 0, shiftY);

		//add as child to tile
		coin.transform.parent = tile.GameObject.transform;
	}

	#endregion

	#region Helper Methods

	private Location GetNextLocationTo(int x, int y, Direction direction)
	{
		if (direction == Direction.Up) return new Location(x, y + 1);
		if (direction == Direction.Left) return new Location(x + 1, y);
		if (direction == Direction.Down) return new Location(x, y - 1);
		return new Location(x - 1, y);
	}

	private TerrainTileType GetRandomTurnType()
	{
		int random = Random.Range(1, 4);
		return (TerrainTileType)random;
	}

	private Direction GetLeftDirectionTo(Direction direction)
	{
		switch (direction)
		{
			case Direction.Up: return Direction.Left;
			case Direction.Left: return Direction.Down;
			case Direction.Right: return Direction.Up;
			default:
			case Direction.Down: return Direction.Right;
		}
	}

	private Direction GetRightDirectionTo(Direction direction)
	{
		switch (direction)
		{
			case Direction.Up: return Direction.Right;
			case Direction.Left: return Direction.Up;
			case Direction.Right: return Direction.Down;
			default:
			case Direction.Down: return Direction.Left;
		}
	}

	private TerrainTile GetTile(int x, int y)
	{
		foreach (var tile in _terrainTiles)
		{
			if (tile.X == x && tile.Y == y)
			{
				return tile;
			}
		}
		return null;
	}

	private Vector3 LocationToCoordinates(int x, int y)
	{
		return new Vector3(x * GlobalConstants.TerrainTileSize, 0, y * GlobalConstants.TerrainTileSize);
	}

	private Location CoordinatesToLocation(Vector3 vector)
	{
		const float tileSize = GlobalConstants.TerrainTileSize;
		var x = (int)(((Mathf.Abs(vector.x) + tileSize / 2.0f) / tileSize) * (vector.x < 0 ? -1 : 1));
		var y = (int)(((Mathf.Abs(vector.z) + tileSize / 2.0f) / tileSize) * (vector.z < 0 ? -1 : 1));

		return new Location(x, y);
	}

	#endregion

	#endregion
}
