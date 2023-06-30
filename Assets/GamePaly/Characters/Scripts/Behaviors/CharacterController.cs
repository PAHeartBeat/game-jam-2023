using System;
using System.Collections;
using iPAHeartBeat.Core.Extensions;
using UnityEngine;

public abstract class CharacterController : CharacterBehaviour, ICharacterController {
#pragma warning disable IDE0044 // Make field readonly
	[Header("Core Player information")]
	[SerializeField] protected string runAnimationState = "Run";
	[SerializeField] protected string smashAnimationState = "Smashing";
	[SerializeField] protected ThirdPersonCamera thirdPersonCamera;
	[SerializeField] protected Transform cannonTransform;            // Transform of the cannon
	[SerializeField] protected TetrisShapeShooter shapeShooter;
	[SerializeField] protected float smoothness = 0.5f;
	[SerializeField] protected float recoilDistance = 3f;            // Distance the player is pushed back when the cannon fires
	[SerializeField] protected float moveSpeed = 5f;                  // Speed at which the player moves
#pragma warning restore IDE0044 // Make field readonly

	public bool IsActive => !this.thirdPersonCamera.isCinematicView;
	public bool IsSmashing { get; private set; }

	public Transform BulletInitPoint => this.cannonTransform;

	protected Transform trCache;
	private bool _isTriggerChecked = false;

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

		this.animator.Play("Idle");
		_ = this.StartCoroutine(this.StartRunOnceActive());
	}

	protected virtual bool Update() {
		if (!this.IsActive || this.IsSmashing)
			return false;

		// Move the player automatically
		var targetZ = this.transform.position.z + (this.moveSpeed * Time.smoothDeltaTime);
		var targetPosition = new Vector3(this.transform.position.x, this.transform.position.y, targetZ);
		this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, this.smoothness);
		// this.transform.Translate(Vector3.forward * this.moveSpeed * Time.smoothDeltaTime, Space.Self);

		return true;
	}

	protected override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
		Debug.Log($"Trigger happened in {this.name} with {other?.name ?? ""}");
		if (this._isTriggerChecked) return;

		var obstacle = other.GetComponentInParent<IObstacles>();
		if (obstacle == null) {
			return;
		}

		this._isTriggerChecked = true;
		this.StartSmashing(obstacle);
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

		return this.thirdPersonCamera.IsNull()
			? throw new Exception("Camera Not Found so Can't update target to follow Camera")
			: true;
	}

	private void StartRun()
		=> this.animator.Play(this.runAnimationState);

	private IEnumerator StartRunOnceActive() {
		yield return new WaitUntil(() => this.IsActive);
		this.StartRun();
	}

	private void StartSmashing(IObstacles obstacle) {
		this.IsSmashing = true;
		this.animator.Play(this.smashAnimationState);
		_ = this.StartCoroutine(this.DestroyObstacle(obstacle));
	}

	private IEnumerator DestroyObstacle(IObstacles obstacle) {
		yield return new WaitForSeconds(obstacle.DestroyTime);
		obstacle.DestroyObstacle();
		this._isTriggerChecked = false;
		this.IsSmashing = false;
		this.StartRun();
	}
}
