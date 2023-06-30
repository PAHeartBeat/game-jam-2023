using UnityEngine;

public class PlayerController : CharacterController, IPlayerController {
	public override void Die() { }

	public override void TakeDamage(float damagePoint) { }

	protected override void Start() {
		base.Start();
		this.thirdPersonCamera.target = this.cannonTransform;
		;
	}

	protected override bool Update() {
		if (!base.Update()) return false;

		// Check for input to change the currently selected shape
		if (Input.GetButtonDown("Fire1")) {
			this.shapeShooter.ChangeShape();
		}

		if (Input.GetButtonDown("Fire1")) {
			this.shapeShooter.RotateShape();
		}

		return false;
	}
}
