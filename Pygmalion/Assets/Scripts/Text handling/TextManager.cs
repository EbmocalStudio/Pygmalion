using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TextManager : MonoBehaviour {
	//Contains data about every characters and words in a given string.
	private class DynamicTextData {
		public class CharacterData {
			public char character;
			//Local position of the character in the Text object's RectTransform.
			public Vector2 localPos;
			//World size of the character.
			public Vector2 worldSize;

			public CharacterData(char character, UICharInfo charInfo, float fontSize, float scale) {
				this.character = character;
				worldSize = new Vector2(charInfo.charWidth, fontSize);
				localPos = new Vector2(charInfo.cursorPos.x + worldSize.x / 2, charInfo.cursorPos.y - worldSize.y / 2) * scale;
				worldSize *= scale;
			}
		}

		public class WordData {
			//Local position of the word in the Text object's RectTransform.
			public Vector2 position;
			//World size of the word.
			public Vector2 size;
			public string text;
			//First character index of the word in the CharacterData list.
			public int firstIndex;
			public int length;

			public WordData(string text, int firstIndex, int lastIndex, List<CharacterData> charData) {
				this.text = text;
				this.firstIndex = firstIndex;
				length = lastIndex - firstIndex;

				//Figures out the size of the word using the first char's top left vertex position and the last char's bottom right vertex position.
				CharacterData firstChar = charData[firstIndex];
				CharacterData lastChar = charData[lastIndex];
				Vector2 TopLeftVertex = new Vector2(firstChar.localPos.x - firstChar.worldSize.x / 2, firstChar.localPos.y + firstChar.worldSize.y / 2);
				Vector2 BottomRightVertex = new Vector2(lastChar.localPos.x + lastChar.worldSize.x / 2, lastChar.localPos.y - lastChar.worldSize.y / 2);
				size = new Vector2(BottomRightVertex.x - TopLeftVertex.x, firstChar.worldSize.y);

				//Figures out the word's local position using the vertices position, half width and half height.
				position = new Vector2(TopLeftVertex.x + size.x / 2, BottomRightVertex.y + size.y / 2);
			}
		}

		public List<CharacterData> characters;
		public List<WordData> words;

		//Fills up the characters and words lists using the given string and various properties.
		public void Populate(string text, IList<UICharInfo> charInfo, float fontSize, float scale) {
			characters = new List<CharacterData>();
			words = new List<WordData>();

			int wordIndex = 0;
			//This loop parses the text, splits all words and adds them to the WordData list.
			for(int i = 0; i < text.Length; i++) {
				characters.Add(new CharacterData(text[i], charInfo[i], fontSize, scale));

				/* TODO :
				 * Change conditions to allow a certain flexibility and avoid certain exceptions.
				 * E.g. : ; ' - ! ? ( )
				 */
				if(text[i] == ' ' || text[i] == ',' || text[i] == '.') {
					int length = i - wordIndex;
					if(length > 0) {
						//If a word separating char is found, add the completed word to the list.
						words.Add(new WordData(text.Substring(wordIndex, length), wordIndex, i - 1, characters));
					}

					//Change the starting index of the new word to the next index.
					wordIndex = i + 1;
				}
			}
		}
	}

	public Text textExample;
	public Text highlightExample;
	public GameObject definitionExample;

	private TextGenerator _textGenerator;
	private DynamicTextData _textData;
	private float _canvaScale;
	private bool _isMouseEventActive = true;

	private void Start() {
		TextGenerationSettings settings = textExample.GetGenerationSettings(textExample.rectTransform.rect.size);
		_textGenerator = textExample.cachedTextGenerator;
		_textGenerator.Populate(textExample.text, settings);
		_canvaScale = gameObject.GetComponent<RectTransform>().localScale.x;

		_textData = new DynamicTextData();
		_textData.Populate(textExample.text, _textGenerator.characters, textExample.fontSize, _canvaScale);
	}

	private void Update() {
		//Debugging.
		DrawCharacters(Time.deltaTime, Color.red);
		DrawWords(Time.deltaTime, Color.blue);
	}

	private void OnMouseDown() {
		if(_isMouseEventActive) {
			DynamicTextData.WordData word = HighLightWord(Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.green);

			if(word != null) {
				definitionExample.SetActive(true);
				definitionExample.transform.GetChild(0).GetComponent<Text>().text = word.text;
				definitionExample.transform.GetChild(1).GetComponent<Text>().text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
				_isMouseEventActive = false;
			}
		}
	}

	public void DisableDefinitionPanel() {
		definitionExample.SetActive(false);
		highlightExample.text = "";
		_isMouseEventActive = true;
	}

	private void OnMouseOver() {
		if(_isMouseEventActive)
			HighLightWord(Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);
	}

	private DynamicTextData.WordData HighLightWord(Vector2 position, Color color) {
		foreach(DynamicTextData.WordData word in _textData.words) {
			if(position.x < word.position.x - word.size.x / 2 || position.x > word.position.x + word.size.x / 2)
				continue;
			if(position.y < word.position.y - word.size.y / 2 || position.y > word.position.y + word.size.y / 2)
				continue;

			highlightExample.text = word.text;
			highlightExample.rectTransform.position = word.position;
			highlightExample.color = color;

			return word;
		}

		return null;
	}

	//For debugging purposes, draws the extents of all characters in a given dynamic string.
	private void DrawCharacters(float duration, Color color) {
		foreach(DynamicTextData.CharacterData data in _textData.characters) {
			DebugTools.DrawRectangle(
				(Vector2)textExample.rectTransform.position + data.localPos,
				data.worldSize.x / 2,
				data.worldSize.y / 2,
				color,
				duration
			);
		}
	}

	//For debugging purposes, draws the extents of all words in a given dynamic string.
	private void DrawWords(float duration, Color color) {
		foreach(DynamicTextData.WordData data in _textData.words) {
			DebugTools.DrawRectangle(
				(Vector2)textExample.rectTransform.position + data.position,
				data.size.x / 2,
				data.size.y / 2,
				color,
				duration
			);
		}
	}
}