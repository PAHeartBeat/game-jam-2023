using System.Collections;

using iPAHeartBeat.Core.Extensions;
using iPAHeartBeat.Core.SignalSystem;

using UnityEngine;

/// <summary>
/// Represents the behavior of the boss character.
/// </summary>
public class BossBehavior : CharacterBehaviour {
#pragma warning disable IDE0044 // Make field readonly
	[Header("Boss specific Information")]
	/// <summary>
	/// Represents the boss regular information like health, default damage, etc.
	/// </summary>
	[SerializeField] private Boss _bossInfo;

	/// <summary>
	/// Determines whether the damage should be applied after or before the animation finishes.
	/// </summary>
	[SerializeField] private bool _attackDamageViaAnimation = false;

	/// <summary>
	/// An collection of attack styles which can be performed by the character.
	/// </summary>
	[SerializeField] private Attack[] _attacks;
#pragma warning restore IDE0044 // Make field readonly

	/// <summary>
	/// Indicates whether the boss is currently active.
	/// </summary>
	private bool _isActive = false;

	/// <summary>
	/// Indicates whether the boss is able to perform attacks.
	/// </summary>
	private bool _canAttack = true;

	/// <summary>
	/// Reference to the coroutine for auto-attack.
	/// </summary>
	private Coroutine _autoAttackCoroutine;

	/// <summary>
	/// Current attack related information. This is required to deal attack after the animation is over.
	/// </summary>
	private Attack _currentAttack = null;

	/// <summary>
	/// Activates the boss behavior.
	/// </summary>
	public void ActivateBoss() {
		this._isActive = true;

		if (this._bossInfo.autoAttack)
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
		this.canApplyDamage = true;
		this._currentAttack = this._attacks[styleIndex];

		Debug.Log($"Boss attacked with style {styleIndex} for {this._currentAttack.damageMultiplier} damage.");
		this.animator?.Play(this._currentAttack.attackStyle.ToString());
		if (this.animator.IsNull() || !this._attackDamageViaAnimation) {
			this.ApplyAttackDamage();
		}

		_ = this.StartCoroutine(this.ResetAttack());
	}

	/// <summary>
	/// Applies the attack damage. This method is executed by the Animator when the attack animation finishes.
	/// </summary>
	public void ApplyAttackDamage() {
		if (!this.canApplyDamage)
			return;

		this.canApplyDamage = false;
		var finalDamage = this._bossInfo.damage * (this._currentAttack?.damageMultiplier ?? 1f);
		var damageInfo = new BossAttackSignal {
			damage = finalDamage,
		};

		SignalManager.Me.Fire<BossAttackSignal>(damageInfo);
	}

	/// <inheritdoc />
	public override void TakeDamage(float damagePoint) {
		if (damagePoint <= 0) return;

		this._bossInfo.Health -= damagePoint;
		if (this._bossInfo.Health <= 0) {
			this.Die();
		}
	}

	/// <inheritdoc />
	public override void Die() {
		var dieInfo = new BossDiedSignal {
			reward = this._bossInfo.reward * this._bossInfo.rewardMultiplier,
		};
		SignalManager.Me.Fire<BossDiedSignal>(dieInfo);
		this.animator?.Play(this.dieAnimation);
	}

	/// <inheritdoc />
	protected override void OnEnable() {
		base.OnEnable();
		SignalManager.Me.SubscribeSignal<BossDamageSignal>(this.OnBossAttacked);
	}

	/// <inheritdoc />
	protected override void OnDisable() {
		base.OnDisable();
		SignalManager.Me.UnsubscribeSignal<BossDamageSignal>(this.OnBossAttacked);
	}

	/// <summary>
	/// Event handler for boss being attacked by an external source.
	/// </summary>
	/// <param name="data">The BossDamageSignal containing damage information.</param>
	private void OnBossAttacked(BossDamageSignal data) {
		if (!this._isActive) return;
		this.TakeDamage(data?.damage ?? 0);
	}

	/// <summary>
	/// Coroutine to reset the attack flag after the attack interval.
	/// </summary>
	private IEnumerator ResetAttack() {
		yield return new WaitForSeconds(this._bossInfo.attackInterval);
		this._canAttack = true;
	}

	/// <summary>
	/// Starts the auto-attack coroutine.
	/// </summary>
	private void StartAutoAttack()
		=> this._autoAttackCoroutine ??= this.StartCoroutine(this.AutoAttack());

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
		while (this._isActive && this._bossInfo.autoAttack) {
			if (this._canAttack) {
				var randomStyle = Random.Range(0, this._attacks.Length);
				this.Attack(randomStyle);
			}

			yield return new WaitForSeconds(this._bossInfo.attackInterval);
		}
	}
}
