using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	private List<List<Cell>> _listCells;
	private float _duration;
	private Human _human;
	private Vector2 _id;
	private Vector3 _prevPos;
	private int _margin;
	private bool _isRight;
	private Transform _room;
	private Player _player;
	private List<AStarLog> _listAStar;

	public void SetPropaty (float duration, int margin)
	{
		_duration = duration;
		_margin = margin;
		_human.SetDuration (_duration);
	}

	public void SetRoom (Transform room)
	{
		_room = room;
	}

	public void Show (Transform room)
	{
		if (_room == room)
			transform.gameObject.SetActive (true);
		else
			transform.gameObject.SetActive (false);
	}

	public void SetID (Vector2 vec)
	{
		_id = vec;
	}

	public void SetPlayer (Player player)
	{
		_player = player;
	}

	public void SetListCells (List<List<Cell>> listCells)
	{
		_listCells = listCells;
	}

	public void Damage (Vector2 id)
	{
		var l = new Vector2 (_id.x + 1, _id.y);
		var r = new Vector2 (_id.x - 1, _id.y);
		var f = new Vector2 (_id.x, _id.y + 1);
		var b = new Vector2 (_id.x, _id.y - 1);
		var dur = _duration / 2.0f;

		if (id == l) {
			transform.DOLocalRotate (new Vector3 (0, 90, 0), dur);
			_human.Damage ();
		} else if (id == r) {
			transform.DOLocalRotate (new Vector3 (0, -90, 0), dur);
			_human.Damage ();
		} else if (id == f) {
			transform.DOLocalRotate (new Vector3 (0, 0, 0), dur);
			_human.Damage ();
		} else if (id == b) {
			transform.DOLocalRotate (new Vector3 (0, 180, 0), dur);
			_human.Damage ();
		}
	}

	private void Awake ()
	{
		_listAStar = new List<AStarLog> ();
		StartCoroutine (SequenceInit ());
	}

	private IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_isRight = false;
		_human = transform.Find ("human").GetComponent<Human> ();
		yield break;
	}

	public void TurnAction ()
	{
		_human.Attack ();
		StartCoroutine (SequenceAction ());
	}

	private IEnumerator SequenceAction ()
	{
		if (_listAStar.Count > 0) {
			for (int i = 0; i < _listAStar.Count; i++) {
				var a = _listAStar [0];
				Destroy (a);
				_listAStar.RemoveAt (0);
			}
		}

		var list = GetListAStarLog (_id);
		//openしたastarから進行方向を算出

		yield break;
	}

	private void MoveCalculation (List<AStarLog> list)
	{
		
	}

	private List<AStarLog> GetListAStarLog (Vector2 vec)
	{
		var basic = GetAstar (vec, 0);
		var left = GetAstar (new Vector2 (vec.x - 1, vec.y), 1);
		var right = GetAstar (new Vector2 (vec.x + 1, vec.y), 1);
		var front = GetAstar (new Vector2 (vec.x, vec.y + 1), 1);
		var back = GetAstar (new Vector2 (vec.x, vec.y - 1), 1);

		var list = new List<AStarLog> ();
		return list;
	}

	private AStarLog GetAstar (Vector2 vec, int num)
	{
		var cell = _listCells [(int)vec.x] [(int)vec.y];
		if (cell.GetIsActive ()) {
			var astar = CostCalculation (num, _player.GetID (), cell.GetID ());
			return astar;
		} else
			return null;
	}

	private AStarLog CostCalculation (int num, Vector2 goal, Vector2 start)
	{
		AStarLog astar = ScriptableObject.CreateInstance<AStarLog> ();
		var dx = goal.x - start.x;
		var dy = goal.y - start.y;

		var h = dx + dy; 
		var c = num;
		var s = c + h;

		astar.hCost = (int)h;
		astar.cost = (int)c;
		astar.score = (int)s;
		astar.id = start;

		return astar;
	}
}