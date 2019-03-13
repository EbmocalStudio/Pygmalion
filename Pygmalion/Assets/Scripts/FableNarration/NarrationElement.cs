using UnityEngine;
using UnityEngine.UI;
using TextUtil;

public class NarrationElement : MonoBehaviour {
	#region Variables
	public FableAsset fableAsset;
	public Text text;
    public Text highlightText;
	public DynamicTextData TextData { get; private set; }

	[HideInInspector]
    public AudioClip audioClip;
	[HideInInspector]
	public int currentTimestamp = -1;

	public NarrationElement queuedAfter;

	[HideInInspector]
	public bool hasBeenQueued = false;

	//public DynamicTextData TextData { get; private set; }

	private NarrationManager _manager;
	private float[] _markers;
	#endregion

	#region Unity functions
	private void Awake() {
		_manager = transform.parent.parent.GetComponent<NarrationManager>();
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

	//public void LoadData(float scale) {
	//	TextGenerationSettings settings = text.GetGenerationSettings(text.rectTransform.rect.size);
	//	TextGenerator textGenerator = new TextGenerator();
	//	textGenerator.Populate(text.text, settings);
	//	//Find another way to implement scale.
	//
	//	TextData = new DynamicTextData();
	//	TextData.Populate(text.rectTransform, text.text, textGenerator.characters, text.fontSize, scale);
	//}
	//
	//public DynamicTextData.WordData UpdateWord(float audioTime) {
	//	if(_currentTimestamp + 1 < timestamps.Length && timestamps[_currentTimestamp + 1] < audioTime) {
	//		_currentTimestamp++;
	//		return TextData.words[_currentTimestamp];
	//	}
	//
	//	return null;
	//}
	//
	//public void HighlightWord(DynamicTextData.WordData word, Color color) {
	//	highlightText.text = word.text;
	//	highlightText.rectTransform.position = (Vector2)TextData.containerTransform.position + word.position;
	//	highlightText.color = color;
	//}
	#endregion
}