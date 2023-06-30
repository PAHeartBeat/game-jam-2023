using iPAHeartBeat.Core.SignalSystem;
using UnityEngine;

public class Obstacles : MonoBehaviour, IShapeInfo, IObstacles {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private ObstacleShapes _shape;
	[SerializeField] private int _angle;
	[SerializeField] private long _reward;
#pragma warning restore IDE0044 // Make field readonly

	public ObstacleShapes Shape => this._shape;
	public int Angle => this._angle;

	public void CheckShape(IBlockController shape) {
		if (this.Shape == shape.Shape && this.Angle == shape.Angle) {
			var info = new BlockMatchSignal {
				shape = this.Shape,
				angle = this.Angle,
				reward = this._reward,
			};

			SignalManager.Me.Fire<BlockMatchSignal>(info);
			shape.KillOnTrigger();
			Destroy(this);
		} else {
			shape.KillOnTrigger();
		}
	}
}

public class BlockMatchSignal {
	public float reward;
	public ObstacleShapes shape;
	public int angle;
}

public enum ObstacleShapes {
	T,
	O,
	J,
	L,
	I,
	Z,
	ZR,
}
