using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{

	[SerializeField]
	public Image hpbar;

	[SerializeField]
	private Image bite;

	[SerializeField]
	private CameraPathAnimator animator;

	public float maxHp = 1000;
	public float playerHp = 100;

	private static GameController _instance;

	public static GameController instance { get { return _instance; } }

	public static UnityEvent stopShack = new UnityEvent ();
	public static UnityEvent startSeaCockroaches = new UnityEvent ();
	public static UnityEvent stopSeacockroaches = new UnityEvent ();
	public Transform[] atk;

	void Awake ()
	{
		_instance = this;

		animator.AnimationCustomEvent += OnCustomEvent;

		playerHp = maxHp;
	}

	public static void hit ()
	{
		_instance.hitPlayer ();
	}

	public void hitPlayer ()
	{
		if (playerHp > 0) {
			playerHp -= maxHp / 50;
			hpbar.fillAmount = playerHp / maxHp;

			if (!bite.enabled) {
				bite.enabled = true;
				StartCoroutine (this.disableBiteImage ());
			}
			animator.enabled = false;
			Camera.main.GetComponentInParent<WHCameraShake> ().doShake ();
			StartCoroutine (countdownToRestartAnimator ());
		} else {
			hpbar.fillAmount = 0f;
		}
	}

	protected IEnumerator countdownToRestartAnimator ()
	{
		yield return new WaitForSeconds (0.5f);
		animator.enabled = true;
	}

	private IEnumerator disableBiteImage ()
	{
		yield return new WaitForSeconds (1f);
		bite.enabled = false;
	}

	private void OnCustomEvent (string eventname)
	{
		switch (eventname) {
		case "StopShark":
			if (stopShack != null) {
				stopShack.Invoke ();
			}
			break;
		case "Start Cockroaches":
			if (startSeaCockroaches != null) {
				startSeaCockroaches.Invoke ();
			}
			break;

		case "Stop Cockroaches":
			if (stopSeacockroaches != null) {
				stopSeacockroaches.Invoke ();
			}
			break;
		}
	}

}
