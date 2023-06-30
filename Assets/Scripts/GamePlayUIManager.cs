using System;

using iPAHeartBeat.Core.Extensions;
using iPAHeartBeat.Core.SignalSystem;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIManager : MonoBehaviour {
	[SerializeField] private Image _nextShape;
	[SerializeField] private TextMeshProUGUI _multiplier;

	[SerializeField] private ShapeImageInfo[] _shapeImages;
	private int _currentMultiplier = 1;

	public int CurrentMultiplier {
		get => this._currentMultiplier;
		set {
			this._currentMultiplier = value;
			this.UpdateMultiplierInfoOnUI();
		}
	}

	private void OnEnable() {
		SignalManager.Me.SubscribeSignal<MultiplierIncreaseSignal>(this.MultiplierUpdate);
		SignalManager.Me.SubscribeSignal<MultiplierResetSignal>(this.MultiplierReset);
		SignalManager.Me.SubscribeSignal<ShapeChangeSignal>(this.NextShapeChanged);
	}
	private void OnDisable() {
		SignalManager.Me.UnsubscribeSignal<MultiplierIncreaseSignal>(this.MultiplierUpdate);
		SignalManager.Me.UnsubscribeSignal<MultiplierResetSignal>(this.MultiplierReset);
		SignalManager.Me.UnsubscribeSignal<ShapeChangeSignal>(this.NextShapeChanged);

	}

	private void NextShapeChanged(ShapeChangeSignal shapeInfo) {
		if (shapeInfo.isBot)
			return;

		var shape = shapeInfo.shape;
		var shapeImageInfo = Array.Find(this._shapeImages, o => o.shape == shape);
		if (shapeImageInfo.IsNull())
			return;
		this._nextShape.sprite = shapeImageInfo.shapeImage;
	}

	private void MultiplierUpdate(MultiplierIncreaseSignal info)
		=> this.CurrentMultiplier++;
	private void MultiplierReset(MultiplierResetSignal info)
		=> this.CurrentMultiplier = 0;

	private void UpdateMultiplierInfoOnUI() {
		this._multiplier.text = this.CurrentMultiplier > 0
			? $"{this.CurrentMultiplier}x"
			: string.Empty;
	}
}
