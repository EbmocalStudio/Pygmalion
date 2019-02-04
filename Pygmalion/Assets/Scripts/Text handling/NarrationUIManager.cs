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
	
	public Text highlightExample;
	public GameObject definitionExample;
	public DynamicTextData textData;
	
	private bool _isHoverActive = true;

	private void Update() {
		//Debugging.
		DrawCharacters(Time.deltaTime, Color.red);
		DrawWords(Time.deltaTime, Color.blue);
	}

	private void OnMouseDown() {
		DynamicTextData.WordData word = GetWordFromPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

		if(word != null) {
			if(!definitionExample.activeSelf) {
				definitionExample.SetActive(true);
				definitionExample.transform.GetChild(0).GetComponent<Text>().text = word.text;
				definitionExample.transform.GetChild(1).GetComponent<Text>().text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
				HighlightWord(word, Color.green);
				_isHoverActive = false;
			}
			else if(definitionExample.transform.GetChild(0).GetComponent<Text>().text == word.text)
				DisableDefinitionPanel();
		}
	}

	public void DisableDefinitionPanel() {
		definitionExample.SetActive(false);
		highlightExample.text = "";
		_isHoverActive = true;
	}

	private void OnMouseOver() {
		if(_isHoverActive) {
			DynamicTextData.WordData word = GetWordFromPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if(word != null)
				HighlightWord(word, Color.red);
		}
	}

	private DynamicTextData.WordData GetWordFromPosition(Vector2 position) {
		foreach(DynamicTextData.WordData word in textData.words) {
			Vector2 pos = (Vector2)textData.containerTransform.position + word.position;
			if(position.x < pos.x - word.size.x / 2 || position.x > pos.x + word.size.x / 2)
				continue;
			if(position.y < pos.y - word.size.y / 2 || position.y > pos.y + word.size.y / 2)
				continue;

			return word;
		}

		return null;
	}

	public void HighlightWord(DynamicTextData.WordData word, Color color) {
		highlightExample.text = word.text;
		highlightExample.rectTransform.position = (Vector2)textData.containerTransform.position + word.position;
		highlightExample.color = color;
	}

	//For debugging purposes, draws the extents of all characters in a given dynamic string.
	private void DrawCharacters(float duration, Color color) {
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
	private void DrawWords(float duration, Color color) {
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