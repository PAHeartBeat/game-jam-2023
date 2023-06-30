using UnityEngine;

public class BotController : CharacterController, IBotController {
	public override void Die() { }

	public override void TakeDamage(float damagePoint) { }

	protected override bool Update() {

		Vector3 newRotation = this.animator.gameObject.transform.rotation.eulerAngles;
		newRotation.y = 0f;
		this.animator.gameObject.transform.rotation = Quaternion.Euler(newRotation);

		return base.Update();
	}
}
