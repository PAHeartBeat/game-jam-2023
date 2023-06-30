using System.Collections.Generic;

using UnityEngine;

public class Track : MonoBehaviour {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private List<IObstacles> _obstacles = new();
	[SerializeField] public Transform meshTransform;
	[SerializeField] public Transform startPoint;
	[SerializeField] public Transform bossPoint;
#pragma warning restore IDE0044 // Make field readonly

	public void Init()
		=> this._obstacles = new();

	public void AddObstacle(IObstacles obj) {
		if (this._obstacles.Contains(obj))
			return;

		this._obstacles.Add(obj);
	}

	public void RemoveObstacle(IObstacles obj) {
		if (!this._obstacles.Contains(obj))
			return;

		_ = this._obstacles.Remove(obj);
	}

	private void Awake()
		=> this.Init();

}
