using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TextUtil;

public class NarrationManager : MonoBehaviour {
	//ToDo :
	// - Find a way to link text objects with audio snippets and implement timestamps to highlight the words in sync with the narration using TextManager class.
	// - Write a tool that helps making timestamps.
	// -> Play audio clip at slow playback speed.
	// -> Add a button that when pressed, writes the current audio source time.
	// -> All you have to do is listen to the audio clip and press the button each time the narrator says a word, in the end you end up with a text file with
	// ->> all the indices and their timestamps.

	#region Variables
	public float queueClipDelay = 0.25f;

	private AudioSource _audioSource;
	private NarrationElement _currentElement;
	private Queue<NarrationElement> _narrElements = new Queue<NarrationElement>();
	private float _timeSinceLastClip = 0;
	private NarrationUIManager _ui;
	#endregion

	#region Unity functions
	private void Awake() {
		_audioSource = GetComponent<AudioSource>();
		_ui = GetComponent<NarrationUIManager>();
	}

	private void Update() {
		if(_audioSource.isPlaying) {
			DynamicTextData.WordData word = _currentElement.UpdateMarker(_audioSource.time);
			if(word != null)
				_ui.HighlightWord(word, Color.red);
		}
		else if(_audioSource.time == 0) {
			if(_timeSinceLastClip < queueClipDelay)
				_timeSinceLastClip += Time.deltaTime;
			else {
				DequeueElement();

				_timeSinceLastClip = 0;
			}
		}
	}
	#endregion

	#region Functions
	public void QueueElement(NarrationElement element) {
		_narrElements.Enqueue(element);

		if(_currentElement == null)
			DequeueElement();
	}

	private void DequeueElement() {
		if(_narrElements.Count > 0) {
			_currentElement = _narrElements.Dequeue();
			_currentElement.text.enabled = true;
			_currentElement.LoadData(_ui.transform.localScale.x);
			_ui.LoadData(_currentElement.TextData);
			_audioSource.clip = _currentElement.audioClip;
			_audioSource.Play();
		}
		else {
			_currentElement = null;
			_ui.SetEmpty();
		}
	}
	#endregion
}