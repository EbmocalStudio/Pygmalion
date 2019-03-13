using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TimestampsEditor : EditorWindow {
	public static void ShowWindow() {
		EditorWindow window = GetWindow<TimestampsEditor>();
		window.titleContent = new GUIContent("Timestamps");
		window.Show();
	}

	public AudioClip audioClip;

	private List<TimestampMarker> _markers = new List<TimestampMarker>();
	private TimestampMarker _currentMarker;
	private TimestampTools _toolBox;
	private Rect _audioEditArea = new Rect(10, 50, 1000, 275);
	private Rect _scrollRect = new Rect(0, 30, 1000, 245);
	private Rect _scrollRectInner = new Rect(0, 0, 1000, 230);
	private Rect _toolsBox = new Rect(0, 0, 1000, 30);
	private Rect _markerBox = new Rect(0, 0, 1000, 30);
	private Rect _waveformRect = new Rect(0, 30, 1000, 200);
	private Rect _fableEditArea = new Rect(10, 0, 1000, 250);
	private Vector2 _scrollPos;
	private Texture2D _waveform;
	private bool _isPlaying;
	private bool _isLooping;
	private int _currentSample;
	private string _fableTitle = "";
	private string _fableText = "";
	private List<string> _splitFableText;
	private bool _fableNeedUpdate = false;

	//Amount of times the GUI is repainted per second when the audioClip is playing ( needed to visually update the audioclip's position on the waveform ).
	private int _timnelineRepaintFrequency = 4;

	//Time in seconds since the last GUI repaint ( only repaints called in the Update function ).
	private float _timeSinceRepaint = 0;

	private void OnEnable() {
		_toolBox = new TimestampTools(_audioEditArea, ToggleClip, PlayClip, PauseClip, ResumeClip, LoopClip);
	}

	private void Update() {
		if(_isPlaying) {
			if(AudioUtility.IsClipPlaying(audioClip)) {
				if(_timeSinceRepaint > 1 / _timnelineRepaintFrequency) {
					_currentSample = AudioUtility.GetClipSamplePosition(audioClip);
					Repaint();
					_timeSinceRepaint = 0;
				}
				else
					_timeSinceRepaint += Time.deltaTime;
			}
			else
				StopClip();
		}
	}

	private void OnGUI() {
		GUILayout.Space(10f);

		AudioClip newClip = AudioClipField("AudioClip : ", audioClip);
		if(audioClip != newClip) {
			audioClip = newClip;
			StopClip();
			
			PaintWaveform();
		}

		if(audioClip != null) {
			FitToWindow();

			GUILayout.BeginArea(_audioEditArea, GUI.skin.textArea);

			//Draw the toolbox above the waveform and markers ( contains play/pause, loop buttons and waveform size slider ).
			_toolBox.Draw(audioClip.length);

			DrawAudioArea();

			GUILayout.EndArea();
		}

		DrawFableArea();

		ProcessEvents(Event.current);

		if(GUI.changed)
			Repaint();
	}

	private void ProcessEvents(Event e) {
		if(audioClip != null) {
			switch(e.type) {
				case EventType.MouseDown:
					if(e.button == 0) {
						UpdateWaveformTimeline(e.mousePosition);

						if(IsCursorOnMarker(e.mousePosition)) {
							foreach(TimestampMarker marker in _markers) {
								if(GetChildRect(marker.rect, _audioEditArea, _scrollRect).Contains(e.mousePosition)) {
									_currentMarker = marker;
									Repaint();
									break;
								}
							}
						}
					}
					else if(e.button == 1)
						ProcessContextMenu(e.mousePosition);
					break;
				case EventType.MouseDrag:
					if(e.button == 0) {
						UpdateWaveformTimeline(e.mousePosition);

						if(IsCursorOnMarker(e.mousePosition)) {
							bool shouldRepaint = false;

							foreach(TimestampMarker marker in _markers) {
								if(GetChildRect(marker.rect, _audioEditArea, _scrollRect).Contains(e.mousePosition)) {
									marker.Drag(e.delta.x, audioClip.length, _toolBox.sliderValue);
									shouldRepaint = true;
								}
							}

							if(shouldRepaint)
								Repaint();
						}
					}
					break;
			}
		}
	}

	private void ProcessContextMenu(Vector2 mousePosition) {
		Rect child = GetChildRect(_markerBox, _audioEditArea, _scrollRect);
		if(child.Contains(mousePosition)) {
			GenericMenu genericMenu = new GenericMenu();
			
			if(_splitFableText != null && _splitFableText.Count > 0 == true && !_fableNeedUpdate && _markers.Count < _splitFableText.Count) {
				genericMenu.AddItem(new GUIContent("Add Marker"), false, AddMarker, mousePosition);
				genericMenu.AddItem(new GUIContent("Remove Marker"), false, RemoveMarker);
			}
			else {
				genericMenu.AddItem(new GUIContent("Add Marker"), false, null);
				genericMenu.AddItem(new GUIContent("Remove Marker"), false, null);
			}

			genericMenu.ShowAsContext();
		}
	}

	private bool IsCursorOnMarker(Vector2 mousePosition) {
		if(GetChildRect(_markerBox, _audioEditArea, _scrollRect).Contains(mousePosition))
			return true;

		return false;
	}

	private void UpdateWaveformTimeline(Vector2 mousePosition) {
		if(audioClip != null) {
			Rect child = GetChildRect(_waveformRect, _audioEditArea, _scrollRect);
			if(child.Contains(mousePosition)) {
				if(mousePosition.x - child.x > _toolBox.sliderValue)
					_currentSample = audioClip.samples;
				else {
					_currentSample = Mathf.CeilToInt((mousePosition.x - child.x) / _toolBox.sliderValue * audioClip.samples);

					if(!AudioUtility.IsClipPlaying(audioClip)) {
						PlayClip(_currentSample);
						PauseClip();
					}

					AudioUtility.SetClipSamplePosition(audioClip, _currentSample);
				}

				GUI.changed = true;
			}
		}
	}

	private void DrawAudioArea() {
		ResizeEditRects();

		_scrollPos = GUI.BeginScrollView(_scrollRect, _scrollPos, _scrollRectInner, true, false);

		GUI.Box(_markerBox, GUIContent.none, GUI.skin.textField);

		GUI.DrawTexture(_waveformRect, _waveform, ScaleMode.StretchToFill);

		DrawMarkers();

		Handles.color = Color.red;
		Handles.DrawLine(
			new Vector2((float)_currentSample / audioClip.samples * _toolBox.sliderValue, _waveformRect.y),
			new Vector2((float)_currentSample / audioClip.samples * _toolBox.sliderValue, _waveformRect.y + _waveformRect.size.y)
		);

		GUI.EndScrollView();
	}

	private void DrawFableArea() {
		_fableEditArea.y = audioClip != null ? _audioEditArea.height + _audioEditArea.y + 10 : _audioEditArea.y;
		_fableEditArea.width = Screen.width - 20;

		GUILayout.BeginArea(_fableEditArea);
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical(GUILayout.Width(300));

		_fableTitle = TextField("Fable Title : ", _fableTitle, 300, 20);

		string newText = TextArea("", _fableText, 300, 150);
		_fableNeedUpdate = !(newText == _fableText) || _fableNeedUpdate;
		_fableText = newText;

		GUILayout.BeginHorizontal();

		SplitString();

		SaveFable();

		LoadFable();

		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		GUILayout.Space(30f);

		GUILayout.BeginVertical(GUI.skin.textArea, GUILayout.Width(200), GUILayout.Height(170));

		GUILayout.Label("Marker Settings : ");

		if(_currentMarker != null) {
			GUILayout.Label("Max Sample : ");
			using(new EditorGUI.DisabledScope())
				TextField("Word : ", _splitFableText[_markers.IndexOf(_currentMarker)]);
			_currentMarker.audioTime = float.Parse(TextField("Audio Time : ", _currentMarker.audioTime.ToString()));
			_currentMarker.sample = int.Parse(TextField("Audio Sample : ", _currentMarker.sample.ToString()));
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void SplitString() {
		//Disables the button if the fable split string is up to date with the fable text.
		using(new EditorGUI.DisabledScope(!_fableNeedUpdate)) {
			if(GUILayout.Button(new GUIContent("Split String"), GUILayout.Width(98), GUILayout.Height(30))) {
				_splitFableText = new List<string>();

				int wordIndex = 0;

				string escapedText = _fableText + '\0';

				for(int i = 0; i < escapedText.Length; i++) {
					if(escapedText[i] == ' ' || escapedText[i] == ',' || escapedText[i] == '.' || escapedText[i] == '\0' || escapedText[i] == '!' || escapedText[i] == ':' || escapedText[i] == '\n') {
						int length = i - wordIndex;

						if(length > 0) {
							//If a word separating char is found, add the completed word to the list.
							_splitFableText.Add(escapedText.Substring(wordIndex, length));
						}

						//Change the starting index of the new word to the next index.
						wordIndex = i + 1;
					}
				}

				_fableNeedUpdate = false;
			}
		}
	}

	private void SaveFable() {
		using(new EditorGUI.DisabledScope(_fableNeedUpdate || _fableTitle == "" || _fableText == "" || audioClip == null)) {
			if(GUILayout.Button(new GUIContent("Save Fable"), GUILayout.Width(98), GUILayout.Height(30))) {
				FableAsset fable = new FableAsset(_fableTitle, _fableText, audioClip);

				List<FableAsset.NarrationMarker> narMarkers = new List<FableAsset.NarrationMarker>();

				foreach(TimestampMarker marker in _markers)
					narMarkers.Add(new FableAsset.NarrationMarker(marker.audioTime, marker.sample));

				fable.markers = narMarkers;

				if(FableAssetCreator.CreateAsset(fable))
					Debug.Log("Asset creation succeeded");
				else
					Debug.Log("Asset creation failed");
			}
		}
	}

	private void LoadFable() {
		if(GUILayout.Button(new GUIContent("Load Fable"), GUILayout.Width(98), GUILayout.Height(30))) {
			string fablePath = EditorUtility.OpenFilePanel("Load Fable", "Assets/Prefabs/Narration", "");
			if(fablePath != "") {
				fablePath = fablePath.Replace(Application.dataPath, "Assets");
				FableAsset loadedFable = FableAssetCreator.LoadAsset(fablePath);

				if(loadedFable != null) {
					_fableTitle = loadedFable.fableTitle;
					_fableText = loadedFable.fableText;
					audioClip = loadedFable.audioClip;

					_markers = new List<TimestampMarker>();
					if(loadedFable.markers != null) {
						foreach(FableAsset.NarrationMarker marker in loadedFable.markers)
							_markers.Add(new TimestampMarker(marker.audioTime, marker.sample));
					}
					else
						Debug.Log("Loaded markers are empty.");

					ResetSettings();
					PaintWaveform();

					Repaint();
				}
				else
					Debug.Log("Wadu hek :(");
			}
		}
	}

	private void PaintWaveform() {
		_toolBox.sliderValue = audioClip.length * 100;
		_waveform = AudioClipVisualizer.PaintWaveformSpectrum(audioClip, 1, (int)_waveformRect.height, Color.yellow);
	}

	private void ResetSettings() {
		_currentMarker = null;
		_isPlaying = false;
		_isLooping = false;
		_currentSample = 0;
		_fableNeedUpdate = false;
	}

	#region RectOperations
	private void FitToWindow() {
		_audioEditArea.width = Screen.width - 20;
		_toolBox.rect.width = _audioEditArea.width;
		_scrollRect.width = _audioEditArea.width;
	}

	private void ResizeEditRects() {
		_scrollRectInner.width = _toolBox.sliderValue;
		_markerBox.width = _toolBox.sliderValue;
		_waveformRect.width = _toolBox.sliderValue;
		UpdateMarkerPosition();
	}

	private Rect GetChildRect(Rect child, params Rect[] parent) {
		foreach(Rect rect in parent) {
			child.x += rect.x;
			child.y += rect.y;
		}

		return child;
	}

	private Rect GetChildRect(Rect child, Rect parent) {
		child.x += parent.x;
		child.y += parent.y;

		return child;
	}
	#endregion

	#region Markers
	private void AddMarker(object mousePosition) {
		Vector2 pos = (Vector2)mousePosition;
		float audioTimeFraction = (pos.x - GetChildRect(_audioEditArea, _markerBox).x) / _toolBox.sliderValue;
		float audioTime = audioTimeFraction * audioClip.length;
		int sampleTime = Mathf.CeilToInt(audioTimeFraction * (float)audioClip.samples);

		_markers.Add(new TimestampMarker(audioTime, sampleTime));
		_currentMarker = _markers.Last();

		Repaint();
	}

	private void RemoveMarker() {

	}

	private void UpdateMarkerPosition() {
		foreach(TimestampMarker marker in _markers)
			marker.UpdateRect(audioClip.length, _toolBox.sliderValue);
	}

	private void DrawMarkers() {
		foreach(TimestampMarker marker in _markers) {
			marker.Draw();

			Vector2 firstPoint = new Vector2(marker.rect.position.x + marker.rect.width / 2, marker.rect.height);
			Vector2 secondPoint = new Vector2(marker.rect.position.x + marker.rect.width / 2, marker.rect.height + _waveformRect.height);

			Handles.color = Color.white;
			Handles.DrawLine(
				firstPoint,
				secondPoint
			);
		}
	}
	#endregion

	#region AudioUtility
	private bool ToggleClip() {
		_isPlaying = !_isPlaying;
		if(_isPlaying) {
			if(_currentSample == 0)
				AudioUtility.PlayClip(audioClip, 0, _isLooping);
			else
				ResumeClip();
		}
		else {
			_currentSample = AudioUtility.GetClipSamplePosition(audioClip);
			AudioUtility.PauseClip(audioClip);
		}

		return _isPlaying;
	}

	private void PlayClip(int? samplePosition = null) {
		if(samplePosition == null)
			AudioUtility.PlayClip(audioClip, 0, _isLooping);
		else
			AudioUtility.PlayClip(audioClip, samplePosition.Value, _isLooping);
	}

	private void PauseClip() {
		AudioUtility.PauseClip(audioClip);
	}

	private void ResumeClip() {
		AudioUtility.ResumeClip(audioClip);
	}

	private bool LoopClip() {
		_isLooping = !_isLooping;
		AudioUtility.LoopClip(audioClip, _isLooping);

		return _isLooping;
	}

	private void StopClip() {
		_currentSample = 0;
		_isLooping = false;
		_isPlaying = false;
		_toolBox.ResetSettings();
		AudioUtility.StopClip(audioClip);

		Repaint();
	}
	#endregion

	#region Property Drawers
	private static AudioClip AudioClipField(string label, AudioClip output) {
		GUILayout.Space(10f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(10f);

		float maxWidth = label.Length * 6;

		GUILayout.Label(label, GUILayout.MaxWidth(maxWidth));

		output = EditorGUILayout.ObjectField(output, typeof(AudioClip), false, GUILayout.Width(240)) as AudioClip;

		GUILayout.EndHorizontal();

		return output;
	}

	private static string TextArea(string label, string output, float width, float height) {
		GUILayout.BeginVertical(GUILayout.Width(width));

		if(label != "")
			GUILayout.Label(label, GUILayout.Height(20));

		output = EditorGUILayout.TextArea(output, GUILayout.Width(width), GUILayout.Height(height));

		GUILayout.EndVertical();

		return output;
	}

	private static string TextField(string label, string output, float width = 0, float height = 0) {
		if(width == 0 || height == 0)
			output = EditorGUILayout.TextField(label, output);
		else
			output = EditorGUILayout.TextField(label, output, GUILayout.Width(width), GUILayout.Height(height));

		return output;
	}
	#endregion
}