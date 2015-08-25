using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Character : MonoBehaviour
{
	[SerializeField] protected int _HP;
	[SerializeField] protected int _ATK;
	protected float _duration;
	protected Human _human;
	protected List<List<Cell>> _listCells;
	protected Vector2 _id;
	protected bool _isRight;
	protected Vector2 _damegeRot;
	protected bool _isDamage;
	protected int _index;
	protected bool _isDead;

	protected enum enumRotType
	{
		Front,
		Back,
		Right,
		Left,
		None,
	}

	protected enum enumAction
	{
		Attack,
		Move,
		Damage,
		None,
	}

	protected enum enumDistance
	{
		AwayL,
		AwayR,
		AwayF,
		AwayB,
		NearL,
		NearR,
		NearF,
		NearB,
		SameLR,
		SameFB,
		Not,
	}

	protected virtual void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	protected virtual void BranchReaction ()
	{
		if (_HP <= 0)
			StartCoroutine (SequenceDead ());
		else if (_isDamage) {
			_human.Damage ();
			if (_damegeRot == GetMovePoint (_id, enumRotType.Left)) {
				ActionDamage (enumRotType.Left);
			} else if (_damegeRot == GetMovePoint (_id, enumRotType.Right)) {
				ActionDamage (enumRotType.Right);
			} else if (_damegeRot == GetMovePoint (_id, enumRotType.Front)) {
				ActionDamage (enumRotType.Front);
			} else if (_damegeRot == GetMovePoint (_id, enumRotType.Back)) {
				ActionDamage (enumRotType.Back);
			}
		}
	}

	public void SetParameters (float duration, int index, List<List<Cell>> listCells)
	{
		_duration = duration;
		_human.SetDuration (_duration);
		_index = index;
		_listCells = listCells;
	}

	public void SetID (Cell cell)
	{
		var prev = _listCells [(int)_id.x] [(int)_id.y];
		prev.SetObj (null);
		cell.SetObj (gameObject);
		_id = cell.GetID ();
	}

	public Vector2 GetID ()
	{
		return _id;
	}

	public virtual void Damage (int atk, Vector2 id)
	{
		_HP -= atk;
		if (this is Player)
			Debug.Log ("プレイヤーに" + atk + "のダメージ");
		else
			Debug.Log ("敵" + _index + "に" + atk + "のダメージ");
		
		_damegeRot = id;
		_isDamage = true;
	}

	public virtual void TurnAction ()
	{
		
	}

	public virtual void TurnReaction ()
	{
		
	}

	private void OnDestroy ()
	{
		var cell = _listCells [(int)_id.x] [(int)_id.y];
		cell.SetObj (null);
		_listCells = null;
		Destroy (gameObject);
	}

	protected virtual IEnumerator SequenceDead ()
	{
		_isDead = true;
		_human.Dead ();
		yield return new WaitForSeconds (_duration);
		Destroy (this);
		yield break;
	}

	protected IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_isRight = false;
		_human = transform.Find ("human").GetComponent<Human> ();
		yield break;
	}

	protected virtual void ActionDamage (enumRotType rot)
	{
		Rotation (rot);
		Vector2 p = Vector2.zero;

		if (_isDead)
			return;

		if (rot == enumRotType.Front) {
			p = GetMovePoint (_id, enumRotType.Back);
		} else if (rot == enumRotType.Back) {
			p = GetMovePoint (_id, enumRotType.Front);
		} else if (rot == enumRotType.Left) {
			p = GetMovePoint (_id, enumRotType.Right);
		} else if (rot == enumRotType.Right) {
			p = GetMovePoint (_id, enumRotType.Left);
		}

		if (CellCheck (p)) {
			Move (p);
		}
	}

	protected virtual void Move (Vector2 vec)
	{
		if (vec.x < 0 || vec.x >= _listCells.Count || vec.y < 0 || vec.y >= _listCells.Count)
			return;
		var cell = _listCells [(int)vec.x] [(int)vec.y];
		var pos = cell.transform.position;
		transform.DOMove (pos, _duration);
		SetID (cell);
	}

	protected virtual void Rotation (enumRotType rot)
	{
		var dur = _duration / 2.0f;
		transform.DOLocalRotate (GetMoveRot (rot), dur);
	}

	protected bool CellCheck (Vector2 id)
	{
		if (_isDead)
			return false;
		var cell = _listCells [(int)id.x] [(int)id.y];
		if (cell == null)
			return false;
		
		if (cell.GetIsActive () && cell.GetObj () == null)
			return true;
		else
			return false;
	}

	protected Vector3 GetMoveRot (enumRotType rot)
	{
		if (rot == enumRotType.Left)
			return new Vector3 (0, -90, 0);
		else if (rot == enumRotType.Right)
			return new Vector3 (0, 90, 0);
		else if (rot == enumRotType.Front)
			return new Vector3 (0, 0, 0);
		else if (rot == enumRotType.Back)
			return new Vector3 (0, 180, 0);
		else
			return Vector3.zero;
	}

	protected Vector2 GetMovePoint (Vector2 id, enumRotType rot)
	{
		if (rot == enumRotType.Front)
			return new Vector2 (id.x, id.y + 1);
		else if (rot == enumRotType.Back)
			return new Vector2 (id.x, id.y - 1);
		else if (rot == enumRotType.Left)
			return new Vector2 (id.x - 1, id.y);
		else if (rot == enumRotType.Right)
			return new Vector2 (id.x + 1, id.y);
		else
			return id;
	}
}