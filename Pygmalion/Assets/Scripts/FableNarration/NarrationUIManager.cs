using UnityEngine;
using UnityEngine.UI;
using TextUtil;

public class NarrationUIManager : MonoBehaviour {
	//ToDo :
	// - Make independent of textExample and DynamicTextData.
	// -> Move stuff to NarrationManager ?
	// - Clean canvas scale.
	// - Clean rectTransform utilization.
	// - Figure a way to have the canvas region follow the camera without moving the text.
	
	public GameObject definitionWindow;
	public Text audioHighlight;
	public Text playerHighlight;
	public bool debuggingOn;
	public Color debuggingCharacterColor;
	public Color debuggingWordColor;

	private DynamicTextData _textData;
	private Text _currentHighlight;

	private void Update() {
		if(_textData != null && debuggingOn) {
			//Debugging.
			DrawCharacters(_textData, Time.deltaTime, debuggingCharacterColor);
			DrawWords(_textData, Time.deltaTime, debuggingWordColor);
		}
	}

	public bool ToggleDefinitionWindow(DynamicTextData.WordData word) {
		if(!definitionWindow.activeSelf) {
			definitionWindow.SetActive(true);
			definitionWindow.transform.GetChild(0).GetComponent<Text>().text = word.text;
			definitionWindow.transform.GetChild(1).GetComponent<Text>().text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
			HighlightWord(word, Color.green);

			return false;
		}
		else if(definitionWindow.transform.GetChild(0).GetComponent<Text>().text == word.text)
			DisableDefinitionPanel();

		return true;
	}

	public void DisableDefinitionPanel() {
		definitionWindow.SetActive(false);
		_currentHighlight.text = "";
	}

	public void SetEmpty() {
		_textData = null;
		_currentHighlight = null;
	}

	public void LoadData(DynamicTextData textData) {
		_textData = textData;
	}

	public void HighlightWord(DynamicTextData.WordData word, Color color) {
		Highlight(audioHighlight, _textData, word, color);
	}

	public void MouseHighlight(DynamicTextData textData, DynamicTextData.WordData word, Color color) {
		Highlight(playerHighlight, textData, word, color);
	}

	public void Highlight(Text textObject, DynamicTextData textData, DynamicTextData.WordData word, Color color) {
		textObject.text = word.text;
		textObject.rectTransform.position = (Vector2)_textData.containerTransform.position + word.position;
		textObject.color = color;
	}

	//For debugging purposes, draws the extents of all characters in a given dynamic string.
	private static void DrawCharacters(DynamicTextData textData, float duration, Color color) {
		foreach(DynamicTextData.CharacterData data in textData.characters) {
			DebugTools.DrawRectangle(
				(Vector2)textData.containerTransform.position + data.localPos,
				data.worldSize.x / 2,
				data.worldSize.y / 2,
				color,
				duration
			);
		}
	}

	//For debugging purposes, draws the extents of all words in a given dynamic string.
	private static void DrawWords(DynamicTextData textData, float duration, Color color) {
		foreach(DynamicTextData.WordData data in textData.words) {
			DebugTools.DrawRectangle(
				(Vector2)textData.containerTransform.position + data.position,
				data.size.x / 2,
				data.size.y / 2,
				color,
				duration
			);
		}
	}
}