using Assets.Scripts.Auxilary;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
	public int MinDirectTiles = 10;
	public int MaxDirectTiles = 15;
	public GameManager GameManager;
	public GameObject TrackPrefab;	

	private List<TerrainTile> _terrainTiles;
	private Player _player;
	
	#region Events
	
	void Awake()
	{
		_terrainTiles = new List<TerrainTile>();		
		_player = GameManager.Player;
		
		
		var firstTileObject = (GameObject) Instantiate(TrackPrefab);

		var newTile = new TerrainTile(
			firstTileObject, 
			0, 
			0, 
			Direction.Up, 
			0, 
			TerrainTileType.Direct);
		
		newTile.IsBorderTail = true;
		_terrainTiles.Add(newTile);
	}

	void Update()
	{
		UpdateTrack();
	}

	private void UpdateTrack()
	{
		RemoveNotVisibleTiles();

		AddNewTiles();
	}
	
	#endregion
	
	private void RemoveNotVisibleTiles()
	{
		List<TerrainTile> tilesToRemove = new List<TerrainTile>();
		
		foreach (var terrainTile in _terrainTiles)
		{
			if (_player.CannotSee(terrainTile))
			{
				tilesToRemove.Add(terrainTile);				
			}
		}
		
		foreach(var tile in tilesToRemove)
		{
			Delete(tile);
		}
	}

	private void AddNewTiles()
	{
		List<TerrainTile> tilesProccessAdding = new List<TerrainTile>();
		
		foreach (var terrainTile in _terrainTiles)
		{
			if (terrainTile.IsBorderTail && _player.CanFullySee(terrainTile))
			{
				tilesProccessAdding.Add(terrainTile);
			}
		}
		
		foreach(var tile in tilesProccessAdding)
		{
			AddNextTileTo(tile);
		}
	}

	private void Delete(TerrainTile tile)
	{
		_terrainTiles.Remove(tile);
		Destroy(tile.GameObject);
	}

	private void AddNextTileTo(TerrainTile tile)
	{
		if(tile.Type != TerrainTileType.Direct)
		{
			AddNextDirectTileTo(tile);
		}
		else if(tile.DirectTilesBefore < MinDirectTiles)
		{
			AddNextDirectTileTo(tile);
		}
		else if (tile.DirectTilesBefore >= MaxDirectTiles)
		{
			AddNextTurnTileTo(tile);
		} 
		else 
		{
			if(MayAddTurnTile(tile))
			{
				AddNextTurnTileTo(tile);
			}
			else
			{
				AddNextDirectTileTo(tile);
			}
		}
	}
	
	private bool MayAddTurnTile(TerrainTile tile)
	{
		var mayAddTurnTile = Random.Range(0, MaxDirectTiles - MinDirectTiles);
		return mayAddTurnTile == 0;
	}
	
	private void AddNextDirectTileTo(TerrainTile tile)
	{
		if(tile.Type == TerrainTileType.TurnLeft || tile.Type == TerrainTileType.TurnLeftRight) 
		{
			var newTileDirection = GetLeftDirectionTo(tile.Direction);
			AddNextTileTo(tile, newTileDirection, TerrainTileType.Direct, true);
		}
		if(tile.Type == TerrainTileType.TurnRight || tile.Type == TerrainTileType.TurnLeftRight) 
		{
			var newTileDirection = GetRightDirectionTo(tile.Direction);
			AddNextTileTo(tile, newTileDirection, TerrainTileType.Direct, true);
		}
		if(tile.Type ==  TerrainTileType.Direct)
		{
			AddNextTileTo(tile, tile.Direction, TerrainTileType.Direct, false);
		}
	}
	
	private void AddNextTurnTileTo(TerrainTile tile)
	{
		var newTileType = GetRandomTurnType();
		AddNextTileTo(tile, tile.Direction, newTileType, false);
	}
	
	private void AddNextTileTo(
		TerrainTile tile, 
		Direction newDirection, 
		TerrainTileType newTileType, 
		bool isFirstAfterTurn)
	{
		var newTileLocation = GetNextLocationTo(tile.X, tile.Y, tile.Direction);
		
		var newTileGameObject = (GameObject) Instantiate(TrackPrefab);
		
		var newTileCoordinates = LocationToCoordinates(newTileLocation.X, newTileLocation.Y);
		newTileGameObject.transform.Translate(newTileCoordinates);
		
		var newTile = new TerrainTile(
			newTileGameObject, 
			newTileLocation.X, 
			newTileLocation.Y, 
			newDirection, 
			isFirstAfterTurn ? 0 : tile.DirectTilesBefore + 1, 
			newTileType);
		
		tile.IsBorderTail = false;
		newTile.IsBorderTail = true;
		
		_terrainTiles.Add(newTile);
	}
	
	private Location GetNextLocationTo(int x, int y, Direction direction)
	{
		if(direction == Direction.Up) return new Location(x, y + 1);
		else if(direction == Direction.Left) return new Location(x + 1, y);
		else if(direction == Direction.Down) return new Location(x, y - 1);
		else return new Location(x - 1, y);
	}
	
	private TerrainTileType GetRandomTurnType()
	{
		int random = Random.Range(1, 4);
		return (TerrainTileType) random;
	}
	
	private Vector3 LocationToCoordinates(int x, int y)
	{
		return new Vector3(
			x * GlobalConstants.TerrainTileSize, 
			0, 
			y * GlobalConstants.TerrainTileSize);
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
		foreach(var tile in _terrainTiles)
		{
			if(tile.X == x && tile.Y == y)
			{
				return tile;
			}
		}
		return null;
	}
}
