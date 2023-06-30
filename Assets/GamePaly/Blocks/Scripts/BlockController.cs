
using UnityEngine;

public class BlockController : MonoBehaviour, IBlockController {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private ObstacleShapes _shape;
	[SerializeField] private int _angle;
	// [SerializeField] private TetrisShapeRotation _rotation;
#pragma warning restore IDE0044 // Make field readonly

	public ObstacleShapes Shape => this._shape;
	public int Angle => this._angle;

	private IShapeShooter _controller;

	public void Setup(IShapeShooter controller, float lifeTime) {
		this._controller = controller;
		this.gameObject.SetActive(true);
		Destroy(this, lifeTime);
	}

	public void AddForce(Vector3 force, ForceMode mode)
		=> this._rigidbody.AddForce(force, mode);

	public void KillOnTrigger()
		=> Destroy(this);

	public void RotateShape() {
		this.transform.Rotate(Vector3.back, 90f, Space.Self);
		this._angle += 90;
		if (this._angle >= 360)
			this._angle %= 360;
	}

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
	bool _isTriggerChecked = false;
	private void OnTriggerEnter(Collider other) {
		if (this._isTriggerChecked) return;

		var obstacle = other.GetComponentInParent<IObstacles>();
		if (obstacle == null) {
			return;
		}

		this._isTriggerChecked = true;
		obstacle.CheckShape(this);
	}
#pragma warning restore IDE0051 // private member is unused.
}
