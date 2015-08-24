﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	private Vector3 _offsetPosition;

	/* - - - - - - - - - - - - - */
	private void Start ()
	{
		DOTween.Init ();
		_offsetPosition = transform.position;
	}

	public void Move (Vector3 pos, float duration)
	{
		float x = pos.x + _offsetPosition.x;
		float y = pos.y + _offsetPosition.y;
		float z = pos.z + _offsetPosition.z;

		Vector3 vec = new Vector3 (x, y, z);
		transform.DOMove (vec, duration * 10);
	}
}
