using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitTarget
{
	void onHit ();
}

public class Monster : MonoBehaviour , IHitTarget
{

	protected enum States
	{
		Stop,
		Normal,
		Forward,
		Attack,
		KeepMove,
		KeepToGone,
		Die
	}

	public int hp = 30;
	protected States state = States.Normal;
	protected float offset_x = 2f;
	protected float offset_z = 6f;
	protected float offset_range = 2f;


	protected Vector3 target;

	protected static bool isAttack = false;

	protected float atktime = 0f;
	protected float disappearTime = 0f;
	protected Animator ani;

	void Awake ()
	{
		init ();
	}

	protected virtual void init ()
	{
		ani = this.GetComponent<Animator> ();
		offset_x = Random.Range (-offset_range, offset_range);
		offset_z = Random.Range (-offset_range, offset_range);
	}

	private bool isRunSh = false;

	// Update is called once per frame
	public virtual void onHit ()
	{
		if (this.state == States.Die || this.state == States.KeepToGone)
			return;
		
		this.hp -= 6;

		if (!isRunSh) {
			StartCoroutine (changeShader ());
		}

		if (this.hp <= 0) {
			this.state = States.Die;
		}
	}

	private IEnumerator changeShader ()
	{
		isRunSh = true;
		this.GetComponentInChildren<SkinnedMeshRenderer> ().material.shader = Shader.Find ("Custom/White");
		yield return new WaitForSeconds (.05f);
		this.GetComponentInChildren<SkinnedMeshRenderer> ().material.shader = Shader.Find ("Standard (Specular setup)");
		yield return new WaitForSeconds (.05f);
		isRunSh = false;
	}
}
