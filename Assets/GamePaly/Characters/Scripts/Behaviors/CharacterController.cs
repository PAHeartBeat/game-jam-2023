using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iPAHeartBeat.Core.Extensions;
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
	[SerializeField] protected GameObject winningText,lossText,drawText;

	private int _totalHurdleCount = 7;
	public static int _botHurdleDestroyCount = 0;
	public static int _playerHurdleDestroyCount = 0;

	// Speed at which the player moves
#pragma warning restore IDE0044 // Make field readonly

	public bool IsActive => !this.thirdPersonCamera.isCinematicView;
	public bool IsSmashing { get; private set; }

	public Transform BulletInitPoint => this.cannonTransform;

	protected Transform trCache;
	private bool _isTriggerChecked = false;
	public static string winner;

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
		// this.transform.Translate(Vector3.forward * this.moveSpeed * Time.smoothDeltaTime, Space.Self);

		return true;
	}

	IEnumerator ChangeScene() {
		yield return new WaitForSeconds(3f);
		winner = null;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
	}

	[SerializeField] private float hurdleDetectionDistance = 5f; // Maximum distance to detect hurdles
	[SerializeField] private List<GameObject> objects; // Tag assigned to the hurdles


	public bool IsHurdleNear() {
		return objects.Any(obj => obj != null && Vector3.Distance(transform.position, obj.transform.position) <= hurdleDetectionDistance);
	}

	protected override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
		Debug.Log($"Trigger happened in {this.name} with {other?.name ?? ""}");

		if (other.name.Equals("EndPoint")) {
			IsSmashing = true;
			this.animator.SetTrigger(this.idleAnimation);
				if (string.IsNullOrEmpty(winner)) {
				{
					winner = "Winner";
				}
				if (!shapeShooter._isBot) {
					{
						winningText.gameObject.SetActive(!string.IsNullOrEmpty(winner));
						lossText.gameObject.SetActive(string.IsNullOrEmpty(winner));
						StartCoroutine(ChangeScene());
					}
				}
			}

		}

		if (this._isTriggerChecked) return;

		var obstacle = other.GetComponentInParent<IObstacles>();

		if (obstacle == null) {
			AudioManager.Instance.PlaySFX("negative_beeps-6008");
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
		=> this.animator.SetTrigger(this.runAnimationState);

	private IEnumerator StartRunOnceActive() {
		yield return new WaitUntil(() => this.IsActive);
		this.StartRun();
	}

	private void StartSmashing(IObstacles obstacle) {
		this.IsSmashing = true;
		this.animator.SetTrigger(this.smashAnimationState);

		_ = this.StartCoroutine(this.DestroyObstacle(obstacle));
		_ = this.StartCoroutine(this.PlaySmash());

	}
	private IEnumerator PlaySmash() {

		while (IsSmashing) {
			AudioManager.Instance.PlaySFX("punch-140236");
			yield return new WaitForSeconds(0.7f);
		}
	}

	private IEnumerator DestroyObstacle(IObstacles obstacle) {
		yield return new WaitForSeconds(obstacle.DestroyTime);
		obstacle.DestroyObstacle();
		AudioManager.Instance.PlaySFX("rock-destroy-6409");

		this._isTriggerChecked = false;
		this.IsSmashing = false;
		this.StartRun();
		
		this.transform.position = new Vector3(startPos.position.x, startPos.position.y, this.transform.position.z);
		this.transform.rotation = Quaternion.Euler(startPos.rotation.eulerAngles.x, startPos.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
	}
}
