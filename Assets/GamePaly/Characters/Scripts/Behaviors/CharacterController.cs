using System;
using iPAHeartBeat.Core.Extensions;
using UnityEngine;

public abstract class CharacterController : CharacterBehaviour, ICharacterController {
#pragma warning disable IDE0044 // Make field readonly
	[Header("Core Player infomration")]
	[SerializeField] protected ThirdPersonCamera thirdPersonCamera;
	[SerializeField] protected Transform cannonTransform;            // Transform of the cannon
	[SerializeField] protected TetrisShapeShooter shapeShooter;
	[SerializeField] protected float smoothness = 0.5f;
	[SerializeField] protected float recoilDistance = 3f;            // Distance the player is pushed back when the cannon fires
	[SerializeField] protected float moveSpeed = 5f;                  // Speed at which the player moves
#pragma warning restore IDE0044 // Make field readonly

	public bool IsActive => !this.thirdPersonCamera.isCinematicView;

	public Transform BulletInitPoint => this.cannonTransform;

	protected Transform trCache;

	public void ApplyRecoil() {
		// Calculate the recoil direction
		var recoilDirection = -this.cannonTransform.forward;

		// Push the player back
		this.trCache.position += recoilDirection * this.recoilDistance;
	}

	#region  Mono Action
#pragma warning disable IDE0051 // private member is unused.
	protected override void Start() {
		base.Start();
		this.trCache = this.transform;
		if (this.thirdPersonCamera.IsNull()) {
			_ = this.TryToGetCameraController();
		}
	}

	protected virtual void Update() {
		if (this.thirdPersonCamera.isCinematicView)
			return;

		// Move the player automatically
		// var targetZ = this.trCache.position.z + (this.moveSpeed * Time.smoothDeltaTime);
		// var targetPosition = new Vector3(this.trCache.position.x, this.trCache.position.y, targetZ);
		// this.trCache.position = Vector3.Lerp(this.trCache.position, targetPosition, this.smoothness);
		this.transform.Translate(Vector3.forward * this.moveSpeed * Time.smoothDeltaTime, Space.Self);
	}
#pragma warning restore IDE0051 // private member is unused.
	#endregion

	protected virtual bool TryToGetCameraController() {
		var mainCam = Camera.main;
		if (!mainCam.IsNotNull()) {
			this.thirdPersonCamera = mainCam.GetComponent<ThirdPersonCamera>();

		}

		if (this.thirdPersonCamera.IsNull()) {
			this.thirdPersonCamera = GameObject.FindObjectOfType<ThirdPersonCamera>();
		}

		if (this.thirdPersonCamera.IsNull()) {
			throw new Exception("Camera Not Found so Can't update target to follow Camera");
		}

		return true;
	}
}
