using UnityEngine;

public class TimestampMarker {
	public Rect rect;
	public float audioTime;
	public int sample;

	private GUIStyle _style;

	public TimestampMarker(float audioTime, int sample) {
		rect = new Rect(0, 0, 6, 30);
		this.audioTime = audioTime;
		this.sample = sample;

		_style = "Icon.Event";
	}

	public void Draw() {
		GUI.Box(rect, "", _style);
	}

	public void Drag(float delta, float clipLength, float markerAreaSize) {
		rect.x += delta;
		PositionToTime(clipLength, markerAreaSize);
	}

	public void UpdateRect(float clipLength, float markerAreaSize) {
		TimeToPosition(clipLength, markerAreaSize);
	}

	private void TimeToPosition(float clipLength, float markerAreaSize) {
		rect.x = audioTime / clipLength * markerAreaSize - 3;
	}

	private void PositionToTime(float clipLength, float markerAreaSize) {
		audioTime = (rect.x + 3) / markerAreaSize * clipLength;
	}
}