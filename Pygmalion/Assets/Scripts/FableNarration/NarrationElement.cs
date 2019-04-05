using UnityEngine;
using UnityEngine.UI;
using TextUtil;

public class NarrationElement : MonoBehaviour {
	#region Variables
	public FableAsset fableAsset;
	public Text text;
	public DynamicTextData TextData { get; private set; }

	[HideInInspector]
    public AudioClip audioClip;
	[HideInInspector]
	public int currentTimestamp = -1;

	public NarrationElement queuedAfter;

	[HideInInspector]
	public bool hasBeenQueued = false;

	private static NarrationManager _manager;
	private static NarrationUIManager _uiManager;

	private float[] _markers;
	private bool _isHoverActive = true;
	#endregion

	#region Unity functions
	private void Awake() {
		if(_manager == null)
			_manager = FindObjectOfType(typeof(NarrationManager)) as NarrationManager;
		if(_uiManager == null)
			_uiManager = FindObjectOfType(typeof(NarrationUIManager)) as NarrationUIManager;

		text.enabled = false;
		text.text = fableAsset.fableText;
		audioClip = fableAsset.audioClip;

		//Currently only supporting autioTime ( no sample support ).
		_markers = new float[fableAsset.markers.Count];
		for(int i = 0; i < fableAsset.markers.Count; i++)
			_markers[i] = fableAsset.markers[i].audioTime;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if(queuedAfter == null || queuedAfter.hasBeenQueued) {
			if(!hasBeenQueued && text.text.Length > 0) {
				hasBeenQueued = true;
				_manager.QueueElement(this);
			}
		}
	}

	private void OnMouseDown() {
		DynamicTextData.WordData word = GetWordFromPosition(TextData, Camera.main.ScreenToWorldPoint(Input.mousePosition));
		
		if(word != null)
			_isHoverActive = _uiManager.ToggleDefinitionWindow(word);
	}

	private void OnMouseOver() {
		if(_isHoverActive) {
			DynamicTextData.WordData word = GetWordFromPosition(TextData, Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if(word != null)
				_uiManager.MouseHighlight(TextData, word, Color.cyan);
		}
	}
	#endregion

	#region functions
	public DynamicTextData.WordData UpdateMarker(float audioTime) {
		if(currentTimestamp + 1 < _markers.Length && _markers[currentTimestamp + 1] < audioTime) {
			currentTimestamp++;
			return TextData.words[currentTimestamp];
		}

		return null;
	}

	public void LoadData(float scale) {
		TextGenerationSettings settings = text.GetGenerationSettings(text.rectTransform.rect.size);
		TextGenerator textGenerator = new TextGenerator();
		textGenerator.Populate(text.text, settings);
		//Find another way to implement scale.
	
		TextData = new DynamicTextData();
		TextData.Populate(text.rectTransform, fableAsset.fableText, textGenerator.characters, text.fontSize, scale);
	}

	private static DynamicTextData.WordData GetWordFromPosition(DynamicTextData textData, Vector2 position) {
		if(textData != null) {
			foreach(DynamicTextData.WordData word in textData.words) {
				Vector2 pos = (Vector2)textData.containerTransform.position + word.position;
				if(position.x < pos.x - word.size.x / 2 || position.x > pos.x + word.size.x / 2)
					continue;
				if(position.y < pos.y - word.size.y / 2 || position.y > pos.y + word.size.y / 2)
					continue;

				return word;
			}
		}

		return null;
	}
	#endregion
}