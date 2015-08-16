﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Human : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	private Animator _animator;
	private int _hushAttack;
	private int _hushRunL;
	private int _hushRunR;
	private int _hushDamage;
	private ParticleSystem _dashFX;
	private ParticleSystem _attackFX;
	private float _duration;

	/* - - - - - - - - - - - - - */

	public void SetDuration (float duration)
	{
		_duration = duration;
	}

	public void Damage ()
	{
		_attackFX.Play ();
		_animator.SetBool (_hushDamage, true);
		transform.DOLocalJump (new Vector3 (0, 0, -4), _duration, 1, _duration).OnComplete (OnJump);
	}

	public void Attack ()
	{
		_animator.SetBool (_hushAttack, true);
		transform.DOLocalJump (new Vector3 (0, 0, 4), _duration, 1, _duration).OnComplete (OnJump);
	}

	public void RunR (float duration)
	{
		_dashFX.Play ();
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnJump);
		_animator.SetBool (_hushRunR, true);
	}

	public void RunL (float duration)
	{
		_dashFX.Play ();
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnJump);
		_animator.SetBool (_hushRunL, true);
	}

	/* - - - - - - - - - - - - - */

	private void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	private IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_animator = transform.GetComponent<Animator> ();
		_hushAttack = Animator.StringToHash ("IsAttack");
		_hushRunL = Animator.StringToHash ("IsRunL");
		_hushRunR = Animator.StringToHash ("IsRunR");
		_hushDamage = Animator.StringToHash ("IsDamage");

		_dashFX = transform.parent.Find ("DashFX").GetComponent<ParticleSystem> ();
		_attackFX = transform.parent.Find ("AttackFX").GetComponent<ParticleSystem> ();

		_dashFX.Stop ();
		_attackFX.Stop ();
		yield break;
	}

	private void OnJump ()
	{
		_dashFX.Stop ();
		transform.localPosition = Vector3.zero;

		_animator.SetBool (_hushRunL, false);
		_animator.SetBool (_hushRunR, false);
		_animator.SetBool (_hushAttack, false);
		_animator.SetBool (_hushDamage, false);
	}

	private void OnAttackFX ()
	{
	}
}