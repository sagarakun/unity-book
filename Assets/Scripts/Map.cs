using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	[SerializeField] private Player _player;
	[SerializeField] private GameObject _prefabEnemy;
	[SerializeField] private Vector2 _depth;
	[SerializeField] private float _duration;
	[SerializeField] private int _numberOfEnemy;

	private int _margin;
	private int _width;
	private bool _isTurning;

	private Transform _activeRoom;
	private List<Transform> _listRoom;
	private List<List<Cell>> _listCells;
	private List<Cell> _listActiveCells;
	private List<Enemy> _listEnemy;

	/* - - - - - - - - - - - - - */

	public List<List<Cell>> GetListCells ()
	{
		return this._listCells;
	}

	/* - - - - - - - - - - - - - */

	private void Start ()
	{		
		StartCoroutine (SequenceInit ());
	}

	//初期化シーケンス
	private IEnumerator SequenceInit ()
	{
		_width = 48;
		_margin = 4;
		yield return StartCoroutine (_player.SequenceSetPropaty (_duration, _margin));
		yield return StartCoroutine (SequenceSearchRoom ());
		yield return StartCoroutine (SequenceCreateCell ());
		yield return StartCoroutine (SequenceInitActiveCell ());
		yield return StartCoroutine (SequenceCreateEnemy ());
		yield return StartCoroutine (SequenceChangeActiveRoom ());
		yield return StartCoroutine (SequenceShowActiveEnemy ());
		yield break;
	}

	//Room用GameObjectの走査
	private IEnumerator SequenceSearchRoom ()
	{
		_listRoom = new List<Transform> ();
		foreach (Transform child in transform) {
			if (child.tag == "Room") {
				var c = child;
				_listRoom.Add (c);
			}
		}
		_activeRoom = _listRoom [0];
		yield break;
	}

	//cellを配置
	private IEnumerator SequenceCreateCell ()
	{
		_listCells = new List<List<Cell>> ();

		var limitU = ((_width * 2) / _margin) + 1;
		var limitX = limitU * _depth.x;
		var limitZ = limitU * _depth.y;

		for (int i = 0; i < limitX; i++) {

			var listC = new List<Cell> ();

			for (int j = 0; j < limitZ; j++) {

				var objC = new GameObject ();
				var col = objC.AddComponent<BoxCollider> ();
				col.isTrigger = true;
				var cell = objC.AddComponent<Cell> ();
				objC.transform.SetParent (transform);

				var x = -_width + (i * _margin);
				var z = -_width + (j * _margin);

				objC.transform.position = new Vector3 (x, 0, z);
				cell.CreateCellData (new Vector2 ((x / _margin) + 12, (z / _margin) + 12));
				listC.Add (cell);
			}
			_listCells.Add (listC);
		}
		yield break;
	}

	//ActiveなCellのみを抽出
	private IEnumerator SequenceInitActiveCell ()
	{
		_listActiveCells = new List<Cell> ();
		for (int i = 0; i < _listCells.Count; i++) {
			var list = _listCells [i];

			for (int j = 0; j < list.Count; j++) {
				var cell = _listCells [i] [j];
				cell.DestroyMaker ();
				if (cell.GetIsActive ())
					_listActiveCells.Add (cell);
			}
		}
		yield break;
	}

	//敵を_numberOfEnemy個分生成する
	private IEnumerator SequenceCreateEnemy ()
	{
		_listEnemy = new List<Enemy> ();
		int count = 0;

		while (count <= _numberOfEnemy) {
			var rand = Random.Range (0, _listActiveCells.Count);
			var cell = _listActiveCells [rand];

			//場所被りしないようにcellの持ってるenemy判定
			if (cell.GetObj () == null) {
				GameObject obj = Instantiate (_prefabEnemy);

				var enemy = obj.GetComponent<Enemy> ();
				enemy.SetPropaty (_duration, _margin);

				enemy.transform.SetParent (transform);
				enemy.transform.position = cell.transform.position;
				enemy.SetPlayer (_player);
				enemy.SetListCells (_listCells);

				for (int i = 0; i < _listRoom.Count; i++) {
					var room = _listRoom [i];
					if (IsActiveRoom (room, enemy.transform))
						enemy.SetRoom (room);
				}

				cell.SetObj (obj);
				enemy.SetID (cell.GetID ());

				_listEnemy.Add (enemy);
				count++;
			}
		}

		yield break;
	}

	//ActiveなRoomのみ表示する
	private IEnumerator SequenceChangeActiveRoom ()
	{
		for (int j = 0; j < _listRoom.Count; j++) {
			var room = _listRoom [j];
			if (room != _activeRoom) {
				room.gameObject.SetActive (false);
			} else {
				room.gameObject.SetActive (true);
			}
		}
		yield break;
	}


	private void Update ()
	{
		if (_isTurning) {
			return;
		} else if (Input.GetKey (KeyCode.UpArrow)) {//上入力
			StartCoroutine (SequenceInput (0, 1));
		} else if (Input.GetKey (KeyCode.DownArrow)) {//下入力
			StartCoroutine (SequenceInput (0, -1));
		} else if (Input.GetKey (KeyCode.RightArrow)) {//右入力
			StartCoroutine (SequenceInput (1, 0));
		} else if (Input.GetKey (KeyCode.LeftArrow)) {//左入力
			StartCoroutine (SequenceInput (-1, 0));
		}
	}

	//ユーザー入力シーケンス
	private IEnumerator SequenceInput (int x, int z)
	{
		_isTurning = true;
		yield return StartCoroutine (_player.SequenceInputAction (x, z));
		yield return StartCoroutine (SequenceSearchActiveRoom ());
		yield return StartCoroutine (SequenceShowActiveEnemy ());
		yield return StartCoroutine (SequenceEnemyTurn ());
		_isTurning = false;
		yield break;
	}
		
	//ActiveなRoomを調べる
	private IEnumerator SequenceSearchActiveRoom ()
	{
		for (int i = 0; i < _listRoom.Count; i++) {
			var room = _listRoom [i];
			if (IsActiveRoom (room, _player.transform))
				_activeRoom = room;
		}
		yield break;
	}

	//ActiveなRoomにいる敵だけを表示
	private IEnumerator SequenceShowActiveEnemy ()
	{
		yield return StartCoroutine (SequenceChangeActiveRoom ());

		for (int i = 0; i < _listEnemy.Count; i++) {
			var e = _listEnemy [i];
			e.Show (_activeRoom);
		}
		yield break;
	}

	//敵の行動ターン
	private IEnumerator SequenceEnemyTurn ()
	{
		for (int i = 0; i < _listEnemy.Count; i++) {
			var e = _listEnemy [i];
			if (e.gameObject.activeSelf)
				e.TurnAction ();
		}
		yield break;
	}

	//targetのpositionがactiveかどうかをboolで返す
	private bool IsActiveRoom (Transform target, Transform trans)
	{
		var lx = target.position.x - _width;
		var rx = target.position.x + _width;
		var bz = target.position.z - _width;
		var fz = target.position.z + _width;

		var px = trans.position.x;
		var pz = trans.position.z;

		if (px <= rx && px >= lx && pz <= fz && pz >= bz)
			return true;
		else
			return  false;
	}
}