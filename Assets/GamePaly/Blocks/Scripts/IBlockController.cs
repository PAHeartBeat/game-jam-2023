
using UnityEngine;

public interface IBlockController : IShapeInfo {
	void Setup(IShapeShooter controller, float lifeTime);
	void AddForce(Vector3 force, ForceMode mode);
	void KillOnTrigger();
}
