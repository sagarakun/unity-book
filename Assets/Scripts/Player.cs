using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Player : MonoBehaviour
{
	/* - - - - - - - - - - - - - */

	[SerializeField] private Map _map;
	private float _duration;
	private Human _human;
	private Vector2 _id;
	private Vector3 _prevPos;
	private int _margin;
	private bool _isRight;

	private enum enumVectorPlayer
	{
		Front,
		Back,
		Right,
		Left
	}

	private enum enumActionPlayer
	{
		Attack,
		Move,
		Damage,
	}

	/* - - - - - - - - - - - - - */

	public Vector2 GetID ()
	{
		return _id;
	}

	public void Damage ()
	{
		_human.Damage ();
	}

	/* - - - - - - - - - - - - - */

	private void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	private IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_isRight = false;
		_human = transform.Find ("human").GetComponent<Human> ();
		yield break;
	}

	private void OnTriggerEnter (Collider other)
	{
		var cell = other.gameObject.GetComponent<Cell> ();
		if (cell != null) {
			_id = cell.GetID ();
			var pos = cell.transform.position;
			transform.position = pos;
			_prevPos = pos;
		}
	}

	/* - - - - - - - - - - - - - */

	public IEnumerator SequenceSetPropaty (float duration, int margin)
	{
		_duration = duration;
		_margin = margin;
		_human.SetDuration (_duration);
		yield break;
	}

	//アクションのログをとる
	public IEnumerator SequenceInputAction (int x, int z)
	{
		//キャラの方向変更
		var dur = _duration / 2.0f;
		if (x == 1)
			transform.DOLocalRotate (new Vector3 (0, 90, 0), dur);
		else if (x == -1)
			transform.DOLocalRotate (new Vector3 (0, -90, 0), dur);
		else if (z == 1)
			transform.DOLocalRotate (new Vector3 (0, 0, 0), dur);
		else if (z == -1)
			transform.DOLocalRotate (new Vector3 (0, 180, 0), dur);

		var vec = new Vector2 (x, z);
		var xr = _id.x + vec.x;
		var yr = _id.y + vec.y;
		var cell = _map.GetListCells () [(int)xr] [(int)yr];

		if (!IsCellActive (cell))
			yield break;

		enumActionPlayer type; 
		if (cell.GetObj () != null) {//敵のいるcellに進もうとした場合			
			type = enumActionPlayer.Attack;
			var enemy = cell.GetObj ().GetComponent<Enemy> ();
			enemy.Damage (_id);
		} else {//何も無いcellに進もうとした場合
			type = enumActionPlayer.Move;
		}

		yield return StartCoroutine (SequenceAction (type, vec));

		yield break;
	}

	//アクションを実行
	private IEnumerator SequenceAction (enumActionPlayer type, Vector2 vec)
	{
		switch (type) {
		case enumActionPlayer.Move:
			var xr = _id.x + vec.x;
			var yr = _id.y + vec.y;

			var prevCell = _map.GetListCells () [(int)_id.x] [(int)_id.y];
			var cell = _map.GetListCells () [(int)xr] [(int)yr];

			var pos = cell.transform.position;
			transform.DOMove (pos, _duration);
			cell.SetObj (gameObject);
			prevCell.SetObj (null);
			_id = cell.GetID ();
			_prevPos = pos;

			if (_isRight) {
				_human.RunR (_duration);
				_isRight = false;
			} else {
				_human.RunL (_duration);
				_isRight = true;
			}

			yield return new WaitForSeconds (_duration);
			break;

		case enumActionPlayer.Attack:
			_human.Attack ();
			yield return new WaitForSeconds (_duration);
			break;
		}
		yield break;
	}

	private void CellChange ()
	{
		
	}

	//cellがActiveな所しか進めない + cellの配置されてない場所には進めない
	private bool IsCellActive (Cell cell)
	{
		if (cell == null)
			return false;

		if (!cell.GetIsActive ())
			return false;
		else
			return true;
	}
}
