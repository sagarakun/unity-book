using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Enemy : Character
{
	private Player _player;

	/// <summary>
	/// Destroyのコールバック
	/// </summary>
	protected override void OnDestroy ()
	{
		_player = null;
		base.OnDestroy ();
	}

	/// <summary>
	/// プレイヤー参照のセット
	/// </summary>
	public void SetPlayer (Player player)
	{
		_player = player;
	}

	/// <summary>
	/// 移動
	/// </summary>
	protected override void Move (Vector2 id)
	{
		base.Move (id);
		if (_isRight) {
			_human.RunR ();
			_isRight = false;
		} else {
			_human.RunL ();
			_isRight = true;
		}
	}

	/// <summary>
	/// ターン時行動分岐
	/// </summary>
	public void TurnAction ()
	{
		if (_isDead) {
			StartCoroutine (SequenceDead ());
		} else {
			if (_isDamage)
				StartCoroutine (Reaction ());
			else
				StartCoroutine (Action ());
		}
	}

	/// <summary>
	/// リアクションの分岐
	/// </summary>
	private IEnumerator Reaction ()
	{
		_human.Damage ();
		_isDamage = false;
		Action ();
		yield break;
	}

	/// <summary>
	/// 行動
	/// </summary>
	private IEnumerator Action ()
	{
		var f = GetMovePoint (_id, enumDirection.Front);
		var b = GetMovePoint (_id, enumDirection.Back);
		var l = GetMovePoint (_id, enumDirection.Left);
		var r = GetMovePoint (_id, enumDirection.Right);

		var route = GetShortRoute (f, b, l, r);

		switch (route) {
		case enumDirection.Front:
			Route (f, route, l, enumDirection.Left, r, enumDirection.Right);
			break;
		case enumDirection.Back:
			Route (b, route, l, enumDirection.Left, r, enumDirection.Right);
			break;
		case enumDirection.Left:
			Route (l, route, f, enumDirection.Front, b, enumDirection.Back);
			break;
		case enumDirection.Right:
			Route (r, route, f, enumDirection.Front, b, enumDirection.Back);
			break;
		}
		yield break;
	}

	/// <summary>
	/// 移動時ルート選択
	/// </summary>
	private void Route (Vector2 t, enumDirection te, Vector2 tA, enumDirection teA, Vector2 tB, enumDirection teB)
	{
		if (IsCell (t)) {
			Branch (t, te);
		} else {
			var e = GetShortRoute2nd (tA, tB, teA, teB);
			if (e == teA) {
				if (IsCell (tA)) {
					Branch (tA, teA);
				} else if (IsCell (tB))
					Branch (tB, teB);
			} else if (e == teB) {
				if (IsCell (tB))
					Branch (tB, teB);
				else if (IsCell (tA))
					Branch (tA, teA);
			}
		}
	}

	/// <summary>
	/// 攻撃、移動の行動分岐
	/// </summary>
	private void Branch (Vector2 vec, enumDirection rot)
	{
		var id = _player.GetID ();
		var f = GetMovePoint (id, enumDirection.Front);
		var b = GetMovePoint (id, enumDirection.Back);
		var l = GetMovePoint (id, enumDirection.Left);
		var r = GetMovePoint (id, enumDirection.Right);

		if (_id == f) {
			Rotation (enumDirection.Back);
			Attack ();
		} else if (_id == b) {
			Rotation (enumDirection.Front);
			Attack ();
		} else if (_id == l) {
			Rotation (enumDirection.Right);
			Attack ();
		} else if (_id == r) {
			Rotation (enumDirection.Left);
			Attack ();
		} else {
			Rotation (rot);
			Move (vec);
		}
	}

	/// <summary>
	/// 攻撃
	/// </summary>
	private void Attack ()
	{
		_human.Attack ();
		_player.Damage (_ATK, _id);
	}

	/// <summary>
	/// Playerまでの最短ルート取得
	/// </summary>
	private enumDirection GetShortRoute (Vector2 f, Vector2 b, Vector2 l, Vector2 r)
	{
		var type = enumDirection.Front;
		var id = _player.GetID ();
		var dF = Vector2.Distance (id, f);
		var dB = Vector2.Distance (id, b);
		var dL = Vector2.Distance (id, l);
		var dR = Vector2.Distance (id, r);

		if (dF <= dB && dF <= dL && dF <= dR)
			type = enumDirection.Front;
		else if (dB <= dF && dB <= dL && dB <= dR)
			type = enumDirection.Back;
		else if (dL <= dF && dL <= dB && dL <= dR)
			type = enumDirection.Left;
		else if (dR <= dF && dR <= dB && dR <= dL)
			type = enumDirection.Right;
		return type;
	}

	/// <summary>
	/// Playerまでの第2最短ルート
	/// </summary>
	private enumDirection GetShortRoute2nd (Vector2 vA, Vector2 vB, enumDirection eA, enumDirection eB)
	{
		var type = enumDirection.Front;
		var id = _player.GetID ();
		var dF = Vector2.Distance (id, vA);
		var dB = Vector2.Distance (id, vB);

		if (dF <= dB)
			type = eA;
		else if (dB <= dF)
			type = eB;
		return type;
	}
}