using UnityEngine;

public class BotController : CharacterController, IBotController {
	public override void Die() { }

	public override void TakeDamage(float damagePoint) { }

	[SerializeField] private int _botPercentageOfMakingRightDecision = 60;

	protected override void Start() {
		base.Start();
		this.InvokeRepeating("ChangeBotShape", 3, 0.2f);
		this.InvokeRepeating("RotationByBot", 3, 0.2f);
	}

	private void OnDestroy() {
		this.CancelInvoke("ChangeBotShape");
		this.CancelInvoke("RotationByBot");
	}

	private void RotationByBot() {
		var decisionIndex = Random.Range(0, 100);
		if (decisionIndex < this._botPercentageOfMakingRightDecision) {
			return;
		}

		this.shapeShooter.RotateShape();
	}

	private void ChangeBotShape() {
		var decisionIndex = Random.Range(0, 100);
		if (decisionIndex < this._botPercentageOfMakingRightDecision) {
			return;
		}

		this.shapeShooter.ChangeShape();
	}


}
