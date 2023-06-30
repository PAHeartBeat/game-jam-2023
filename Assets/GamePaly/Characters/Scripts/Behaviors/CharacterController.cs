using System;
using System.Collections;
using System.Collections.Generic;
using iPAHeartBeat.Core.Extensions;
using iPAHeartBeat.Core.SignalSystem;
using UnityEngine;

public abstract class CharacterController : CharacterBehaviour, ICharacterController {
#pragma warning disable IDE0044 // Make field readonly
	[Header("Core Player information")]
	[SerializeField] protected string runAnimationState = "Run";
	[SerializeField] protected string smashAnimationState = "Punch";
	[SerializeField] protected ThirdPersonCamera thirdPersonCamera;
	[SerializeField] protected Transform cannonTransform;            // Transform of the cannon
	[SerializeField] protected TetrisShapeShooter shapeShooter;
	[SerializeField] protected float smoothness = 0.5f;
	[SerializeField] protected float recoilDistance = 3f;            // Distance the player is pushed back when the cannon fires
	[SerializeField] protected float moveSpeed = 5f;

	private int _totalHurdleCount = 7;
	private int _obstacleDestroyCount = 0;

	// Speed at which the player moves
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

		this.animator.SetTrigger("Idle");
		_ = this.StartCoroutine(this.StartRunOnceActive());
	}

	protected virtual bool Update() {
		if (!this.IsActive || this.IsSmashing)
			return false;

		// Move the player automatically
		var targetZ = this.transform.position.z + (this.moveSpeed * Time.smoothDeltaTime);
		var targetPosition = new Vector3(this.transform.position.x, this.transform.position.y, targetZ);
		this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, this.smoothness);

		var newRotation = this.animator.gameObject.transform.rotation.eulerAngles;
		newRotation.y = 0f;
		this.animator.gameObject.transform.rotation = Quaternion.Euler(newRotation);

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
		this.FireResetMultiplierSignal();
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
		=> this.animator.SetTrigger(this.runAnimationState);

	private IEnumerator StartRunOnceActive() {
		yield return new WaitUntil(() => this.IsActive);
		this.StartRun();
	}

	private void StartSmashing(IObstacles obstacle) {
		this.IsSmashing = true;
		this.animator.SetTrigger(this.smashAnimationState);
		_ = this.StartCoroutine(this.DestroyObstacle(obstacle));
	}

	protected IEnumerator DestroyObstacle(IObstacles obstacle) {
		yield return new WaitForSeconds(obstacle.DestroyTime);

		obstacle.DestroyObstacle(this.shapeShooter.IsBot);
		this._isTriggerChecked = false;
		this.IsSmashing = false;
		this._obstacleDestroyCount++;
		if (this._obstacleDestroyCount >= this._totalHurdleCount) {
			this.FireLevelEndSignal();
		} else {
			this.StartRun();
		}

		// 	if (shapeShooter.IsBot) {
		// 		_botHurdleDestroyCount++;
		// 		if (_botHurdleDestroyCount != _totalHurdleCount) {
		// 			this.StartRun();
		// 		} else {
		// 			IsSmashing = true;
		// 			this.animator.SetTrigger(this.idleAnimation);
		// 		}
		// 	} else {
		// 		_playerHurdleDestroyCount++;
		// 		if (_playerHurdleDestroyCount != _totalHurdleCount) {
		// 			this.StartRun();
		// 		} else {
		// 			IsSmashing = true;
		// 			this.animator.SetTrigger(this.idleAnimation);
		// 		}
		// 	}
		//
		// 	this.transform.position = new Vector3(startPos.position.x, startPos.position.y, this.transform.position.z);
		// 	this.transform.rotation = Quaternion.Euler(startPos.rotation.eulerAngles.x, startPos.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
	}

	private void FireResetMultiplierSignal() {
		var restInfo = new MultiplierResetSignal {
			isBot = this.shapeShooter.IsBot
		};

		SignalManager.Me.Fire<MultiplierResetSignal>(restInfo);
	}

	private void FireLevelEndSignal() {
		var restInfo = new LevelEndSignal {
			isBot = this.shapeShooter.IsBot
		};

		SignalManager.Me.Fire<LevelEndSignal>(restInfo);
	}
}
