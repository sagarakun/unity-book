using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Player : Character
{
	[SerializeField] private PlayerCamera _camera;
	private Vector2 _inputID;

	public void SetInputID (Vector2 id)
	{
		_inputID = id;
	}

	private void OnTriggerEnter (Collider other)
	{
		var cell = other.gameObject.GetComponent<Cell> ();
		if (cell != null) {
			SetID (cell);
			var pos = cell.transform.position;
			transform.position = pos;
		}
	}

	public override void TurnReaction ()
	{
		BranchReaction ();
	}

	public override void TurnAction ()
	{
		_isDamage = false;

		if (_inputID.x == 1)
			Rotation (enumRotType.Right);
		else if (_inputID.x == -1)
			Rotation (enumRotType.Left);
		else if (_inputID.y == 1)
			Rotation (enumRotType.Front);
		else if (_inputID.y == -1)
			Rotation (enumRotType.Back);

		var xr = _id.x + _inputID.x;
		var yr = _id.y + _inputID.y;
		var cell = _listCells [(int)xr] [(int)yr];

		if (!cell.GetIsActive ())
			return;
		
		enumAction e; 
		//敵のいるcellに進もうとした場合	
		if (cell.GetObj ()) {
			var enemy = cell.GetObj ().GetComponent<Enemy> ();
			enemy.Damage (_ATK, _id);
			e = enumAction.Attack;
		} else {
			//何も無いcellに進もうとした場合
			e = enumAction.Move;
		}

		switch (e) {
		case enumAction.Move:
			Move (cell.GetID ());
			_camera.Move (cell.transform.position, _duration);

			if (_isRight) {
				_human.RunR (_duration);
				_isRight = false;
			} else {
				_human.RunL (_duration);
				_isRight = true;
			}
			break;

		case enumAction.Attack:
			_human.Attack ();
			break;
		}
	}
		

}
