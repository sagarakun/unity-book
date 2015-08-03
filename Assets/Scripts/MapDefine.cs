using UnityEngine;
using System.Collections;

public class MapDefine : ScriptableObject
{
	public string MapName;
	public Vector2 MapDepth;
	public Vector2 RoomDepthMax;
	public RoomDefine Room;
}