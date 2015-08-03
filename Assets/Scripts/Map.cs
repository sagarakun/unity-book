using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	[SerializeField] private MapDefine mapDefinbe;
	private List<List<GameObject>> _listRoom;

	private enum Stairs
	{
		NoStairs,
		UPStairs,
		DownStairs,
	}
	// Use this for initialization
	void Start ()
	{
		InitRoom ();
	}

	//Room初期化
	private void InitRoom ()
	{
		if (mapDefinbe != null) {
			_listRoom = new List<List<GameObject>> ();
			int line = (int)mapDefinbe.MapDepth.x;
			int column = (int)mapDefinbe.MapDepth.y;
			int roomMaxLine = (int)mapDefinbe.RoomDepthMax.x;
			int roomMaxColumn = (int)mapDefinbe.RoomDepthMax.y;
			for (int i = 0; i < line; i++) {
				List<GameObject> list_c = new List<GameObject> ();

				for (int j = 0; j < column; j++) {
					Vector3 pos = new Vector3 (i * roomMaxLine, 0, j * roomMaxColumn);
					GameObject obj;
					Stairs stairs = StartAndExit (i, j, line, column);
					if (stairs == Stairs.UPStairs) {
						obj = CreateRoom (pos, false);
					} else if (stairs == Stairs.DownStairs) {
						obj = CreateRoom (pos, false);
					} else {
						int ran = Random.Range (0, 10);
						if (ran <= 5)
							obj = CreateRoom (pos, false);
						else
							obj = CreateRoom (pos, true);
					}
					list_c.Add (obj);
				}
				_listRoom.Add (list_c);
			}
		}
	}

	private GameObject CreateRoom (Vector3 pos, bool isEmpty)
	{
		GameObject obj = new GameObject ();//GameObject.CreatePrimitive (PrimitiveType.Cube);
		obj.transform.SetParent (transform);
		obj.name = "obj";
		obj.AddComponent<Room> ();
		Room room = obj.GetComponent<Room> ();
		room.InitRoom (pos, mapDefinbe.MapDepth, mapDefinbe.Room, mapDefinbe.RoomDepthMax, isEmpty);
		return obj;
	}

	private Stairs StartAndExit (int x, int y, int limitX, int limitY)
	{
		Stairs stairs = Stairs.NoStairs;

		if (x == 0 && y == 0)
			stairs = Stairs.DownStairs;
		else if (x == limitX - 1 && y == limitY - 1)
			stairs = Stairs.UPStairs;
		
		return stairs;
	}
}