using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Enemy : Character
{
	private Player _player;
	private Vector2 _pId;

	public void SetPlayer (Player player)
	{
		_player = player;
	}

	protected override void Move (Vector2 id)
	{
		base.Move (id);
		if (_isRight) {
			_human.RunR (_duration);
			_isRight = false;
		} else {
			_human.RunL (_duration);
			_isRight = true;
		}
	}

	public override void TurnAction ()
	{
		if (_isDead) {
			Dead ();
		} else {
			if (_isDamage)
				Reaction ();
			else
				Action ();
		}
	}

	private void Action ()
	{
		var f = GetMovePoint (_id, enumRotType.Front);
		var b = GetMovePoint (_id, enumRotType.Back);
		var l = GetMovePoint (_id, enumRotType.Left);
		var r = GetMovePoint (_id, enumRotType.Right);

		var route = GetShortRoute (f, b, l, r);

		switch (route) {
		case enumRotType.Front:
			Route (f, route, l, enumRotType.Left, r, enumRotType.Right);
			break;
		case enumRotType.Back:
			Route (b, route, l, enumRotType.Left, r, enumRotType.Right);
			break;
		case enumRotType.Left:
			Route (l, route, f, enumRotType.Front, b, enumRotType.Back);
			break;
		case enumRotType.Right:
			Route (r, route, f, enumRotType.Front, b, enumRotType.Back);
			break;
		}
	}

	private void Route (Vector2 t, enumRotType te, Vector2 tA, enumRotType teA, Vector2 tB, enumRotType teB)
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

	private void Branch (Vector2 vec, enumRotType rot)
	{
		var id = _player.GetID ();
		var f = GetMovePoint (id, enumRotType.Front);
		var b = GetMovePoint (id, enumRotType.Back);
		var l = GetMovePoint (id, enumRotType.Left);
		var r = GetMovePoint (id, enumRotType.Right);

		if (_id == f) {
			Rotation (enumRotType.Back);
			Attack ();
		} else if (_id == b) {
			Rotation (enumRotType.Front);
			Attack ();
		} else if (_id == l) {
			Rotation (enumRotType.Right);
			Attack ();
		} else if (_id == r) {
			Rotation (enumRotType.Left);
			Attack ();
		} else {
			Rotation (rot);
			Move (vec);
		}
	}

	private void Attack ()
	{
		_human.Attack ();
		_player.Damage (_ATK, _id);
		_pId = _player.GetID ();
	}

	private enumRotType GetShortRoute (Vector2 f, Vector2 b, Vector2 l, Vector2 r)
	{
		var type = enumRotType.Front;
		var id = _player.GetID ();
		var dF = Vector2.Distance (id, f);
		var dB = Vector2.Distance (id, b);
		var dL = Vector2.Distance (id, l);
		var dR = Vector2.Distance (id, r);

		if (dF <= dB && dF <= dL && dF <= dR)
			type = enumRotType.Front;
		else if (dB <= dF && dB <= dL && dB <= dR)
			type = enumRotType.Back;
		else if (dL <= dF && dL <= dB && dL <= dR)
			type = enumRotType.Left;
		else if (dR <= dF && dR <= dB && dR <= dL)
			type = enumRotType.Right;
		return type;
	}

	private enumRotType GetShortRoute2nd (Vector2 vA, Vector2 vB, enumRotType eA, enumRotType eB)
	{
		var type = enumRotType.Front;
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