using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TerrainTile
{
	private GameObject _gameObject;

	public bool IsBorderTail { get; set; }
	
	public Vector3 Direction { get; private set; }	

	public TerrainTile(GameObject gameObject, Vector3 direction)
	{
		_gameObject = gameObject;
		Direction = direction;
	}	
}