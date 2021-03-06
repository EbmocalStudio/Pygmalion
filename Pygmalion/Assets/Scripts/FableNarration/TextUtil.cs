﻿using UnityEngine;
using System.Collections.Generic;

namespace TextUtil {
	//Contains data about every characters and words in a given string.
	public class DynamicTextData {
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
		public RectTransform containerTransform;

		//Fills up the characters and words lists using the given string and various properties.
		public void Populate(RectTransform containerTransform, string text, IList<UICharInfo> charInfo, float fontSize, float scale) {
			this.containerTransform = containerTransform;
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
				if(text[i] == ' ' || text[i] == ',' || text[i] == '.' || text[i] == '\0' || text[i] == '!' || text[i] == ':' || text[i] == '\n') {
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
}