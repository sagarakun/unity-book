using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Player : Character
{
	[SerializeField] private PlayerCamera _camera;

	/// <summary>
	/// 衝突判定コールバック
	/// </summary>
	private void OnTriggerEnter (Collider other)
	{
		var cell = other.gameObject.GetComponent<Cell> ();
		if (cell != null) {
			SetCell (cell);
			var pos = cell.transform.position;
			transform.position = pos;
		}
	}

	/// <summary>
	/// ターン時リアクション
	/// ※プレイヤー専用
	/// </summary>
	public void TurnReaction ()
	{
		if (_isDead)
			StartCoroutine (SequenceDead ());
		else if (_isDamage)
			StartCoroutine (Reaction ());
	}

	/// <summary>
	/// ターン時アクション
	/// </summary>
	public void TurnAction (Vector2 vec)
	{
		if (vec.x == 1)
			Rotation (enumDirection.Right);
		else if (vec.x == -1)
			Rotation (enumDirection.Left);
		else if (vec.y == 1)
			Rotation (enumDirection.Front);
		else if (vec.y == -1)
			Rotation (enumDirection.Back);

		var xr = _id.x + vec.x;
		var yr = _id.y + vec.y;

		var cv = new Vector2 (xr, yr);
		var cell = GetCell (cv);

		//選択されたのがActiveなCellじゃ無ければターン終了
		if (!cell.GetIsActive ())
			return;
		
		//敵のいるcellに進もうとした場合	
		if (cell.GetObj ()) {
			var enemy = cell.GetObj ().GetComponent<Enemy> ();
			enemy.Damage (_ATK, _id);
			if (!enemy.IsDead ())
				_human.Attack ();
			
		} else {
			//何も無いcellに進もうとした場合
			Move (cell.GetID ());
			_camera.Move (cell.transform.position, _duration);

			if (_isRight) {
				_human.RunR ();
				_isRight = false;
			} else {
				_human.RunL ();
				_isRight = true;
			}
		}
	}

	/// <summary>
	/// リアクションの挙動
	/// </summary>
	private IEnumerator Reaction ()
	{
		_human.Damage ();
		_isDamage = false;
		yield break;
	}
}
