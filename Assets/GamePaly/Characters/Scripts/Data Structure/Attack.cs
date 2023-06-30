// AttackStyle class to define an attack style with a specific damage point
[System.Serializable]
public class Attack {
	public float damageMultiplier;  // Damage point for the attack style
	public AttackStyles attackStyle; // Attack style which will be used for identify Animation

	public Attack() {
		this.damageMultiplier = 1f;
		this.attackStyle = AttackStyles.FireBullet;
	}
}
