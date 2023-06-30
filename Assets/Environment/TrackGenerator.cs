using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackGenerator : EditorWindow {
#pragma warning disable IDE0044 // Make field readonly
	private GameObject _trackPrefab;
	private List<GameObject> _obstaclePrefabs = new List<GameObject>();
	private int _levelNumber;
	private GameObject _environmentPrefab;
	private float _maxLength;
	private int _numTracks;
	private float _startOffset;
	private float _bossOffset;
	private float _startCleanOffset;
	private float _endCleanOffset;
	private float _minObstacleDistance;
	private float _maxObstacleDistance;
	private int _obstaclesPerTrack;
#pragma warning restore IDE0044 // Make field readonly

	[MenuItem("Window/Track Generator")]
	private static void Init() {
		var window = GetWindow<TrackGenerator>();
		window.Show();
	}

	private void OnGUI() {
		_trackPrefab = EditorGUILayout.ObjectField("Track Prefab", _trackPrefab, typeof(GameObject), false) as GameObject;

		GUILayout.Label("Obstacle Prefabs", EditorStyles.boldLabel);
		for (int i = 0; i < _obstaclePrefabs.Count; i++) {
			_obstaclePrefabs[i] = EditorGUILayout.ObjectField($"Obstacle {i + 1}", _obstaclePrefabs[i], typeof(GameObject), false) as GameObject;
		}

		if (GUILayout.Button("Add Obstacle")) {
			_obstaclePrefabs.Add(null);
		}
		if (GUILayout.Button("Remove Last Obstacle") && _obstaclePrefabs.Count > 0) {
			_obstaclePrefabs.RemoveAt(_obstaclePrefabs.Count - 1);
		}

		_levelNumber = EditorGUILayout.IntField("Level Number", _levelNumber);

		_environmentPrefab = EditorGUILayout.ObjectField("Environment Prefab", _environmentPrefab, typeof(GameObject), false) as GameObject;

		_maxLength = EditorGUILayout.FloatField("Max Length", _maxLength);

		_numTracks = EditorGUILayout.IntField("Number of Tracks", _numTracks);

		_startOffset = EditorGUILayout.FloatField("Start Offset", _startOffset);
		_bossOffset = EditorGUILayout.FloatField("Boss Offset", _bossOffset);

		_startCleanOffset = EditorGUILayout.FloatField("Start Clean area", _startCleanOffset);
		_endCleanOffset = EditorGUILayout.FloatField("End Clean Area", _endCleanOffset);

		_minObstacleDistance = EditorGUILayout.FloatField("Min Obstacle Distance", _minObstacleDistance);
		_maxObstacleDistance = EditorGUILayout.FloatField("Max Obstacle Distance", _maxObstacleDistance);

		if (GUILayout.Button("Generate Tracks")) {
			GenerateTracks();
		}
		if (GUILayout.Button("Save Prefab")) {
			SavePrefab();
		}
	}

	GameObject _environment;
	private void GenerateTracks() {
		var _tracks = new List<GameObject>(); ;
		_environment = new GameObject($"Environment {_levelNumber}");
		for (int i = 0; i < _numTracks; i++) {
			var track = PrefabUtility.InstantiatePrefab(_trackPrefab) as GameObject;
			_tracks.Add(track);

			track.transform.parent = _environment.transform;

			var trackMesh = track.transform.GetChild(0);
			var meshPos = trackMesh.transform.localPosition;
			var meshScale = trackMesh.transform.localScale;
			meshScale.z = _maxLength;
			meshPos.z = meshScale.z / 2;
			trackMesh.transform.localScale = meshScale;
			trackMesh.transform.localPosition = meshPos;

			// Set the position and scale of the track
			var scale = track.transform.localScale;
			var pos = track.transform.position;
			pos.x = scale.x * meshScale.x * i;
			track.transform.position = pos;

			// Create the starting point for the player
			var startPoint = new GameObject("StartPoint");
			startPoint.transform.parent = track.transform;
			startPoint.transform.localPosition = new Vector3(0f, 0f, _startOffset);

			// Create the boss position near the end of the track
			var bossPosition = new GameObject("BossPosition");
			bossPosition.transform.parent = track.transform;
			bossPosition.transform.localPosition = new Vector3(0f, 0f, _maxLength - _bossOffset);
		}

		// Calculate the number of obstacles for the track
		var lastObstacles = new int[_tracks.Count];
		var obstaclePosition = new List<float>();
		var z = _startCleanOffset;
		var endZ = _maxLength - _endCleanOffset;

		while (z >= _startCleanOffset && z < endZ) {
			z += UnityEngine.Random.Range(_minObstacleDistance, _maxObstacleDistance);
			obstaclePosition.Add(z);
		}
		var count = obstaclePosition.Count;
		obstaclePosition.RemoveAt(count-1);

		var obstacleTypeIndex = -1;

		// Create obstacles
		for (var obstacleIdx = 0; obstacleIdx < obstaclePosition.Count; obstacleIdx++) {
			// Create obstacles on the track
			for (var trackIndex = 0; trackIndex < _tracks.Count; trackIndex++) {
				obstacleTypeIndex = lastObstacles[trackIndex];
				while (lastObstacles[trackIndex] == obstacleTypeIndex || obstacleTypeIndex == -1) {
					obstacleTypeIndex = UnityEngine.Random.Range(0, _obstaclePrefabs.Count);
				}

				lastObstacles[trackIndex] = obstacleTypeIndex;
				GameObject obstaclePrefab = _obstaclePrefabs[obstacleTypeIndex];

				var obstacle = PrefabUtility.InstantiatePrefab(obstaclePrefab) as GameObject;
				obstacle.name = $"T_{trackIndex}-O_{obstacleIdx}";
				obstacle.transform.parent = _tracks[trackIndex].transform;
				obstacle.transform.localPosition = new Vector3(0f, 0f, obstaclePosition[obstacleIdx]);
			}
		}
	}

	private void SavePrefab() {
		if (_environmentPrefab == null) {
			// Save the environment as a prefab
			string savePath = EditorUtility.SaveFilePanelInProject("Save Environment Prefab", "Environment", "prefab", "Save Environment Prefab");
			if (savePath.Length > 0) {
				PrefabUtility.SaveAsPrefabAsset(_environment, savePath);
			}
		}
	}
}
