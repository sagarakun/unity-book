using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 人形のアニメーションを操作するクラス
/// </summary>
public class Human : MonoBehaviour
{
	private Animator _animator;
	private int _hushAttack;
	private int _hushRunL;
	private int _hushRunR;
	private int _hushDamage;
	private int _hushDead;

	private ParticleSystem _dashFX;
	private ParticleSystem _attackFX;
	private ParticleSystem _deadFX;
	private float _duration;

	/// <summary>
	/// Animationの秒数指定
	/// </summary>
	public void SetDuration (float duration)
	{
		_duration = duration;
	}

	/// <summary>
	/// 死亡時アニメーション
	/// </summary>
	public void Dead ()
	{
		transform.DOKill ();
		_animator.SetBool (_hushDead, true);
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnDead);
	}

	/// <summary>
	/// ダメージアニメーション
	/// </summary>
	public void Damage ()
	{
		transform.DOKill ();
		_attackFX.Play ();
		_animator.SetBool (_hushDamage, true);
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnAction);
	}

	/// <summary>
	/// 攻撃アニメーション
	/// </summary>
	public void Attack ()
	{
		transform.DOKill ();
		_animator.SetBool (_hushAttack, true);
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnAction);
	}

	/// <summary>
	/// 右足で走るアニメーション
	/// </summary>
	public void RunR ()
	{
		transform.DOKill ();
		_dashFX.Play ();
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnAction);
		_animator.SetBool (_hushRunR, true);
	}

	/// <summary>
	/// 左足で走るアニメーション
	/// </summary>
	public void RunL ()
	{
		transform.DOKill ();
		_dashFX.Play ();
		transform.DOLocalJump (Vector3.zero, 1, 1, _duration).OnComplete (OnAction);
		_animator.SetBool (_hushRunL, true);
	}

	/// <summary>
	/// 生成時コールバック
	/// </summary>
	private void Awake ()
	{
		StartCoroutine (SequenceInit ());
	}

	/// <summary>
	/// 初期化コルーチン
	/// </summary>
	private IEnumerator SequenceInit ()
	{
		DOTween.Init ();
		_animator = transform.GetComponent<Animator> ();
		_hushAttack = Animator.StringToHash ("IsAttack");
		_hushRunL = Animator.StringToHash ("IsRunL");
		_hushRunR = Animator.StringToHash ("IsRunR");
		_hushDamage = Animator.StringToHash ("IsDamage");
		_hushDead = Animator.StringToHash ("IsDead");

		_dashFX = transform.parent.Find ("DashFX").GetComponent<ParticleSystem> ();
		_attackFX = transform.parent.Find ("AttackFX").GetComponent<ParticleSystem> ();
		_deadFX = transform.parent.Find ("DeadFX").GetComponent<ParticleSystem> ();

		_dashFX.Stop ();
		_attackFX.Stop ();
		yield break;
	}

	/// <summary>
	/// アニメーションのコールバック
	/// </summary>
	private void OnAction ()
	{
		_dashFX.Stop ();
		transform.localPosition = Vector3.zero;

		_animator.SetBool (_hushRunL, false);
		_animator.SetBool (_hushRunR, false);
		_animator.SetBool (_hushAttack, false);
		_animator.SetBool (_hushDamage, false);
	}

	/// <summary>
	/// 死亡時アニメーションのコールバック
	/// </summary>
	private void OnDead ()
	{
		_deadFX.Play ();
	}
}
