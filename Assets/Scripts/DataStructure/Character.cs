public abstract class Character : Entity {
	public float rewardMultiplier = 1;
	public float movementSpeed;
	public long damage;
	public float attackInterval;

	public bool autoAttack;
	public Character() : base() {
		this.rewardMultiplier = 1f;
		this.movementSpeed = 2;
		this.damage = 10;
		this.attackInterval = 5f;
		this.autoAttack = false;
	}
}
