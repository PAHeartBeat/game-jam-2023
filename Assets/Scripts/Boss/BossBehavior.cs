using System.Collections;

using iPAHeartBeat.Core.SignalSystem;

using UnityEngine;

public class BossBehavior : MonoBehaviour
{
	[SerializeField] private Animator animator;  // Animation System for Boss/character
	[SerializeField] private int defaultDamage = 10;  // Default damage point
	[SerializeField] private float attackInterval = 2f;  // Interval between attacks
	[SerializeField] private bool autoAttack = false;  // Flag to enable auto-attack
	[SerializeField] private bool attackDamageViaAnimation = false;  // Flag to Apply damage after or before animation finish
	[SerializeField] private Attack[] attacks;  // Array of attack styles

	private bool isActive = false;  // Flag to check if the boss is active
	private bool canAttack = true;  // Flag to check if the boss can attack
	private bool canApplyDamage = true;  // Flag to check if attack can damage or not. it's avoid double damage from Animation.
	private Coroutine autoAttackCoroutine;  // Reference to the auto-attack coroutine
	private Attack currentAttack = null;

	// Function to activate the boss
	public void ActivateBoss()
	{
		isActive = true;

		if (autoAttack)
			StartAutoAttack();
	}

	// Function to deactivate the boss
	public void DeactivateBoss()
	{
		isActive = false;
		StopAutoAttack();
	}

	// Function to perform an attack based on the given attack style index
	public void Attack(int styleIndex)
	{
		if (!isActive || styleIndex < 0 || styleIndex >= attacks.Length || !canAttack)
			return;

		canAttack = false;
		canApplyDamage = true;
		currentAttack = attacks[styleIndex];
		Debug.Log("Boss attacked with style " + styleIndex + " for " + currentAttack.damage + " damage.");
		this.animator?.Play(currentAttack.attackStyle.ToString());
		if (!attackDamageViaAnimation)
		{
			ApplyAttackDamage();
		}

		StartCoroutine(ResetAttack());
	}

	// Function will execute by the Animator when particular Attack animation finished.
	public void ApplyAttackDamage()
	{
		if (canApplyDamage) return;

		canApplyDamage = false;
		var damageInfo = new BossAttackSignal
		{
			Damage = currentAttack?.damage ?? 0,
		};

		SignalManager.Me.Fire<BossAttackSignal>(damageInfo);
	}

	// Coroutine to reset the attack flag after the attack interval
	private IEnumerator ResetAttack()
	{
		yield return new WaitForSeconds(attackInterval);
		canAttack = true;
	}

	// Function to start auto-attack coroutine
	private void StartAutoAttack()
	{
		autoAttackCoroutine ??= StartCoroutine(AutoAttack());
	}

	// Function to stop auto-attack coroutine
	private void StopAutoAttack()
	{
		if (autoAttackCoroutine != null)
		{
			StopCoroutine(autoAttackCoroutine);
			autoAttackCoroutine = null;
		}
	}

	// Coroutine to perform auto-attack
	private IEnumerator AutoAttack()
	{
		while (isActive && autoAttack)
		{
			if (canAttack)
			{
				int randomStyle = Random.Range(0, attacks.Length);
				Attack(randomStyle);
			}

			yield return new WaitForSeconds(attackInterval);
		}
	}
}
