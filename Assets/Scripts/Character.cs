using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Character : MonoBehaviour
{
	[SerializeField] protected int _HP;
	[SerializeField] protected int _ATK;
	protected int _index;
	protected float _duration;
	protected List<List<Cell>> _listCells;
	protected Vector2 _id;
	protected Vector2 _damegeRot;
	protected bool _isRight;
	protected bool _isDamage;
	protected bool _isDead;
	protected Human _human;


	protected enum enumRotType
	{
		Front,
		Back,
		Right,
		Left,
	}

	protected enum enumAction
	{
		Attack,
		Move,
		Damage,
		Wait,
	}

	/// <summary>
	/// 可動に必要なパラメータを外部から参照
	/// </summary>
	public void SetParameters (float duration, int index, List<List<Cell>> listCells)
	{
		_duration = duration;
		_human.SetDuration (_duration);
		_index = index;
		_listCells = listCells;
	}

	/// <summary>
	/// セルの設定
	/// </summary>
	public void SetID (Cell cell)
	{
		var prev = GetCell (_id);
		prev.SetObj (null);
		cell.SetObj (gameObject);
		_id = cell.GetID ();
	}

	/// <summary>
	/// 現在設定中のID用Vector2を返す
	/// </summary>
	public Vector2 GetID ()
	{
		return _id;
	}

	/// <summary>
	/// 現在設定中のID用Vector2を返す
	/// </summary>
	public bool IsDead ()
	{
		return _isDead;
	}

	/*
	 * Mapからの干渉部分 ----------------------------------------------------------------------
	*/

	/// <summary>
	/// アクション用Function
	/// 継承先で設定
	/// </summary>
	public virtual void TurnAction ()
	{

	}

	/*
	 * コールバック ----------------------------------------------------------------------
	*/

	/// <summary>
	/// 生成時コールバック
	/// </summary>
	protected virtual void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	protected virtual void OnDestroy ()
	{
		var cell = GetCell (_id);
		cell.SetObj (null);
		Destroy (gameObject);
	}
	/*
	 * OtherCharacterからの干渉部分 ----------------------------------------------------------------------
	*/

	/// <summary>
	/// ダメージを受けた時呼ばれるFunction
	/// </summary>
	public virtual void Damage (int atk, Vector2 id)
	{
		_HP -= atk;

		if (_HP <= 0)
			_isDead = true;

		_damegeRot = id;
		_isDamage = true;

		if (this is Player)
			Debug.Log ("プレイヤーに" + atk + "のダメージ");
		else
			Debug.Log ("敵" + _index + "に" + atk + "のダメージ");
	}
		
	/*
	 * コルーチン ----------------------------------------------------------------------
	*/

	/// <summary>
	/// 初期化コルーチン
	/// </summary>
	protected IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_isRight = false;
		_human = transform.Find ("human").GetComponent<Human> ();
		yield break;
	}

	/// <summary>
	/// 死亡時コルーチン
	/// </summary>
	protected virtual IEnumerator SequenceDead ()
	{
		_human.Dead ();
		yield return new WaitForSeconds (_duration * 2);
		var cell = GetCell (_id);
		cell.SetObj (null);
		Destroy (gameObject);
		yield break;
	}

	/*
	 * Characterの挙動 ----------------------------------------------------------------------
	*/

	/// <summary>
	/// リアクションの挙動
	/// </summary>
	protected virtual void Reaction ()
	{
		_human.Damage ();
		_isDamage = false;

		if (_damegeRot == GetMovePoint (_id, enumRotType.Left)) {
			Rotation (enumRotType.Left);
		} else if (_damegeRot == GetMovePoint (_id, enumRotType.Right)) {
			Rotation (enumRotType.Right);
		} else if (_damegeRot == GetMovePoint (_id, enumRotType.Front)) {
			Rotation (enumRotType.Front);
		} else if (_damegeRot == GetMovePoint (_id, enumRotType.Back)) {
			Rotation (enumRotType.Back);
		}
	}

	/// <summary>
	/// 移動
	/// </summary>
	protected virtual void Move (Vector2 vec)
	{
		var cell = GetCell (vec);
		var pos = cell.transform.position;
		transform.DOMove (pos, _duration);
		SetID (cell);
	}

	/// <summary>
	/// 回転
	/// </summary>
	protected virtual void Rotation (enumRotType rot)
	{
		var dur = _duration / 2.0f;
		transform.DOLocalRotate (GetMoveRot (rot), dur);
	}

	/// <summary>
	/// 死亡
	/// </summary>
	protected virtual void Dead ()
	{
		StartCoroutine (SequenceDead ());
	}


	/*
	 * Util ----------------------------------------------------------------------
	*/

	/// <summary>
	/// 引数に入れたCellがActiveかどうかを判別する
	/// </summary>
	protected bool IsCellActive (Cell cell)
	{
		return cell.GetIsActive ();
	}

	/// <summary>
	/// 引数に入れたCellにGameObjectが配置されてるかを判別
	/// </summary>
	protected bool IsCellNotPlace (Cell cell)
	{
		if (cell.GetObj () == null)
			return true;
		else
			return false;
	}

	/// <summary>
	/// 引数に入れたVector2がCellの範囲内に収まっているかを判別
	/// </summary>
	protected bool IsCellOutRange (Vector2 vec)
	{
		if (vec.x < 0 || vec.x >= _listCells.Count || vec.y < 0 || vec.y >= _listCells.Count)
			return false;
		else
			return true;
	}

	/// <summary>
	/// セルが進める状態なのか判別
	/// </summary>
	protected bool IsCell (Vector2 vec)
	{
		bool flg = false;
		if (IsCellOutRange (vec)) {
			var c = GetCell (vec);
			if (IsCellNotPlace (c) && IsCellActive (c))
				flg = true;	
		}
		return flg;
	}

	/// <summary>
	/// 引数のVector2からCellを取得
	/// </summary>
	protected Cell GetCell (Vector2 vec)
	{
		var cell = _listCells [(int)vec.x] [(int)vec.y];
		return cell;
	}

	/// <summary>
	/// 引数のenumRotTypeに対応するVector3を取得
	/// </summary>
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

	/// <summary>
	/// 第1引数に入れたVector2の前後左右のVector2の中から
	/// 第2引数に入れたenumRotTypeに対応したものを取得
	/// </summary>
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