using UnityEngine;

public abstract class CharacterController : MonoBehaviour, ICharacterController {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] protected ThirdPersonCamera thirdPersonCamera;
	[SerializeField] protected Transform cannonTransform;            // Transform of the cannon
	[SerializeField] protected TetrisShapeShooter shapeShooter;
	[SerializeField] protected float smoothness = 0.5f;
	[SerializeField] protected float recoilDistance = 3f;            // Distance the player is pushed back when the cannon fires
	[SerializeField] protected float moveSpeed = 5f;                  // Speed at which the player moves
#pragma warning restore IDE0044 // Make field readonly

	public bool IsActive => !this.thirdPersonCamera.isCinematicView;

	public Transform BulletInitPoint { get => this.cannonTransform; }

	protected Transform trCache;

	public void ApplyRecoil() {
		// Calculate the recoil direction
		var recoilDirection = -this.cannonTransform.forward;

		// Push the player back
		this.trCache.position += recoilDirection * this.recoilDistance;
	}

	#region  Mono Action
#pragma warning disable IDE0051 // private member is unused.
	protected virtual void Start()
		=> this.trCache = this.transform;

	protected virtual void Update() {
		if (this.thirdPersonCamera.isCinematicView)
			return;

		// Move the player automatically
		var targetZ = this.trCache.position.z + (this.moveSpeed * Time.smoothDeltaTime);
		var targetPosition = new Vector3(this.trCache.position.x, this.trCache.position.y, targetZ);
		this.trCache.position = Vector3.Lerp(this.trCache.position, targetPosition, this.smoothness);
	}
#pragma warning restore IDE0051 // private member is unused.
	#endregion
}
