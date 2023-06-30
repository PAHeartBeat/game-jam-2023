using System.Collections.Generic;

using UnityEngine;

public class Environment : MonoBehaviour {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private List<Track> _tracks;
#pragma warning restore IDE0044 // Make field readonly

	public void AddTrack(Track obj) {
		if (this._tracks.Contains(obj))
			return;
		this._tracks.Add(obj);
	}

	public void RemoveTrack(Track obj) {
		if (!this._tracks.Contains(obj))
			return;
		_ = this._tracks.Remove(obj);
	}
}
