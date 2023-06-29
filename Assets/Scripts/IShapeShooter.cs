using UnityEngine;

public interface IShapeShooter {
	void RemoveBlockFromCache(BlockController obj);
	void SelectNextShape();
	void ShootShape();
}

public interface IPlayerController : ICharacterController { }
public interface IBotController : ICharacterController { }

public interface ICharacterController {
	bool IsActive { get; }
	Transform BulletInitPoint { get; }
	void ApplyRecoil();
}
