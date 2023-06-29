using UnityEngine;

public abstract class CharacterBehaviour : MonoBehaviour {

	protected virtual void Awake() { }
	protected virtual void Start() { }

	protected virtual void OnTriggerEnter(Collider other) {

	}

	protected virtual void OnCollisionEnter(Collision other) {

	}

	public abstract void TakeDamage(float damagePoint);
	public abstract void Die();
}
