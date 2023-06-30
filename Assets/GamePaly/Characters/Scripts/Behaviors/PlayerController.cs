public class PlayerController : CharacterController, IPlayerController {
	public override void Die() { }

	public override void TakeDamage(float damagePoint) { }

	protected override void Start() {
		base.Start();
		this.thirdPersonCamera.target = this.cannonTransform;
		;
	}

}
