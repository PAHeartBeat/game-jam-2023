using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackGenerator : EditorWindow {
#pragma warning disable IDE0044 // Make field readonly
	private Track _trackPrefab;
	private List<Obstacles> _obstaclePrefabs = new List<Obstacles>();
	private int _levelNumber;
	private Environment _environmentPrefab;
	private float _maxLength;
	private int _numTracks;
	private float _startOffset;
	private float _bossOffset;
	private float _startCleanOffset;
	private float _endCleanOffset;
	private float _minObstacleDistance;
	private float _maxObstacleDistance;
#pragma warning restore IDE0044 // Make field readonly

	[MenuItem("Window/Track Generator")]
	private static void Init() {
		var window = GetWindow<TrackGenerator>();
		window.Show();
	}

	private void OnGUI() {
		this._trackPrefab = EditorGUILayout.ObjectField("Track Prefab", this._trackPrefab, typeof(Track), false) as Track;

		GUILayout.Label("Obstacle Prefabs", EditorStyles.boldLabel);
		for (int i = 0; i < this._obstaclePrefabs.Count; i++) {
			this._obstaclePrefabs[i] = EditorGUILayout.ObjectField($"Obstacle {i + 1}", this._obstaclePrefabs[i], typeof(Obstacles), false) as Obstacles;
		}

		if (GUILayout.Button("Add Obstacle")) {
			this._obstaclePrefabs.Add(null);
		}
		if (GUILayout.Button("Remove Last Obstacle") && this._obstaclePrefabs.Count > 0) {
			this._obstaclePrefabs.RemoveAt(this._obstaclePrefabs.Count - 1);
		}

		this._levelNumber = EditorGUILayout.IntField("Level Number", this._levelNumber);

		this._environmentPrefab = EditorGUILayout.ObjectField("Environment Prefab", this._environmentPrefab, typeof(Environment), false) as Environment;

		this._maxLength = EditorGUILayout.FloatField("Max Length", this._maxLength);

		this._numTracks = EditorGUILayout.IntField("Number of Tracks", this._numTracks);

		this._startOffset = EditorGUILayout.FloatField("Start Offset", this._startOffset);
		this._bossOffset = EditorGUILayout.FloatField("Boss Offset", this._bossOffset);

		this._startCleanOffset = EditorGUILayout.FloatField("Start Clean area", this._startCleanOffset);
		this._endCleanOffset = EditorGUILayout.FloatField("End Clean Area", this._endCleanOffset);

		this._minObstacleDistance = EditorGUILayout.FloatField("Min Obstacle Distance", this._minObstacleDistance);
		this._maxObstacleDistance = EditorGUILayout.FloatField("Max Obstacle Distance", this._maxObstacleDistance);

		if (GUILayout.Button("Generate")) {
			this.GenerateTracks();
		}
		if (GUILayout.Button("Save Prefab")) {
			this.SavePrefab();
		}
		if (GUILayout.Button("Generate & Save Prefab")) {
			this.GenerateTracks();
			this.SavePrefab();
		}

	}

	GameObject _envGo;
	private void GenerateTracks() {
		Environment _environment;
		this._envGo = new GameObject($"Environment {this._levelNumber}");
		_environment = this._envGo.AddComponent<Environment>();
		_environment.Init();
		Track track;
		for (var i = 0; i < this._numTracks; i++) {
			track = PrefabUtility.InstantiatePrefab(this._trackPrefab) as Track;
			track.Init();
			_environment.AddTrack(track);

			var trackMesh = track.meshTransform;
			var meshPos = trackMesh.localPosition;
			var meshScale = trackMesh.localScale;
			meshScale.z = this._maxLength;
			meshPos.z = meshScale.z / 2;
			trackMesh.localScale = meshScale;
			trackMesh.localPosition = meshPos;

			// Set the position and scale of the track
			var scale = track.transform.localScale;
			var pos = track.transform.position;
			pos.x = scale.x * meshScale.x * i;
			track.transform.position = pos;

			// Create the starting point for the player
			var startPoint = new GameObject("StartPoint");
			track.startPoint = startPoint.transform;
			startPoint.transform.parent = track.transform;
			startPoint.transform.localPosition = new Vector3(0f, 0f, this._startOffset);

			// Create the boss position near the end of the track
			var bossPoint = new GameObject("BossPoint");
			track.bossPoint = bossPoint.transform;
			bossPoint.transform.parent = track.transform;
			bossPoint.transform.localPosition = new Vector3(0f, 0f, this._maxLength - this._bossOffset);
		}

		// Calculate the number of obstacles for the track
		var lastObstacles = new int[_environment.TrackCount];
		var obstaclePosition = new List<float>();
		var z = this._startCleanOffset;
		var endZ = this._maxLength - this._endCleanOffset;

		while (z >= this._startCleanOffset && z < endZ) {
			z += UnityEngine.Random.Range(this._minObstacleDistance, this._maxObstacleDistance);
			obstaclePosition.Add(z);
		}

		var count = obstaclePosition.Count;
		obstaclePosition.RemoveAt(count - 1);

		int obstacleTypeIndex;

		Obstacles obstacle, obstaclePrefab;
		// Create obstacles
		for (var obstacleIdx = 0; obstacleIdx < obstaclePosition.Count; obstacleIdx++) {
			// Create obstacles on the track
			for (var trackIndex = 0; trackIndex < _environment.TrackCount; trackIndex++) {
				track = _environment.GetTrackByIndex(trackIndex);
				obstacleTypeIndex = lastObstacles[trackIndex];

				while (lastObstacles[trackIndex] == obstacleTypeIndex || obstacleTypeIndex == -1) {
					obstacleTypeIndex = UnityEngine.Random.Range(0, this._obstaclePrefabs.Count);
				}

				lastObstacles[trackIndex] = obstacleTypeIndex;
				obstaclePrefab = this._obstaclePrefabs[obstacleTypeIndex];

				obstacle = PrefabUtility.InstantiatePrefab(obstaclePrefab) as Obstacles;
				track.AddObstacle(obstacle);

				obstacle.name = $"T_{trackIndex}-O_{obstacleIdx}";
				obstacle.transform.parent = track?.transform ?? null;
				obstacle.transform.localPosition = new Vector3(0f, 0f, obstaclePosition[obstacleIdx]);
			}
		}
	}

	private void SavePrefab() {
		// Save the environment as a prefab
		var savePath = EditorUtility.SaveFilePanelInProject("Save Environment Prefab", $"Level {this._levelNumber}", "prefab", "Save Environment Prefab");
		if (savePath.Length > 0) {
			_ = PrefabUtility.SaveAsPrefabAsset(this._envGo, savePath);
			if (this._envGo != null) {
				GameObject.Destroy(this._envGo);
			}
		}
	}
}
