using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	[SerializeField] private Player _player;
	[SerializeField] private GameObject _prefabEnemy;
	[SerializeField] private Vector2 _depth;
	[SerializeField] private float _duration;
	[SerializeField] private int _numberOfEnemyMax;
	[SerializeField] private int _numberOfEnemyMin;

	private int _margin;
	private int _width;
	private bool _isTurning;
	private Transform _activeRoom;
	private Transform _prevRoom;
	private List<Transform> _listRoom;
	private List<List<Cell>> _listCells;
	private List<Cell> _listActiveCells;
	private List<Enemy> _listEnemy;

	private void Start ()
	{		
		StartCoroutine (SequenceInit ());
	}

	//初期化シーケンス
	private IEnumerator SequenceInit ()
	{
		_width = 48;
		_margin = 4;
		_listEnemy = new List<Enemy> ();
		_listRoom = new List<Transform> ();
		_listCells = new List<List<Cell>> ();
		yield return StartCoroutine (SequenceSearchRoom ());
		yield return StartCoroutine (SequenceCreateCell ());
		yield return StartCoroutine (SequenceInitActiveCell ());
		yield return StartCoroutine (SequenceSearchActiveRoom ());
		yield return StartCoroutine (SequenceShowActiveRoom ());

		yield break;
	}

	//Room用GameObjectの走査
	private IEnumerator SequenceSearchRoom ()
	{
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
		var u = ((_width * 2) / _margin) + 1;
		var lx = u * _depth.x;
		var ly = u * _depth.y;
		for (int i = 0; i < lx; i++) {
			var list = new List<Cell> ();
			for (int j = 0; j < ly; j++) {
				var x = -_width + (i * _margin);
				var z = -_width + (j * _margin);
				var o = new GameObject ();
				o.transform.SetParent (transform);
				o.transform.position = new Vector3 (x, 0, z);
				var c = o.AddComponent<BoxCollider> ();
				c.isTrigger = true;
				var cell = o.AddComponent<Cell> ();
				cell.CreateCellData (new Vector2 ((x / _margin) + 12, (z / _margin) + 12));
				list.Add (cell);
			}
			_listCells.Add (list);
		}
		_player.SetParameters (_duration, 0, _listCells);
		yield break;
	}

	//ActiveなCellのみを抽出してListに保存
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
		
	//ActiveなRoom
	private IEnumerator SequenceShowActiveRoom ()
	{
		if (_prevRoom != _activeRoom) {
			yield return StartCoroutine (SequenceInitEnemy ());
			for (int j = 0; j < _listRoom.Count; j++) {
				var room = _listRoom [j];
				if (room != _activeRoom) {
					room.gameObject.SetActive (false);
				} else {
					room.gameObject.SetActive (true);
					yield return StartCoroutine (SequenceCreateEnemy ());
					_prevRoom = _activeRoom;
				}
			}
		}
		yield break;
	}

	//roomがアクティブになったら前のroomの敵は全て消す
	private IEnumerator SequenceInitEnemy ()
	{
		int z = 0;
		if (_listEnemy.Count == z)
			yield break;
		else {
			while (_listEnemy.Count > z) {
				Enemy e = _listEnemy [z];
				Destroy (e);
				_listEnemy.RemoveAt (z);
			}
			yield break;
		}
	}

	//敵を最小_numberOfEnemyMin体、最大_numberOfEnemyMax体生成する
	private IEnumerator SequenceCreateEnemy ()
	{
		var max = Random.Range (_numberOfEnemyMin, _numberOfEnemyMax);

		for (int i = 0; i < max; i++) {
			var list = GetRoomCell (_activeRoom);
			var rand = Random.Range (0, list.Count);
			var cell = list [rand];

			//場所被りしないようにcellの持ってるenemy判定
			if (cell.GetObj () == null) {
				if (IsActiveRoom (_activeRoom, cell.transform)) {
					GameObject obj = Instantiate (_prefabEnemy);
					var enemy = obj.GetComponent<Enemy> ();
					enemy.transform.SetParent (transform);
					enemy.SetParameters (_duration, i, _listCells);
					enemy.SetPlayer (_player);
					enemy.SetID (cell);
					enemy.transform.position = cell.transform.position;
					_listEnemy.Add (enemy);
				}
			}
		}
		yield break;
	}

	private List<Cell> GetRoomCell (Transform room)
	{
		var l = new List<Cell> (); 
		for (int i = 0; i < _listActiveCells.Count; i++) {
			var cell = _listActiveCells [i];
			if (IsActiveRoom (room, cell.transform)) {
				l.Add (cell);
			}
		}
		return l;
	}

	private void Update ()
	{
		if (_isTurning)
			return;
		else if (Input.GetKey (KeyCode.UpArrow)) //上入力
			StartCoroutine (SequenceInput (new Vector2 (0, 1)));
		else if (Input.GetKey (KeyCode.DownArrow)) //下入力
			StartCoroutine (SequenceInput (new Vector2 (0, -1)));
		else if (Input.GetKey (KeyCode.RightArrow)) //右入力
			StartCoroutine (SequenceInput (new Vector2 (1, 0)));
		else if (Input.GetKey (KeyCode.LeftArrow)) //左入力
			StartCoroutine (SequenceInput (new Vector2 (-1, 0)));
	}

	//ユーザー入力シーケンス
	private IEnumerator SequenceInput (Vector2 vec)
	{
		_isTurning = true;
		yield return StartCoroutine (SequencePLayerTurn (vec));
		yield return new WaitForFixedUpdate ();
		yield return StartCoroutine (SequenceSearchActiveRoom ());
		yield return StartCoroutine (SequenceShowActiveRoom ());
		yield return StartCoroutine (SequenceEnemyReactionTurn ());
		yield return new WaitForFixedUpdate ();
		yield return StartCoroutine (SequenceEnemyTurn ());
		yield return StartCoroutine (SequencePlayerReactionTurn ());
		yield return new WaitForFixedUpdate ();
		_isTurning = false;
		yield break;
	}

	private IEnumerator SequencePLayerTurn (Vector2 vec)
	{
		_player.SetInputID (vec);
		_player.TurnAction ();

		yield break;
	}

	//ActiveなRoomを調べる
	private IEnumerator SequenceSearchActiveRoom ()
	{
		for (int i = 0; i < _listRoom.Count; i++) {
			var r = _listRoom [i];
			var c = _listCells [(int)_player.GetID ().x] [(int)_player.GetID ().y];
			if (IsActiveRoom (r, c.transform))
				_activeRoom = r;
		}
		yield break;
	}

	//敵のリアクションターン
	private IEnumerator SequenceEnemyReactionTurn ()
	{
		for (int i = 0; i < _listEnemy.Count; i++) {
			var e = _listEnemy [i];
			if (e == null)
				_listEnemy.RemoveAt (i);
			else
				e.TurnReaction ();
		}

		yield break;
	}

	//敵の行動ターン
	private IEnumerator SequenceEnemyTurn ()
	{
		for (int i = 0; i < _listEnemy.Count; i++) {
			var e = _listEnemy [i];
			e.TurnAction ();
		}

		yield break;
	}

	//プレイヤーのリアクションターン
	private IEnumerator SequencePlayerReactionTurn ()
	{
		_player.TurnReaction ();
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