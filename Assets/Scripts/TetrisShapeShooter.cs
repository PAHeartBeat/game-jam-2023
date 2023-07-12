using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisShapeShooter : MonoBehaviour, IShapeShooter {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private CharacterController _characterController;
	[SerializeField] private BlockController[] _blocks;
	[SerializeField] private Transform _blocksParent;
	private List<BlockController> _generatedBlocks;

	public bool _isBot;

	[SerializeField] private float _shootInterval = 2f;             // Interval between each shape shoot
	[SerializeField] private float _shootForce = 10f;               // Force to apply to the shape when shooting
	[SerializeField] private float _bulletLifetime = 5f;            // Time in seconds before the bullet is destroyed

	[SerializeField] private bool _canUseGun = true;               // Flag to indicate if the player can use the gun
#pragma warning restore IDE0044 // Make field readonly

	private float _shootTimer = 0f;               // Timer for shooting shapes
	public int _currentShapeIndex = 0;           // Index of the currently selected shape
	[SerializeField] private int _botPercentageOfMakingRightDecision = 60;

	public static Action<int> onShapeChange;
	
	public void RemoveBlockFromCache(BlockController obj) {
		if (this._generatedBlocks.Contains(obj)) {
			_ = this._generatedBlocks.Remove(obj);
		}
	}

	public void ChangeShape() {

		if (_isBot) {
			var decisionIndex =UnityEngine.Random.Range(0, 100);
			if (decisionIndex < _botPercentageOfMakingRightDecision) {
				return;
			}
		}

		// Increase the current shape index
		this._currentShapeIndex++;

		// Wrap the index around if it exceeds the shape count
		if (this._currentShapeIndex >= this._blocks.Length) {
			this._currentShapeIndex = 0;
		}

		if (!_isBot) {
			onShapeChange?.Invoke(_currentShapeIndex);
			AudioManager.Instance.PlaySFX("bottle-clink-101000");
		}

		// Print the selected shape index
		Debug.Log("Selected Shape Index: " + this._currentShapeIndex);
	}

	public void GetRandomShape() {
		var randomIndex =UnityEngine.Random.Range(0, this._blocks.Length);
		if (randomIndex < 0 || randomIndex >= this._blocks.Length) {
			return;
		}
		this._currentShapeIndex = randomIndex;
		if (!_isBot)
			onShapeChange?.Invoke(_currentShapeIndex);
		// Print the selected shape index
		Debug.Log("Selected Shape Index: " + this._currentShapeIndex);
	}

	public void ShootShape() {
		if (this._currentShapeIndex < 0 || this._currentShapeIndex >= this._blocks.Length) {
			return;
		}
		var shape = this._blocks[this._currentShapeIndex];

		// Create a parent object for the shape
		var block = Instantiate(shape, this._blocksParent);
		this._generatedBlocks.Add(block);

		// Position and scale the shape at the cannon's position
		block.transform.position = this._characterController.BulletInitPoint.position;
		// Adjust the scale as needed
		block.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

		// Setup the block for it's behaviour
		block.Setup(this, this._bulletLifetime);

		// Apply a force to shoot the shape
		block.AddForce(this._characterController.BulletInitPoint.forward * this._shootForce, ForceMode.Impulse);
		AudioManager.Instance.PlaySFX("cannonball-89596 - Selected");

		// Apply recoil to the player
		// this._characterController.ApplyRecoil();

		// Disable Shooting while recoil Happens.
		this.DisableShooting();

		//this.GetRandomShape();
	}

	public void RotateShape() {
		if (_isBot) {
			var decisionIndex =UnityEngine.Random.Range(0, 100);
			if (decisionIndex < _botPercentageOfMakingRightDecision) {
				return;
			}
		}
		if (!_isBot) {
			AudioManager.Instance.PlaySFX("clock-close-mic-38567 (mp3cut.net)");
		}
		for (int i = 0; i < this._generatedBlocks.Count; i++) {
			this._generatedBlocks[i].RotateShape();
		}
	}

	#region  Mono Action
#pragma warning disable IDE0051 // private member is unused.
	void Start() {
		this._originalScale = this.cannonThread.transform.localScale;

		this._generatedBlocks = new();
		var dummy = GameObject.Find("ShapeParent");
		if (null != dummy) {
			this._blocksParent = dummy?.transform ?? null;
		}

		this.GetRandomShape();
	}

	public GameObject cannonThread; // Public reference to the cannon thread object

	private Vector3 _originalScale; // Store the original scale of the cannon thread

	private void Update() {
		if (!this._characterController.IsActive)
			return;

		if (this._generatedBlocks.Count == 0) {
			// Check for input to change the currently selected shape
			if (Input.GetButtonDown("Fire1") || this._isBot) {
				this.ChangeShape();
			}

			// Update the shoot timer
			this._shootTimer += Time.smoothDeltaTime;

			// Check if it's time to shoot a shape
			if (this._shootTimer >= this._shootInterval) {
				// Shoot a random Tetris shape if no hurdle is near
				if (this._canUseGun && !_characterController.IsHurdleNear()) {
					// Shoot the shape
					this.ShootShape();
					StartCoroutine(WaitForBulletToDestroyCoroutine());
				} else {
					Debug.LogError("Hurdle is near");
				}

				// Reset the shoot timer
				this._shootTimer = 0f;
			}
		}

		// Check if the bullet is in the air
		if (Input.GetButtonDown("Fire1") && this._generatedBlocks.Count > 0) {
			// Rotate the shape continuously
			this.RotateShape();
		}

		if (this._characterController.IsSmashing) {
			// Character is smashing, so we wait until it stops before restarting the shoot timer
			_ = this.StartCoroutine(this.WaitForSmashToEnd());
		}
	}

	private IEnumerator WaitForBulletToDestroyCoroutine() {
		yield return new WaitUntil(() => this._generatedBlocks.Count > 0);
		yield return new WaitUntil(() => this._generatedBlocks.Count == 0);
		this._shootTimer = 0f; // Reset the shoot timer after the bullet is destroyed
	}

	private IEnumerator WaitForSmashToEnd() {
		while (this._characterController.IsSmashing) {
			yield return null;
		}

		// Character has stopped smashing, so we restart the shoot timer
		this._shootTimer = 0f;
	}

#pragma warning restore IDE0051 // private member is unused.
	#endregion

	private void DisableShooting() {
		// Disable the gun usage temporarily
		this._canUseGun = false;

		// Start a coroutine to enable gun usage after a delay
		_ = this.StartCoroutine(this.EnableGunAfterDelay(this._shootInterval));
	}

	private IEnumerator EnableGunAfterDelay(float delay) {
		yield return new WaitForSeconds(delay);
		this._canUseGun = true;
	}
}
