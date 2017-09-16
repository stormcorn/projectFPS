using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	public float fireDelta = 0.2f;
	private float nextFire = 0F;
	private float myTime = 0.0F;

	private int fireTimes = 0;
	private int keepFire = 12;

	[SerializeField]
	private GameObject weapon;

	[SerializeField]
	private ParticleSystem gunParticle;
	
	// Update is called once per frame
	void Update () {
		myTime = myTime + Time.deltaTime;
		Ray ray = this.GetComponent<Camera>().ScreenPointToRay (Input.mousePosition);
		weapon.transform.LookAt (ray.direction * int.MaxValue);

		if (Input.GetButton ("Fire1") && fireTimes < keepFire && myTime > nextFire ) {

			fireTimes++;

			if (fireTimes >= keepFire) {
				nextFire = myTime + fireDelta;	
				nextFire = nextFire - myTime;
				myTime = 0.0F;
				fireTimes = 0;
			}

			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast (ray, out hit);

			if (hit.collider != null) {
				IHitTarget target = hit.collider.GetComponentInParent<IHitTarget>();
				if (target != null) {
					target.onHit ();
				}
			}

			playGunParticle ();
		}

	}

	void playGunParticle () {
		gunParticle.Play ();
	}
}
