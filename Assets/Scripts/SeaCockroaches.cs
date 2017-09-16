using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaCockroaches : Monster
{
	private float speed = 20f;

	private bool atkflag = false;
	private float stay = 0f;
	private float stayTime = 10f;
	private bool toFall = false;

	private static SeaCockroaches seaAtker = null;

	protected override void init ()
	{
		base.init ();
		GameController.startSeaCockroaches.AddListener (() => this.state = States.Normal);
		GameController.stopSeacockroaches.AddListener (() => this.state = States.KeepToGone);
		this.state = States.Stop;
	}

    // Update is called once per frame
    void Update ()
	{
		switch (state) {    //狀態切換
		case States.Stop:   //停止
			break;
		case States.Normal: //正常
			transform.Translate (Vector3.forward * .06f);
			if (!isAttack) {    //若沒攻擊，則攻擊，轉為攻擊狀態
				isAttack = true;
				this.state = States.Attack;
			}
			break;

		case States.Attack: //攻擊
			if (!atkflag) {
				ani.SetTrigger ("atk"); //撥放攻擊動畫
				atkflag = true;
				seaAtker = this;
				resetAttackFlag ();
			}
				
			float step = speed * Time.deltaTime;
			Transform camTrans = Camera.main.transform;
			target = camTrans.position + camTrans.forward * 1.5f;
			transform.position = Vector3.MoveTowards (transform.position, target, step);

			this.transform.rotation = Quaternion.Euler (0f, 0f, -81f);
			var dis = (target - transform.position).magnitude;

			if (dis < 0.1f) {
				GameController.hit ();
				this.stay += Time.deltaTime;
				this.transform.SetParent (Camera.main.transform);

				if (stay > stayTime) {
					this.state = States.KeepMove;	
				}
			}

			break;

		case States.KeepMove:   //持續行動
			if (!toFall) {  //若沒掉落
				Rigidbody body = this.gameObject.AddComponent<Rigidbody> ();    //加入剛體
				body.AddForce (Vector3.down);
				resetAttackFlag ();
				toFall = true;
			}
			break;

		case States.KeepToGone: //持續到消失
			disappearTime += Time.deltaTime;
			if (disappearTime > 5f) {  //持續的時間
				this.gameObject.SetActive (false);
			}
			break;

		case States.Die:    //死亡
			ani.SetTrigger ("dead");
			this.GetComponentInChildren<CapsuleCollider> ().enabled = false;
			this.state = States.KeepToGone;
			break;
		}
	}

	public override void onHit ()
	{
		base.onHit ();
	}

	private Coroutine delayCoroutine;

	private void resetAttackFlag ()
	{
		if (seaAtker == this) {
			if (delayCoroutine != null)
				StopCoroutine (delayCoroutine);
			delayCoroutine = StartCoroutine (delayToResetFlag ());
		}
	}

	private IEnumerator delayToResetFlag ()
	{
		yield return new WaitForSeconds (0.3f);
		isAttack = false;
		seaAtker = null;
	}
		
}
