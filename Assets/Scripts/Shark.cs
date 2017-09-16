using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : Monster
{
	private float step;

	public int double_attack_rate = 20;

	private float maxSpeed = 17f;
	private float speed = 10f;
	private float av = 2f;
	private float atkspeed = 15f;
	private float atkTime = 1f;
	private float atkCD = 3f;

	private Transform camTrans;
	private float def_y;
	//	private float atk_offset_range = float.MinValue;
	//	private int shark_atk_count = 0;
	private static List<GameObject> atkShakers = new List<GameObject> ();
	private static bool isAllowAtk = true;
	private float nextRandomMove = 0f;
	private int atkTargetIndex = 0;

	protected override void init ()
	{
		base.init ();
		GameController.stopShack.AddListener (() => changeState (States.KeepToGone));

		camTrans = Camera.main.transform;
		def_y = this.transform.position.y;
	}

	void Update ()
	{
		switch (state) {
		case States.Normal:
			atkTime += Time.deltaTime;
			target = camTrans.position + camTrans.forward * 5f;
			target.y = this.transform.position.y;
			target.x += offset_x;
			target.z += offset_z;

			var dis = (target - this.transform.position).magnitude;
			if (speed < maxSpeed) {
				speed += (av * Time.deltaTime);
			}

			if (dis > 2f) {
				float step = speed * Time.deltaTime;
				//move
				transform.position = Vector3.MoveTowards (transform.position, target, step);
				this.transform.LookAt (target);

				nextRandomMove += Time.deltaTime;
				if (nextRandomMove > 1f) {
					randomMoveState ();	
				}
			} else {
				atkTargetIndex = atkShakers.Count;
				if (isAllowAtk && atkTime >= atkCD && atkShakers.Count < 2) {
					isAllowAtk = false;
					//random attack 
					if (Random.Range (0, 100) < double_attack_rate) {
						isAllowAtk = true;
					}
					atkShakers.Add (this.gameObject);
					changeState (States.Attack);
					ani.SetTrigger ("jump");
				} else {
					speed = 6f;
				}
			}
			break;

		case States.Forward:
			transform.Translate (Vector3.forward * .2f);
			target = Camera.main.transform.position + Camera.main.transform.forward * 15f;
			target.y = 0;
			if (Vector3.Distance (target, this.transform.position) > 15f) {
				changeState (States.Normal);
			}
			break;

		case States.Attack:
			step = atkspeed * Time.deltaTime;
			target = GameController.instance.atk [atkTargetIndex].position;
			this.transform.LookAt (camTrans);
			transform.position = Vector3.MoveTowards (transform.position, target, step);
			break;

		case States.KeepToGone:
			disappearTime += Time.deltaTime;
			if (disappearTime > 3f) {
				this.gameObject.SetActive (false);
			}
			break;

		case States.KeepMove:
			transform.Translate (Vector3.forward * .3f);
			break;


		case States.Die:
			ani.SetTrigger ("dead");
			changeState (States.KeepToGone);
			reset_y ();
			transform.LookAt (this.transform.position + Vector3.forward);
			break;
		}
	}

	public override void onHit ()
	{
		if (state != States.Die) {
			base.onHit ();
			if (this.hp <= 0) {
				resetAttackFlag ();
			}	
		}
	}

	public void SharkAttack ()
	{
		GameController.hit ();
		resetAttackFlag ();
	}

	public void SharkEvent ()
	{
		this.overAttack ();
	}

	private void overAttack ()
	{
		this.transform.position = new Vector3 (this.transform.position.x, def_y, this.transform.position.z);
		target = camTrans.position;
		target.y = this.transform.position.y;
		target.x += offset_x;
		target.z += offset_z;
		this.transform.LookAt (target);
		if (this.hp > 0) {
			changeState (States.Normal);
		}
		resetAttackFlag ();

	}

	private Coroutine decor;

	private void resetAttackFlag ()
	{
		if (decor != null)
			StopCoroutine (decor);
		
		if (atkShakers.Contains (this.gameObject)) {			
			decor = StartCoroutine (delayToResetFlag ());
		}
	}

	private IEnumerator delayToResetFlag ()
	{
		yield return new WaitForSeconds (0.1f);
		atkTime = 0f;
		atkShakers.Remove (this.gameObject);
		isAllowAtk = (atkShakers.Count == 0);
	}

	private void reset_y ()
	{
		Vector3 pos = this.transform.position;
		pos.y = this.def_y;
		this.transform.position = pos;
	}

	void randomMoveState ()
	{
		if (ani.GetCurrentAnimatorStateInfo (0).IsName ("move")) {
			int rAni = Random.Range (0, 4);
			switch (rAni) {
			case 0:
				break;
			case 1:
				ani.SetTrigger ("m2");
				break;
			case 2:
				ani.SetTrigger ("m3");
				break;
			case 3:
				ani.SetTrigger ("m4");
				break;
			}
			nextRandomMove = 0f;
		}
	}

	void changeState (States targetState)
	{
		if (state == States.KeepToGone) {
			return;
		} else {
			state = targetState;
		}
	}
}
