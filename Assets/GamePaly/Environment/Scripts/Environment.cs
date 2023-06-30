using System.Collections.Generic;

using UnityEngine;

public class Environment : MonoBehaviour {
#pragma warning disable IDE0044 // Make field readonly
	[SerializeField] private List<Track> _tracks = new();
#pragma warning restore IDE0044 // Make field readonly
	public long TrackCount => this._tracks.Count;

	public void Init()
			=> this._tracks = new();

	public void AddTrack(Track obj) {
		if (this._tracks.Contains(obj))
			return;

		this._tracks.Add(obj);
		obj.transform.parent = this.transform;
	}

	public void RemoveTrack(Track obj) {
		if (this._tracks.Contains(obj))
			_ = this._tracks.Remove(obj);
	}

	public Track GetTrackByIndex(int index) {
		return index < 0 || index >= this._tracks.Count
			? null
			: this._tracks[index];
	}
	private void Awake()
		=> this.Init();
}
