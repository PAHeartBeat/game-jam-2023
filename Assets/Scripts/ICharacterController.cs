using UnityEngine;

public interface ICharacterController {
	bool IsActive { get; }
	Transform BulletInitPoint { get; }
	void ApplyRecoil();
}
