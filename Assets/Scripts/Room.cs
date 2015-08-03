using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	private Vector2 _mapDepth;
	private RoomDefine _roomDefine;
	private Vector2 _roomDepthMax;
	private bool _isEmpty;
	private Vector2 _roomDepthFixed;
	private int _vectorDoorEntrance;
	private int _vectorDoorExit;

	[SerializeField] public Vector3 roomPosition;

	private List<List<GameObject>> _listCell;

	public bool IsEmpty ()
	{
		return _isEmpty;
	}

	public void InitRoom (Vector3 pos, Vector2 mapDepth, RoomDefine roomDefine, Vector2 roomDepthMax, bool isEmpty)
	{
		_isEmpty = isEmpty;
		if (!_isEmpty) {
			roomPosition = pos;
			_mapDepth = mapDepth;
			transform.localPosition = roomPosition;
			_roomDefine = roomDefine;
			_roomDepthMax = roomDepthMax;
			CreateCell ();
			SetTile ();

			_vectorDoorEntrance = Random.Range (0, 4);
			_vectorDoorExit = Random.Range (0, 4);

			if (_vectorDoorEntrance == _vectorDoorExit) {
				if (_vectorDoorEntrance == 3)
					_vectorDoorExit -= 1;
				else
					_vectorDoorExit += 1;
			}

			SetDoorEntrance (_vectorDoorEntrance);
			SetDoorExit (_vectorDoorExit);
			SetRoad ();
			Hypostatization ();
		}
	}

	private void CreateCell ()
	{
		_listCell = new List<List<GameObject>> ();
		for (int i = 0; i < (int)_roomDepthMax.x; i++) {

			List<GameObject> list_c = new List<GameObject> ();

			for (int j = 0; j < (int)_roomDepthMax.y; j++) {
				GameObject obj = new GameObject ();//
				obj.transform.SetParent (transform);
				Vector3 pos = new Vector3 (i, 0, j);
				obj.AddComponent<Cell> ();
				Cell cell = obj.GetComponent<Cell> ();
				cell.InitCell (pos);
				list_c.Add (obj);
			}
			_listCell.Add (list_c);
		}
	}

	private void SetTile ()
	{
		int randamX = (int)Random.Range (5f, _roomDepthMax.x - 2);
		int randamY = (int)Random.Range (5f, _roomDepthMax.y - 2);
		_roomDepthFixed = new Vector2 (randamX, randamY);
		for (int i = 0; i < (int)randamX; i++) {
			List<GameObject> list_c = _listCell [i];
			for (int j = 0; j < (int)randamY; j++) {
				GameObject obj = list_c [j];
				Cell cell = obj.GetComponent<Cell> ();
				CellDefine tile = _roomDefine.Tile;
				CellDefine wall = _roomDefine.Wall;

				//奥左右には壁を出す
				if (i == 0)
					cell.type = wall.type + "_L";
				else if (j == randamY - 1)
					cell.type = wall.type + "_F";
				else
					cell.type = tile.type;
			}
		}
	}

	private void SetDoorEntrance (int vectorDoor)
	{
		Cell cell = SetDoor (vectorDoor, true);
		cell.type = _roomDefine.Door.type + "_Entrance";
	}

	private void SetDoorExit (int vectorDoor)
	{
		Cell cell = SetDoor (vectorDoor, false);
		cell.type = _roomDefine.Door.type + "_Exit";
	}

	private Cell SetDoor (int vectorDoor, bool isEntrance)
	{
		int randamX = (int)Random.Range (0, _roomDepthFixed.x - 1);
		int randamY = (int)Random.Range (0, _roomDepthFixed.y - 1);

		GameObject obj;
		switch (vectorDoor) {
		case 0:
			obj = _listCell [randamX] [(int)_roomDepthFixed.y - 1];//FRONT
			break;
		case 1:
			obj = _listCell [randamX] [0];//BACK
			break;
		case 2:
			obj = _listCell [0] [randamY];//LEFT
			break;
		case 3:
			obj = _listCell [(int)_roomDepthFixed.x - 1] [randamY];//RIGHT
			break;
		default:
			obj = _listCell [randamX] [(int)_roomDepthFixed.y - 1];//FRONT
			break;
		}

		//roomの位置がmapのどこに当たるのか確認
		Vector2 roomID = ConversionRoomID (roomPosition);
		int rx = (int)roomID.x;
		int ry = (int)roomID.y;
		int mx = (int)_mapDepth.x - 1;
		int my = (int)_mapDepth.y - 1;

		if (rx == 0 && ry == 0 && isEntrance == true) {
			//Room_Entrance時
			vectorDoor = 1;
		} else if (rx == mx && ry == my && isEntrance == false) {
			//Door_Exitの時
			vectorDoor = 0;
		} else {
			//それ以外のRoomの時
			if (ry == my && vectorDoor == 0) {
				Debug.Log ("前は禁止");
				vectorDoor = 1;
			}
			if (rx == mx && vectorDoor == 1) {
				Debug.Log ("後は禁止");
			}
			if (rx == 0 && vectorDoor == 2) {
				Debug.Log ("左は禁止");
			}
			if (ry == 0 && vectorDoor == 3) {
				Debug.Log ("右は禁止");
			} 
		}


//		Debug.Log ("_mapDepth" + new Vector2 (_mapDepth.x - 1, _mapDepth.y - 1) + " : roomID" + roomID);

		Cell cell = obj.GetComponent<Cell> ();
		cell.doorSetVector = vectorDoor;

		return cell;
	}

	private void SetRoad ()
	{
		for (int i = 0; i < _listCell.Count; i++) {
			List<GameObject> list_c = _listCell [i];
			for (int j = 0; j < list_c.Count; j++) {
				GameObject obj = list_c [j];
				Cell cell = obj.GetComponent<Cell> ();

				//cellのタイプが入口 または 出口である
				if (cell.type == "Door_Entrance" || cell.type == "Door_Exit") {
					if (cell.doorSetVector == 0) { //方向が正面である
						SetRoadTile (obj, cell, true);
					} else if (cell.doorSetVector == 1) { //方向が右である
						SetRoadTile (obj, cell, false);
					}
				}
			}
		}
	}

	private void SetRoadTile (GameObject obj, Cell cell, bool isFront)
	{
		int roadLength;
		if (isFront) {
		
			int doorX = (int)obj.transform.position.z;
			roadLength = (int)_roomDepthMax.x - doorX;

		} else {
			
			int doorZ = (int)obj.transform.position.z;
			roadLength = (int)_roomDepthMax.y - doorZ;
		}
			
		for (int i = 0; i < roadLength; i++) {
			if (cell.doorSetVector == 0) {
				//Debug.Log (" > > > > > >" + i + "Door_Entrance is Front" + cell.doorSetVector);

			} else if (cell.doorSetVector == 1) {
				//Debug.Log (" > > > > > >\" + i + \" Door_Entrance is Right" + cell.doorSetVector);

			}
		}
	}

	private void Hypostatization ()
	{
		for (int i = 0; i < _listCell.Count; i++) {
			List<GameObject> list_c = _listCell [i];
			for (int j = 0; j < list_c.Count; j++) {
				GameObject obj = list_c [j];
				Cell cell = obj.GetComponent<Cell> ();

				cell.CreateCell ();
			}
		}
	}

	private Vector2 ConversionRoomID (Vector3 pos)
	{
		int x = (int)(pos.x / _roomDepthMax.x);
		int y = (int)(pos.z / _roomDepthMax.y);
		Vector2 conversion = new Vector2 (x, y);
		return conversion;
	}
}
