using Assets.Scripts.Auxilary;
using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
	public GameManager GameManager;

	public GameObject TrackPrefab;
	public GameObject LeftTurnPrefab;
	public GameObject RightTurnPrefab;
	public GameObject LeftRigthTurnPrefab;

	private DoubleKeyDictionary<int, int, TerrainTile> _terrainTiles;
	private Player _player;

	void Awake()
	{
		_terrainTiles = new DoubleKeyDictionary<int, int, TerrainTile>();
		_player = GameManager.Player;
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

	private void RemoveNotVisibleTiles()
	{
		foreach (var terrainTileRecord in _terrainTiles)
		{
			var terrainTile = terrainTileRecord.Value;			
			if (_player.CannotSee(terrainTile))
			{
				Delete(terrainTileRecord);
			}
		}
	}

	private void AddNewTiles()
	{
		foreach (var terrainTileRecord in _terrainTiles)
		{
			var terrainTile = terrainTileRecord.Value;
			if (terrainTile.IsBorderTail && _player.CanFullySee(terrainTile))
			{
				AddNextTileTo(terrainTile);
			}
		}
	}

	private void Delete(DoubleKeyPairValue<int, int, TerrainTile> terrainRecord)
	{
		//TODO: implement
	}

	private void AddNextTileTo(TerrainTile terrainTile)
	{
		//TODO: implement
	}
}
