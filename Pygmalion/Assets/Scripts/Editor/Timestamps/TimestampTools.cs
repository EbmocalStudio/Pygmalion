using UnityEngine;
using UnityEditor;
using System;

public class TimestampTools {
	public Rect rect = new Rect(0, 0, 1000, 30);
	public float sliderValue = 1000;

	private Rect _parent;
	private GUIStyle _buttonStyle;
	private readonly Func<bool> ToggleClip;
	private readonly Action<int?> PlayClip;
	private readonly Action PauseClip;
	private readonly Action ResumeClip;
	private readonly Func<bool> LoopClip;
	private bool _isPlaying = false;
	private bool _isLooping = false;

	public TimestampTools(Rect parent, Func<bool> ToggleClip, Action<int?> PlayClip, Action PauseClip, Action ResumeClip, Func<bool> LoopClip) {
		_parent = parent;

		_buttonStyle = new GUIStyle();
		_buttonStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn right.png") as Texture2D;
		_buttonStyle.active.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn right on.png") as Texture2D;
		_buttonStyle.alignment = TextAnchor.MiddleCenter;
		_buttonStyle.border = new RectOffset(4, 4, 4, 4);

		this.ToggleClip = ToggleClip;
		this.PlayClip = PlayClip;
		this.PauseClip = PauseClip;
		this.ResumeClip = ResumeClip;
		this.LoopClip = LoopClip;
	}

	public void Draw(float audioLength) {
		GUI.Box(rect, GUIContent.none, GUI.skin.textField);
		GUILayout.BeginHorizontal();

		if(GUILayout.Button(EditorGUIUtility.FindTexture(_isPlaying ? "PauseButton On" : "PlayButton"), _buttonStyle, GUILayout.Width(30), GUILayout.Height(30))) {
			_isPlaying = ToggleClip();
			GUI.changed = true;
		}

		if(GUILayout.Button(EditorGUIUtility.FindTexture(_isLooping ? "preAudioLoopOn" : "preAudioLoopOff"), _buttonStyle, GUILayout.Width(30), GUILayout.Height(30))) {
			_isLooping = LoopClip();
			GUI.changed = true;
		}

		GUILayout.EndHorizontal();

		sliderValue = GUI.HorizontalSlider(new Rect(new Vector2(70, 7), new Vector2(100, 16)), sliderValue, audioLength * 40, audioLength * 100);
	}

	public void ResetSettings() {
		_isPlaying = false;
		_isLooping = false;

		GUI.changed = true;
	}
}