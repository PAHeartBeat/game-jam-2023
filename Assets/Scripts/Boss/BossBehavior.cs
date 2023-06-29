using System.Collections;
using iPAHeartBeat.Core.SignalSystem;
using UnityEngine;

/// <summary>
/// Represents the behavior of the boss character.
/// </summary>
public class BossBehavior : MonoBehaviour {
#pragma warning disable IDE0044 // Make Field Read-only
	[SerializeField] private Animator _animator; // Animation System for Boss/character
	[SerializeField] private int _defaultDamage = 10; // Default damage point
	[SerializeField] private float _attackInterval = 2f; // Interval between attacks
	[SerializeField] private bool _autoAttack = false; // Flag to enable auto-attack
	[SerializeField] private bool _attackDamageViaAnimation = false; // Flag to apply damage after or before animation finish
	[SerializeField] private Attack[] _attacks; // Array of attack styles
#pragma warning restore IDE0044 // Make Field Read-only

	private bool _isActive = false; // Flag to check if the boss is active
	private bool _canAttack = true; // Flag to check if the boss can attack
	private bool _canApplyDamage = true; // Flag to check if attack can damage or not. It avoids double damage from Animation.
	private Coroutine _autoAttackCoroutine; // Reference to the auto-attack coroutine
	private Attack _currentAttack = null;

	/// <summary>
	/// Activates the boss behavior.
	/// </summary>
	public void ActivateBoss() {
		this._isActive = true;

		if (this._autoAttack)
			this.StartAutoAttack();
	}

	/// <summary>
	/// Deactivates the boss behavior.
	/// </summary>
	public void DeactivateBoss() {
		this._isActive = false;
		this.StopAutoAttack();
	}

	/// <summary>
	/// Performs an attack based on the given attack style index.
	/// </summary>
	/// <param name="styleIndex">The index of the attack style.</param>
	public void Attack(int styleIndex) {
		if (!this._isActive || styleIndex < 0 || styleIndex >= this._attacks.Length || !this._canAttack)
			return;

		this._canAttack = false;
		this._canApplyDamage = true;
		this._currentAttack = this._attacks[styleIndex];

		Debug.Log($"Boss attacked with style {styleIndex} for {this._currentAttack.damage} damage.");
		this._animator?.Play(this._currentAttack.attackStyle.ToString());
		if (!this._attackDamageViaAnimation) {
			this.ApplyAttackDamage();
		}

		_ = this.StartCoroutine(this.ResetAttack());
	}

	/// <summary>
	/// Applies the attack damage. This method is executed by the Animator when the attack animation finishes.
	/// </summary>
	public void ApplyAttackDamage() {
		if (!_canApplyDamage)
			return;

		this._canApplyDamage = false;
		var damageInfo = new BossAttackSignal {
			damage = this._currentAttack?.damage ?? 0
		};

		SignalManager.Me.Fire<BossAttackSignal>(damageInfo);
	}

	/// <summary>
	/// Coroutine to reset the attack flag after the attack interval.
	/// </summary>
	private IEnumerator ResetAttack() {
		yield return new WaitForSeconds(this._attackInterval);
		this._canAttack = true;
	}

	/// <summary>
	/// Starts the auto-attack coroutine.
	/// </summary>
	private void StartAutoAttack() => this._autoAttackCoroutine ??= this.StartCoroutine(this.AutoAttack());

	/// <summary>
	/// Stops the auto-attack coroutine.
	/// </summary>
	private void StopAutoAttack() {
		if (this._autoAttackCoroutine != null) {
			this.StopCoroutine(this._autoAttackCoroutine);
			this._autoAttackCoroutine = null;
		}
	}

	/// <summary>
	/// Coroutine to perform auto-attack.
	/// </summary>
	private IEnumerator AutoAttack() {
		while (this._isActive && this._autoAttack) {
			if (this._canAttack) {
				var randomStyle = Random.Range(0, this._attacks.Length);
				this.Attack(randomStyle);
			}

			yield return new WaitForSeconds(this._attackInterval);
		}
	}
}
