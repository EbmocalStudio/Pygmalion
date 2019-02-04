using UnityEngine;
using UnityEngine.UI;
using TextUtil;

public class NarrationManager : MonoBehaviour {
	//ToDo :
	// - Find a way to link text objects with audio snippets and implement timestamps to highlight the words in sync with the narration using TextManager class.
	// - Write a tool that helps making timestamps.
	// -> Play audio clip at slow playback speed.
	// -> Add a button that when pressed, writes the current audio source time.
	// -> All you have to do is listen to the audio clip and press the button each time the narrator says a word, in the end you end up with a text file with
	// ->> all the indices and their timestamps.

	[System.Serializable]
	public class NarrationTextData {
		public Text text;
		public AudioClip audio;
		public float[] timestamps;
		public DynamicTextData TextData { get; private set; }

		private int _currentTimestamp = -1;

		public void LoadData(float scale) {
			TextGenerationSettings settings = text.GetGenerationSettings(text.rectTransform.rect.size);
			TextGenerator textGenerator = new TextGenerator();
			textGenerator.Populate(text.text, settings);
			//Find another way to implement scale.

			TextData = new DynamicTextData();
			TextData.Populate(text.rectTransform, text.text, textGenerator.characters, text.fontSize, scale);
		}

		public DynamicTextData.WordData UpdateWord(float audioTime) {
			if(_currentTimestamp + 1 < timestamps.Length && timestamps[_currentTimestamp + 1] < audioTime) {
				_currentTimestamp++;
				return TextData.words[_currentTimestamp];
			}

			return null;
		}
	}

	public Text audioTime;
	public NarrationTextData[] texts;

	private bool _isPlaying = true;
	private int _currentText = 0;
	private AudioSource _audioSource;
	private NarrationUIManager _UIManager;

	private void Awake() {
		_audioSource = GetComponent<AudioSource>();
		_UIManager = GetComponent<NarrationUIManager>();

		if(texts.Length > 0) {
			texts[_currentText].LoadData(_UIManager.transform.localScale.x);
			_UIManager.textData = texts[_currentText].TextData;
			//Debug.Log(texts[_currentText].TextData.words[0].text + " , " + _UIManager.textData.words[0].text);
			//_UIManager.textData.words[0].text = "oof";
			//Debug.Log(texts[_currentText].TextData.words[0].text + " , " + _UIManager.textData.words[0].text);

			_audioSource.clip = texts[_currentText].audio;
			_audioSource.Play();
		}
	}

	private void Update() {
		if(texts.Length > 0 && _isPlaying) {
			DynamicTextData.WordData word = texts[_currentText].UpdateWord(_audioSource.time);
			if(word != null)
				_UIManager.HighlightWord(word, Color.red);
		}

		audioTime.text = "Audio time : " + _audioSource.time.ToString();
	}
}