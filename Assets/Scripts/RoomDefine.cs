using UnityEngine;
using System.Collections;

public class RoomDefine : ScriptableObject
{
	public string Name;
	public Vector2 Depth;
	public CellDefine Tile;
	public CellDefine Door;
	public CellDefine Wall;
}