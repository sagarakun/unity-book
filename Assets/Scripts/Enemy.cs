using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	[SerializeField] private Map _map;
	private float _duration;
	private List<ActionData> _listActionLog;
	private Human _human;
	private Vector2 _id;
	private Vector3 _prevPos;
	private int _margin;
	private bool _isMoving;
	private bool _isRight;

	public void SetPropaty (float duration, int margin)
	{
		_duration = duration;
		_margin = margin;
		_human.SetDuration (_duration);
	}

	public void SetID (Vector2 vec)
	{
		_id = vec;
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

	/* - - - - - - - - - - - - - */

	private void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	private IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_isRight = false;
		_listActionLog = new List<ActionData> ();
		_human = transform.Find ("human").GetComponent<Human> ();
		yield break;
	}

	public void TurnAction ()
	{
		_human.Attack ();
	}
}