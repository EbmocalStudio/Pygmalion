using UnityEngine;
using System.Collections.Generic;

public class FableAsset : ScriptableObject {
	[System.Serializable]
	public class NarrationMarker {
		public float audioTime;
		public int sample;

		public NarrationMarker(float audioTime, int sample) {
			this.audioTime = audioTime;
			this.sample = sample;
		}
	}

	public string fableTitle = "";
	public string fableText = "";
	public AudioClip audioClip;

	[SerializeField]
	public List<NarrationMarker> markers = new List<NarrationMarker>();

	public FableAsset(string title, string text, AudioClip audioClip) {
		fableTitle = title;
		fableText = text;
		this.audioClip = audioClip;
	}
}