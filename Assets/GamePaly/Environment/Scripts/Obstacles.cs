using iPAHeartBeat.Core.SignalSystem;
using UnityEngine;

public class Obstacles : MonoBehaviour, IShapeInfo, IObstacles {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private ShapeStyles _shape;
	[SerializeField] private int _angle;
	[SerializeField] private float _destroyTime;
	[SerializeField] private long _reward;
#pragma warning restore IDE0044 // Make field readonly

	public ShapeStyles Shape => this._shape;
	public float DestroyTime => this._destroyTime;

	public int Angle => this._angle;

	public void CheckShape(IBlockController shape) {
		if (this.Shape == shape.Shape && this.Angle == shape.Angle) {
			shape.KillOnTrigger();
			this.DestroyObstacle(shape.WasByBot);
		} else {
			shape.KillOnTrigger();
		}
	}

	public void DestroyObstacle(bool wasByBot) {
		this.FireScoreIncreaseSignal(wasByBot);
		this.FireIncreaseMultiplierSignal(wasByBot);
		Destroy(this.gameObject);
	}

	private void FireScoreIncreaseSignal(bool wasByBot) {
		if (wasByBot)
			return;

		var info = new BlockMatchSignal {
			shape = this.Shape,
			angle = this.Angle,
			reward = this._reward,
		};

		SignalManager.Me.Fire<BlockMatchSignal>(info);
	}

	private void FireIncreaseMultiplierSignal(bool wasByBot) {
		var restInfo = new MultiplierIncreaseSignal {
			isBot = wasByBot
		};

		SignalManager.Me.Fire<MultiplierIncreaseSignal>(restInfo);
	}

	// private void OnTriggerEnter(Collider other) {
	// 	Debug.Log($"Trigger happened in Obstacle {this.name} with {other?.name ?? ""}");
	// 	var x = other.GetComponent<IBlockController>();
	// 	if (x == null) {
	// 		return;
	// 	}

	// 	this.CheckShape(x);
	// }
}
