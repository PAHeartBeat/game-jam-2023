using UnityEngine;

/// <summary>
/// Represents the abstract behavior of any character.
/// </summary>
public abstract class CharacterBehaviour : MonoBehaviour {
#pragma warning disable IDE0044 // Make Field Read-only
	// Animation System for Boss/character
	[Header("Common Behaviour for all kind of Characters")]
	[SerializeField] protected Animator animator;

	// Animation state name for Character die.
	[SerializeField] protected string dieAnimation = "Die";

	// Animation state name for Character idle.
	[SerializeField] protected string idleAnimation = "Idle";

	/// <summary>
	/// Indicates whether the attack can cause damage. This flag avoids double damage from animation.
	/// </summary>
	[SerializeField] protected bool canApplyDamage = false;
#pragma warning restore IDE0044 // Make Field Read-only
	// protected Transform startPos;

	/// <summary>
	/// Takes damage and reduces the health.
	/// </summary>
	/// <param name="damagePoint">The amount of damage to take.</param>
	public abstract void TakeDamage(float damagePoint);

	/// <summary>
	/// Updates the animation, sends signals to update UI or credit reward.
	/// </summary>
	public abstract void Die();

	/// <summary>
	/// Tries to get damage information from the entered object.
	/// </summary>
	/// <param name="other">The collider of the other object.</param>
	/// <param name="damageAmount">The amount of damage retrieved.</param>
	/// <returns>True if damage information was retrieved successfully, false otherwise.</returns>
	protected bool TryToGetDamageInfo(GameObject other, out float damageAmount) {
		damageAmount = 0f;
		return false;
	}

	/// <summary>
	/// Mono internal method called when the object is awakened.
	/// </summary>
	protected virtual void Awake() {
		// startPos = this.transform;
	}

	/// <summary>
	/// Mono internal method called when the object is started.
	/// </summary>
	protected virtual void Start() { }

	/// <summary>
	/// Mono internal method called when the object is enabled.
	/// </summary>
	protected virtual void OnEnable()
		=> this.animator?.SetTrigger(this.idleAnimation);

	/// <summary>
	/// Mono internal method called when the object is disabled.
	/// </summary>
	protected virtual void OnDisable() { }

	/// <summary>
	/// Mono internal method called when another object enters the collider.
	/// </summary>
	protected virtual void OnTriggerEnter(Collider other) {
		if (this.TryToGetDamageInfo(other.gameObject, out var damage)) {
			this.TakeDamage(damage);
		}
	}

	/// <summary>
	/// Mono internal method called when another object collides with the collider.
	/// </summary>
	protected virtual void OnCollisionEnter(Collision other) {
		if (this.TryToGetDamageInfo(other.gameObject, out var damage)) {
			this.TakeDamage(damage);
		}
	}
}
