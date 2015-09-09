using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// キャラクター操作用クラス
/// PlayerとEnemyの親クラス
/// </summary>
public class Character : MonoBehaviour
{
	[SerializeField] protected int _HP;
	[SerializeField] protected int _ATK;
	protected int _index;
	protected float _duration;
	protected List<List<Cell>> _listCells;
	protected Vector2 _id;
	protected bool _isRight;
	protected bool _isDamage;
	protected bool _isDead;
	protected Human _human;

	/// <summary>
	/// 方向用Enum
	/// </summary>
	protected enum enumDirection
	{
		Front,
		Back,
		Right,
		Left,
	}

	/// <summary>
	/// アクション用Enum
	/// </summary>
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
	/// 引数で渡したCellにCharacterがセットされる
	/// </summary>
	public void SetCell (Cell cell)
	{
		var prev = GetCell (_id);
		prev.SetObj (null);
		cell.SetObj (gameObject);
		_id = cell.GetID ();
	}

	/// <summary>
	/// 現在セット中Cellを判別するためのVector2を返す
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

	/// <summary>
	/// 生成時コールバック
	/// </summary>
	protected virtual void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	/// <summary>
	/// 破棄時コールバック
	/// </summary>
	protected virtual void OnDestroy ()
	{
		var cell = GetCell (_id);
		cell.SetObj (null);
		Destroy (gameObject);
	}

	/// <summary>
	/// ダメージを受けた
	/// </summary>
	public virtual void Damage (int atk, Vector2 id)
	{
		_HP -= atk;

		if (_HP <= 0)
			_isDead = true;

		_isDamage = true;

		if (this is Player) {
			if (_isDead)
				Debug.Log ("プレイヤー死亡");
			else
				Debug.Log ("プレイヤーに" + atk + "のダメージ : 残りHPは" + _HP);
		} else {
			if (_isDead)
				Debug.Log ("敵" + _index + "死亡");
			else
				Debug.Log ("敵" + _index + "に" + atk + "のダメージ: 残りHPは" + _HP);
		}
	}

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
		Destroy (this);
		yield break;
	}

	/// <summary>
	/// 移動
	/// </summary>
	protected virtual void Move (Vector2 vec)
	{
		var cell = GetCell (vec);
		var pos = cell.transform.position;
		transform.DOMove (pos, _duration);
		SetCell (cell);
	}

	/// <summary>
	/// 回転
	/// </summary>
	protected virtual void Rotation (enumDirection rot)
	{
		var dur = _duration / 2.0f;
		transform.DOLocalRotate (GetMoveRot (rot), dur);
	}

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
	/// 引数のenumDirectionに対応するVector3を取得
	/// </summary>
	protected Vector3 GetMoveRot (enumDirection rot)
	{
		if (rot == enumDirection.Left)
			return new Vector3 (0, -90, 0);
		else if (rot == enumDirection.Right)
			return new Vector3 (0, 90, 0);
		else if (rot == enumDirection.Front)
			return new Vector3 (0, 0, 0);
		else if (rot == enumDirection.Back)
			return new Vector3 (0, 180, 0);
		else
			return Vector3.zero;
	}

	/// <summary>
	/// 第1引数に入れたVector2の前後左右のVector2の中から
	/// 第2引数に入れたenumDirectionに対応したものを取得
	/// </summary>
	protected Vector2 GetMovePoint (Vector2 id, enumDirection rot)
	{
		if (rot == enumDirection.Front)
			return new Vector2 (id.x, id.y + 1);
		else if (rot == enumDirection.Back)
			return new Vector2 (id.x, id.y - 1);
		else if (rot == enumDirection.Left)
			return new Vector2 (id.x - 1, id.y);
		else if (rot == enumDirection.Right)
			return new Vector2 (id.x + 1, id.y);
		else
			return id;
	}
}