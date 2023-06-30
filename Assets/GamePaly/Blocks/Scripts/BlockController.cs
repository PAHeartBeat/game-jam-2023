
using UnityEngine;

public class BlockController : MonoBehaviour {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private Rigidbody _rigidbody;
	// [SerializeField] private TetrisShapeRotation _rotation;
#pragma warning restore IDE0044 // Make field readonly
	private IShapeShooter _controller;

	public void Setup(IShapeShooter controller, float lifeTime) {
		this._controller = controller;
		this.gameObject.SetActive(true);
		Destroy(this, lifeTime);
	}

	public void AddForce(Vector3 force, ForceMode mode)
		=> this._rigidbody.AddForce(force, mode);

#pragma warning disable IDE0051 // private member is unused.
	// Start is called before the first frame update
	private void Start() { }

	// Update is called once per frame
	private void Update() { }

	private void OnEnable() { }

	private void OnDestroy() {
		this._controller?.RemoveBlockFromCache(this);
		Destroy(this.gameObject);
	}
#pragma warning restore IDE0051 // private member is unused.
}
